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
            
        }
        // Vorwärtsbewegung
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * MaxSpeed);
            turn = true;
        }

        // Rückwärtsbewegung
        if (Input.GetKey(KeyCode.S))
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
}
