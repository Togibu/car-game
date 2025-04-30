using UnityEngine;

public class cam_movement : MonoBehaviour
{
    public Transform target; // Das Ziel, um das sich die Kamera dreht (z. B. das Auto)
    public float rotationSpeed = 5f; // Geschwindigkeit der Kameradrehung
    private bool isRotating = false; // Gibt an, ob die Kamera gerade rotiert

    void Update()
    {
        // Überprüfen, ob die Taste "C" gedrückt wird
        if (Input.GetKey(KeyCode.C))
        {
            isRotating = true;
        }
        else
        {
            isRotating = false;
        }

        // Kamera mit der Maus bewegen, wenn "C" gedrückt wird
        if (isRotating)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            // Kamera um das Ziel rotieren
            transform.RotateAround(target.position, Vector3.up, mouseX);
            transform.RotateAround(target.position, transform.right, -mouseY);
        }
    }
}
