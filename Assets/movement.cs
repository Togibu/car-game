using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    public float turnSpeed = 50f; // Drehgeschwindigkeit des Autos
    public float brakeSpeed = 5f; // Bremskraft
    public Rigidbody rb;
    private bool turn;
    public Transform resetPosition; // Position, zu der das Auto zurückgesetzt werden soll
    public float MaxSpeed = 10f;
    bool isGrounded = false; // Flag, um zu überprüfen, ob das Auto den Boden berührt
    public GameObject Ground;
    private float currentSpeed = 0f; // Aktuelle Geschwindigkeit für realistische Lenkung
    public float minSpeedForTurning = 1f; // Minimale Geschwindigkeit, um lenken zu können
    private float currentTurnInput = 0f; // Aktuelle Lenkeingabe für sanfte Übergänge
    public float turnAcceleration = 3f; // Wie schnell die Lenkung beschleunigt
    public float turnDeceleration = 5f; // Wie schnell die Lenkung abbremst

    // Start is called before the first frame update
    void Start()
    {
        turn = false;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Cursor im Spiel sperren
        Cursor.visible = false; // Cursor unsichtbar machen
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
        
        if (currentSpeed > MaxSpeed)
        {
            Vector3 newVelocity = speeed.normalized * MaxSpeed;
            rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Auto zurücksetzen
            transform.position = resetPosition.position;
            transform.rotation = resetPosition.rotation;
           
            rb.linearVelocity = Vector3.zero;       // Stoppt lineare Bewegung
            rb.angularVelocity = Vector3.zero; // Stoppt Rotation

            
        }
        // Vorwärtsbewegung
        if (Input.GetKey(KeyCode.W) && isGrounded)
        {
            rb.AddForce(transform.forward * MaxSpeed);
        }

        // Rückwärtsbewegung
        if (Input.GetKey(KeyCode.S) && isGrounded)
        {
            rb.AddForce(-transform.forward * MaxSpeed);
        }

        // Realistische Lenkung: Nur wenn das Auto sich bewegt
        float speedFactor = Mathf.Clamp01(currentSpeed / minSpeedForTurning); // Lenkfaktor basierend auf Geschwindigkeit
        
        // Lenkeingabe erfassen
        float targetTurnInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            targetTurnInput = -1f; // Links
        }
        else if (Input.GetKey(KeyCode.D))
        {
            targetTurnInput = 1f; // Rechts
        }
        
        // Sanfte Beschleunigung/Verlangsamung der Lenkung
        if (targetTurnInput != 0f)
        {
            // Beschleunigung zur Ziel-Lenkeingabe
            currentTurnInput = Mathf.MoveTowards(currentTurnInput, targetTurnInput, turnAcceleration * Time.deltaTime);
        }
        else
        {
            // Verlangsamung zu 0, wenn keine Taste gedrückt wird
            currentTurnInput = Mathf.MoveTowards(currentTurnInput, 0f, turnDeceleration * Time.deltaTime);
        }
        
        // Lenkung anwenden, nur wenn Auto sich bewegt und wir eine Lenkeingabe haben
        if (speedFactor > 0.1f && Mathf.Abs(currentTurnInput) > 0.01f && isGrounded)
        {
            float turnAmount = currentTurnInput * turnSpeed * speedFactor * Time.deltaTime;
            
            // Bei Rückwärtsfahrt umgekehrte Lenkung (wie bei echten Autos)
            if (Vector3.Dot(rb.linearVelocity, transform.forward) < 0) // Rückwärts
            {
                turnAmount = -turnAmount;
            }
            
            transform.Rotate(Vector3.up, turnAmount);
        }

        // Bremsen, wenn keine Bewegungstasten gedrückt werden
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, brakeSpeed * Time.deltaTime);
        }
    }

    void OnCollisionExit(Collision other)
    {
        isGrounded = false; // Auto verlässt den Boden
    }
    void OnCollisionStay(Collision other)
    {
        isGrounded = true; // Auto bleibt auf dem Boden
    }
    void OnCollisionEnter(Collision other)
    {
        isGrounded = true; // Auto berührt den Boden
    }
}
