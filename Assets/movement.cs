using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class movement : MonoBehaviour
{

    public GameObject TextMeshPro;
    private TextMeshProUGUI textMeshProUGUI;
    public float turnSpeed = 55f; // Drehgeschwindigkeit des Autos
    public float brakeSpeed = 8f; // Bremskraft (als Zusatzwiderstand)
    public Rigidbody rb;
    private bool turn;
    public Transform resetPosition; // Position, zu der das Auto zurückgesetzt werden soll
    public float MaxSpeed = 14f;
    public float MaxReverseSpeed = 6f;
    bool isGrounded = false; // Flag, um zu überprüfen, ob das Auto den Boden berührt
    public GameObject Ground;
    private float currentSpeed = 0f; // Aktuelle Geschwindigkeit für realistische Lenkung
    public float minSpeedForTurning = 2f; // Minimale Geschwindigkeit, um lenken zu können
    private float currentTurnInput = 0f; // Aktuelle Lenkeingabe für sanfte Übergänge
    public float turnAcceleration = 4f; // Wie schnell die Lenkung beschleunigt
    public float turnDeceleration = 6f; // Wie schnell die Lenkung abbremst
    public float drag = 2f; // widerstand
    public float acceleration = 12f; // Beschleunigung des Autos (public, einstellbar)
    public float sidewaysFriction = 8f; // Dämpft seitliches Rutschen
    public float groundedDrag = 2.5f;
    public float airDrag = 0.2f;
    public float angularDrag = 2.5f;
    public LayerMask groundMask = ~0;
    public float groundCheckDistance = 0.35f;
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.35f, 0f);
    public float maxAngularVelocity = 4f;

    private float throttleInput = 0f;

    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = TextMeshPro.GetComponent<TextMeshProUGUI>();
        turn = false;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Cursor im Spiel sperren
        Cursor.visible = false; // Cursor unsichtbar machen
        rb.drag = drag; // Setzt den widerstand des Autos
        rb.angularDrag = angularDrag;
        rb.centerOfMass += centerOfMassOffset;
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 maxPosition = transform.position;
        /*
        if (maxPosition.y < -30f || maxPosition.y > 30f || 
            maxPosition.x < -Ground.transform.localScale.x / 2f || maxPosition.x > Ground.transform.localScale.x / 2f || 
            maxPosition.z < -Ground.transform.localScale.z / 2f || maxPosition.z > Ground.transform.localScale.z / 2f)
        {
            // Auto zurücksetzen, wenn es unter die Bodenhöhe fällt
            transform.position = resetPosition.position;
            transform.rotation = resetPosition.rotation;

            rb.linearVelocity = Vector3.zero;       // Stoppt lineare Bewegung
            rb.angularVelocity = Vector3.zero; // Stoppt Rotation
        }
        */
        Vector3 speeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        currentSpeed = speeed.magnitude; // Aktuelle Geschwindigkeit berechnen
        TextOutput(currentSpeed);

        // Eingaben lesen (Update) und in FixedUpdate anwenden
        throttleInput = Mathf.Clamp(Input.GetAxisRaw("Vertical"), -1f, 1f);
        float targetTurnInput = Mathf.Clamp(Input.GetAxisRaw("Horizontal"), -1f, 1f);

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Auto zurücksetzen
            transform.position = resetPosition.position;
            transform.rotation = resetPosition.rotation;
           
            rb.velocity = Vector3.zero;       // Stoppt lineare Bewegung
            rb.angularVelocity = Vector3.zero; // Stoppt Rotation

            
        }
        // Sanfte Beschleunigung/Verlangsamung der Lenkung
        if (Mathf.Abs(targetTurnInput) > 0.01f)
        {
            currentTurnInput = Mathf.MoveTowards(currentTurnInput, targetTurnInput, turnAcceleration * Time.deltaTime);
        }
        else
        {
            currentTurnInput = Mathf.MoveTowards(currentTurnInput, 0f, turnDeceleration * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundMask);
        rb.drag = isGrounded ? groundedDrag : airDrag;

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        currentSpeed = horizontalVelocity.magnitude;
        float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);

        // Vorwärts/Rückwärts-Antrieb
        if (isGrounded)
        {
            if (throttleInput > 0f && forwardSpeed < MaxSpeed)
            {
                rb.AddForce(transform.forward * acceleration * throttleInput, ForceMode.Acceleration);
            }
            else if (throttleInput < 0f && forwardSpeed > -MaxReverseSpeed)
            {
                rb.AddForce(transform.forward * acceleration * throttleInput, ForceMode.Acceleration);
            }
        }

        // Zusätzliche Bremswirkung, wenn kein Gas gegeben wird
        if (isGrounded && Mathf.Abs(throttleInput) < 0.01f && horizontalVelocity.sqrMagnitude > 0.1f)
        {
            rb.AddForce(-horizontalVelocity.normalized * brakeSpeed, ForceMode.Acceleration);
        }

        // Seitliches Rutschen dämpfen
        if (isGrounded && horizontalVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 lateralVelocity = Vector3.Dot(rb.velocity, transform.right) * transform.right;
            rb.AddForce(-lateralVelocity * sidewaysFriction, ForceMode.Acceleration);
        }

        // Lenkung anwenden, nur wenn Auto sich bewegt
        float speedFactor = Mathf.Clamp01(currentSpeed / minSpeedForTurning);
        if (speedFactor > 0.05f && Mathf.Abs(currentTurnInput) > 0.01f && isGrounded)
        {
            float turnAmount = currentTurnInput * turnSpeed * speedFactor * Time.fixedDeltaTime;
            if (forwardSpeed < 0f)
            {
                turnAmount = -turnAmount;
            }
            Quaternion deltaRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    void TextOutput(float speed)
    {
        textMeshProUGUI.text = "Speed: " + (speed * 3.6f).ToString("F1") + " Km/h";
    }

    bool moving()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        return horizontalVelocity.magnitude > 0.4f;
    }
    void OnCollisionExit(Collision other) { }
    void OnCollisionStay(Collision other) { }
    void OnCollisionEnter(Collision other) { }
}
