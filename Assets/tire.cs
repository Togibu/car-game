using UnityEngine;

public class tire : MonoBehaviour
{
    [Header("Reifen Objekte")]
    public GameObject leftTire;  // Linker Reifen
    public GameObject rightTire; // Rechter Reifen
    
    [Header("Drehung Einstellungen")]
    public float maxTurnAngle = 30f; // Maximaler Drehwinkel
    public float turnSpeed = 5f;     // Geschwindigkeit der Drehung
    
    private float currentTurnAngle = 0f; // Aktueller Drehwinkel
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleTireRotation();
    }
    
    void HandleTireRotation()
    {
        float targetAngle = 0f;
        
        // Prüfe Eingaben
        if (Input.GetKey(KeyCode.A))
        {
            targetAngle = -maxTurnAngle; // Links drehen (negativ)
        }
        else if (Input.GetKey(KeyCode.D))
        {
            targetAngle = maxTurnAngle;  // Rechts drehen (positiv)
        }
        
        // Sanfte Übergang zum Zielwinkel
        currentTurnAngle = Mathf.Lerp(currentTurnAngle, targetAngle, turnSpeed * Time.deltaTime);
        
        // Wende Rotation auf beide Reifen an
        if (leftTire != null)
        {
            // Linkes Rad ist 180° gedreht eingebaut - komplett invertierte Steuerung
            leftTire.transform.localRotation = Quaternion.Euler(0f, 180f + currentTurnAngle, 0f);
        }
        
        if (rightTire != null)
        {
            rightTire.transform.localRotation = Quaternion.Euler(0f, currentTurnAngle, 0f);
        }
    }
}
