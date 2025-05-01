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
        if (speeed.magnitude > MaxSpeed)
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
            turn = true;
        }

        // Rückwärtsbewegung
        if (Input.GetKey(KeyCode.S) && isGrounded)
        {
            rb.AddForce(-transform.forward * MaxSpeed);
            turn = true;
        }

        // Linksdrehung
        if (Input.GetKey(KeyCode.A) && turn)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }

        // Rechtsdrehung
        if (Input.GetKey(KeyCode.D) && turn)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }

        // Zurücksetzen der Drehung
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            turn = false;
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
