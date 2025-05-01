using UnityEngine;

public class cam_movement : MonoBehaviour
{
    public Transform target; // Das Ziel, um das sich die Kamera dreht (z. B. das Auto)
    public float rotationSpeed = 5f; // Geschwindigkeit der Kameradrehung
    public float zoomSpeed = 10f; // Geschwindigkeit des Zoomens
    public float minZoomDistance = 5f; // Minimaler Abstand zum Ziel
    public float maxZoomDistance = 20f; // Maximaler Abstand zum Ziel
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

        // Zoom mit dem Mausrad
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 direction = transform.position - target.position;
        float distance = direction.magnitude;

        // Berechne den neuen Abstand basierend auf dem Scroll-Eingang
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minZoomDistance, maxZoomDistance);

        // Setze die neue Position der Kamera
        transform.position = target.position + direction.normalized * distance;
    }
}
