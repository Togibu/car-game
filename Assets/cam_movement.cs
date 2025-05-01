using UnityEngine;

public class cam_movement : MonoBehaviour
{
    public Transform target; // Das Ziel, um das sich die Kamera dreht (z. B. das Auto)
    public float rotationSpeed = 5f; // Geschwindigkeit der Kameradrehung
    public float zoomSpeed = 10f; // Geschwindigkeit des Zoomens
    public float minZoomDistance = 5f; // Minimaler Abstand zum Ziel
    public float maxZoomDistance = 20f; // Maximaler Abstand zum Ziel
    private bool isRotating = false; // Gibt an, ob die Kamera gerade rotiert
    private float idleTime = 0f; // Zeit seit der letzten Eingabe
    public float autoRotateSpeed = 20f; // Geschwindigkeit der automatischen Rotation
    private const float idleThreshold = 60f; // Zeit in Sekunden, bevor die automatische Rotation startet

    // Standardposition und -rotation der Kamera
    private Vector3 defaultPosition = new Vector3(0f, 1.76f, -4.72f);
    private Quaternion defaultRotation = Quaternion.Euler(15f, 0f, 0f);

    void Start()
    {
        // Setze die Kamera auf die Standardposition und -rotation beim Start
        ResetCamera();
    }

    void Update()
    {
        // Überprüfen, ob eine Taste gedrückt wird
        if (Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (idleTime >= idleThreshold)
            {
                // Kamera auf Standardposition zurücksetzen, wenn sie zuvor inaktiv war
                ResetCamera();
            }
            idleTime = 0f; // Zurücksetzen des Timers bei Eingabe
        }
        else
        {
            idleTime += Time.deltaTime; // Timer erhöhen, wenn keine Eingabe erfolgt
        }

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
        else if (idleTime >= idleThreshold)
        {
            // Automatische Rotation, wenn die Kamera 60 Sekunden lang inaktiv war
            transform.RotateAround(target.position, Vector3.up, autoRotateSpeed * Time.deltaTime);
        }

        // Zoom mit dem Mausrad
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 direction = transform.position - target.position;
        float distance = direction.magnitude;

        // Berechne den neuen Abstand basierend auf dem Scroll-Eingang
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minZoomDistance, maxZoomDistance);

        // Setze die neue Position der Kamera
        transform.position = target.position + direction.normalized * distance;

        // Kamera immer auf das Ziel ausrichten
        transform.LookAt(target);
    }

    private void ResetCamera()
    {
        // Setze die Kamera auf die gespeicherte Standardposition und -rotation zurück
        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
    }
}
