using UnityEngine;

public class cam_movement : MonoBehaviour
{
    public Transform target; // Das Ziel, um das sich die Kamera dreht (z. B. das Auto)
    public float rotationSpeed = 5f; // Geschwindigkeit der Kameradrehung
    public float zoomSpeed = 10f; // Geschwindigkeit des Zoomens
    public float minZoomDistance = 5f; // Minimaler Abstand zum Ziel
    public float maxZoomDistance = 20f; // Maximaler Abstand zum Ziel
    public float firstPersonDistance = 1f; // Abstand, ab dem in First-Person-Modus gewechselt wird
    private bool isRotating = false; // Gibt an, ob die Kamera gerade rotiert
    private bool isFirstPerson = false; // Gibt an, ob wir in der First-Person-Ansicht sind
    private Renderer[] carRenderers; // Array aller Renderer des Autos
    private float idleTime = 0f; // Zeit seit der letzten Eingabe
    public float autoRotateSpeed = 20f; // Geschwindigkeit der automatischen Rotation
    private const float idleThreshold = 60f; // Zeit in Sekunden, bevor die automatische Rotation startet

    // Standardposition und -rotation der Kamera
    private Vector3 defaultPosition = new Vector3(0f, 1.76f, -4.72f);
    private Quaternion defaultRotation = Quaternion.Euler(15f, 0f, 0f);

    void Start()
    {
        // Alle Renderer des Autos finden (für das Ein-/Ausblenden)
        if (target != null)
        {
            carRenderers = target.GetComponentsInChildren<Renderer>();
        }
        
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

        // Überprüfen, ob eine Maustaste gedrückt wird
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) // Linke, rechte oder mittlere Maustaste
        {
            isRotating = true;
        }
        else
        {
            isRotating = false;
        }

        // Kamera mit der Maus bewegen, wenn eine Maustaste gedrückt wird
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

        // Spezielle Behandlung für First-Person-Modus
        if (isFirstPerson && scroll < 0) // Herauszoomen aus First-Person
        {
            // Verlasse First-Person-Modus und setze auf minimale Distanz
            distance = firstPersonDistance + 0.5f;
        }
        else if (!isFirstPerson)
        {
            // Normale Zoom-Logik für Third-Person-Modus
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minZoomDistance, maxZoomDistance);
        }
        // Hineinzoomen in First-Person bleibt wie gehabt

        // Überprüfe, ob wir in First-Person-Modus wechseln sollten
        bool shouldBeFirstPerson = distance <= firstPersonDistance;
        
        if (shouldBeFirstPerson && !isFirstPerson)
        {
            // In First-Person-Modus wechseln
            EnterFirstPerson();
        }
        else if (!shouldBeFirstPerson && isFirstPerson)
        {
            // First-Person-Modus verlassen
            ExitFirstPerson();
        }

        if (!isFirstPerson)
        {
            // Setze die neue Position der Kamera (nur wenn nicht in First-Person)
            transform.position = target.position + direction.normalized * distance;
            
            // Kamera immer auf das Ziel ausrichten
            transform.LookAt(target);
        }
    }

    private void ResetCamera()
    {
        // Setze die Kamera auf die gespeicherte Standardposition und -rotation zurück
        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
    }

    private void EnterFirstPerson()
    {
        isFirstPerson = true;
        
        // Auto unsichtbar machen
        if (carRenderers != null)
        {
            foreach (Renderer renderer in carRenderers)
            {
                renderer.enabled = false;
            }
        }
        
        // Kamera direkt am Ziel positionieren (First-Person-Position)
        transform.position = target.position + Vector3.up * 0.5f; // Leicht erhöht für Augenhöhe
        transform.LookAt(target.position + target.forward * 10f); // Nach vorne schauen
    }

    private void ExitFirstPerson()
    {
        isFirstPerson = false;
        
        // Auto wieder sichtbar machen
        if (carRenderers != null)
        {
            foreach (Renderer renderer in carRenderers)
            {
                renderer.enabled = true;
            }
        }
    }
}
