using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float speed = 10f; // Geschwindigkeit des Autos
    public float turnSpeed = 50f; // Drehgeschwindigkeit des Autos

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Vorwärts-/Rückwärtsbewegung (W/S oder Pfeiltasten hoch/runter)
        float moveInput = Input.GetAxis("Vertical"); 
        transform.Translate(Vector3.forward * moveInput * speed * Time.deltaTime);

        // Drehung (A/D oder Pfeiltasten links/rechts)
        float turnInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, turnInput * turnSpeed * Time.deltaTime);
    }
}
