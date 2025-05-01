using UnityEngine;

public class sun : MonoBehaviour
{
    public float rotationSpeed = 1.2f; // Geschwindigkeit der Rotation (Grad pro Sekunde)

    // Update wird einmal pro Frame aufgerufen
    void Update()
    {
        // Rotiert das Objekt um die Y-Achse
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * Time.deltaTime* 5f);
    }
}
