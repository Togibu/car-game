using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class movement : MonoBehaviour
{
    public enum DriveModel { Arcade, SimLight }
    [Header("Driving Model")]
    public DriveModel driveModel = DriveModel.Arcade;

    public GameObject TextMeshPro;
    private TextMeshProUGUI textMeshProUGUI;
    public float turnSpeed = 55f; // Drehgeschwindigkeit des Autos
    public float brakeSpeed = 8f; // Bremskraft (als Zusatzwiderstand)
    public Rigidbody rb;
    public Transform resetPosition; // Position, zu der das Auto zurückgesetzt werden soll
    [Header("Arcade Tuning")]
    public float maxSpeedKmh = 140f;
    public float maxReverseSpeedKmh = 40f;
    public float enginePower = 28f; // Basisbeschleunigung (gefühlt)
    public float accelFalloff = 0.8f; // 0..1: je höher, desto mehr Abfall bei hoher Speed
    public float rollingResistance = 1.5f;
    public float brakeStrength = 10f; // aktives Bremsen bei Gegenrichtung
    
    [Header("Sim-Light Tuning")]
    public float engineForce = 8000f; // Kraft in N (wirkt wie "Motorleistung")
    public float maxSpeedSimKmh = 170f;
    public float maxReverseSpeedSimKmh = 35f;
    public AnimationCurve torqueCurve = new AnimationCurve(
        new Keyframe(0f, 1.0f),
        new Keyframe(0.4f, 1.0f),
        new Keyframe(0.7f, 0.6f),
        new Keyframe(1f, 0.0f)
    );
    public float rollingResistanceSim = 300f; // N
    public float airDragCoeff = 0.35f; // N pro (m/s)^2
    public float brakeForce = 12000f; // N
    bool isGrounded = false; // Flag, um zu überprüfen, ob das Auto den Boden berührt
    public GameObject Ground;
    private float currentSpeed = 0f; // Aktuelle Geschwindigkeit für realistische Lenkung
    public float minSpeedForTurning = 2f; // Minimale Geschwindigkeit, um lenken zu können
    private float currentTurnInput = 0f; // Aktuelle Lenkeingabe für sanfte Übergänge
    public float turnAcceleration = 4f; // Wie schnell die Lenkung beschleunigt
    public float turnDeceleration = 6f; // Wie schnell die Lenkung abbremst
    public float drag = 2f; // widerstand
    public float acceleration = 12f; // Legacy (nicht mehr verwendet)
    public float sidewaysFriction = 8f; // Dämpft seitliches Rutschen
    public float groundedDrag = 2.5f;
    public float airDrag = 0.2f;
    public float angularDrag = 2.5f;
    public LayerMask groundMask = ~0;
    public float groundCheckDistance = 0.35f;
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.35f, 0f);
    public float maxAngularVelocity = 4f;

    private float throttleInput = 0f;
    private Collider cachedCollider;

    // Start is called before the first frame update
    void Start()
    {
        if (TextMeshPro != null)
        {
            textMeshProUGUI = TextMeshPro.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("movement: TextMeshPro GameObject is not assigned. Speed UI will be disabled.");
        }
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Cursor im Spiel sperren
        Cursor.visible = false; // Cursor unsichtbar machen
        rb.linearDamping = drag; // Setzt den widerstand des Autos
        rb.angularDamping = angularDrag;
        rb.centerOfMass += centerOfMassOffset;
        rb.maxAngularVelocity = maxAngularVelocity;
        // Prefer a child collider if the root has none (common for car meshes)
        cachedCollider = GetComponentInChildren<Collider>();
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
        Vector3 speeed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
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
           
            rb.linearVelocity = Vector3.zero;       // Stoppt lineare Bewegung
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
        Vector3 rayOrigin = cachedCollider != null ? cachedCollider.bounds.center : (transform.position + Vector3.up * 0.1f);
        float rayDistance = (cachedCollider != null ? cachedCollider.bounds.extents.y : 1f) + groundCheckDistance;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, rayDistance, groundMask);
        rb.linearDamping = isGrounded ? groundedDrag : airDrag;

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        currentSpeed = horizontalVelocity.magnitude;
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        float speedKmh = currentSpeed * 3.6f;

        if (driveModel == DriveModel.Arcade)
        {
            ApplyArcadeDrive(forwardSpeed, speedKmh, horizontalVelocity);
        }
        else
        {
            ApplySimDrive(forwardSpeed, speedKmh, horizontalVelocity);
        }

        // Seitliches Rutschen dämpfen
        if (isGrounded && horizontalVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 lateralVelocity = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
            rb.AddForce(-lateralVelocity * sidewaysFriction, ForceMode.Acceleration);
        }

        // Lenkung anwenden, nur wenn Auto sich bewegt
        float turnSpeedFactor = Mathf.Clamp01(currentSpeed / minSpeedForTurning);
        if (turnSpeedFactor > 0.05f && Mathf.Abs(currentTurnInput) > 0.01f && isGrounded)
        {
            float turnAmount = currentTurnInput * turnSpeed * turnSpeedFactor * Time.fixedDeltaTime;
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
        if (textMeshProUGUI == null) return;
        textMeshProUGUI.text = "Speed: " + (speed * 3.6f).ToString("F1") + " Km/h";
    }

    bool moving()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        return horizontalVelocity.magnitude > 0.4f;
    }
    void OnCollisionExit(Collision other) { }
    void OnCollisionStay(Collision other) { }
    void OnCollisionEnter(Collision other) { }

    void ApplyArcadeDrive(float forwardSpeed, float speedKmh, Vector3 horizontalVelocity)
    {
        float maxSpeed = maxSpeedKmh / 3.6f;
        float maxReverseSpeed = maxReverseSpeedKmh / 3.6f;

        if (isGrounded)
        {
            float speedRatio = Mathf.Clamp01(speedKmh / maxSpeedKmh);
            float accelFactor = Mathf.Lerp(1f, 1f - accelFalloff, speedRatio);
            float driveForce = enginePower * accelFactor * throttleInput;

            if (throttleInput > 0f && forwardSpeed < maxSpeed)
            {
                rb.AddForce(transform.forward * driveForce, ForceMode.Acceleration);
            }
            else if (throttleInput < 0f && forwardSpeed > -maxReverseSpeed)
            {
                rb.AddForce(transform.forward * driveForce, ForceMode.Acceleration);
            }
        }

        if (isGrounded && Mathf.Abs(throttleInput) < 0.01f && horizontalVelocity.sqrMagnitude > 0.1f)
        {
            rb.AddForce(-horizontalVelocity.normalized * rollingResistance, ForceMode.Acceleration);
        }

        if (isGrounded && Mathf.Abs(throttleInput) > 0.01f)
        {
            float desiredDir = Mathf.Sign(throttleInput);
            if (Mathf.Sign(forwardSpeed) != 0f && Mathf.Sign(forwardSpeed) != desiredDir)
            {
                rb.AddForce(-horizontalVelocity.normalized * brakeStrength, ForceMode.Acceleration);
            }
        }
    }

    void ApplySimDrive(float forwardSpeed, float speedKmh, Vector3 horizontalVelocity)
    {
        float maxSpeed = maxSpeedSimKmh / 3.6f;
        float maxReverseSpeed = maxReverseSpeedSimKmh / 3.6f;
        float speedRatio = Mathf.Clamp01(speedKmh / maxSpeedSimKmh);
        float torque = torqueCurve.Evaluate(speedRatio);

        if (isGrounded)
        {
            if (throttleInput > 0f && forwardSpeed < maxSpeed)
            {
                rb.AddForce(transform.forward * (engineForce * torque * throttleInput), ForceMode.Force);
            }
            else if (throttleInput < 0f && forwardSpeed > -maxReverseSpeed)
            {
                rb.AddForce(transform.forward * (engineForce * torque * throttleInput), ForceMode.Force);
            }
        }

        if (isGrounded && horizontalVelocity.sqrMagnitude > 0.05f)
        {
            Vector3 v = horizontalVelocity;
            Vector3 dragForce = -v.normalized * (rollingResistanceSim + airDragCoeff * v.sqrMagnitude);
            rb.AddForce(dragForce, ForceMode.Force);
        }

        if (isGrounded && Mathf.Abs(throttleInput) > 0.01f)
        {
            float desiredDir = Mathf.Sign(throttleInput);
            if (Mathf.Sign(forwardSpeed) != 0f && Mathf.Sign(forwardSpeed) != desiredDir)
            {
                rb.AddForce(-horizontalVelocity.normalized * brakeForce, ForceMode.Force);
            }
        }
    }
}
