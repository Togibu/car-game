using UnityEngine;

public class policelight : MonoBehaviour
{
    public float toggleInterval = 1.0f; // Zeitintervall zwischen den Lichtwechseln

    private bool isFlashing = false; // Gibt an, ob die Lichter blinken sollen
    private float timer = 0f; // Timer für das Umschalten der Lichter
    public GameObject blueLightObject; // Referenz auf das blaue Licht-GameObject
    private Light bluelight; // Light-Komponente des blauen Lichts
    public GameObject redLightObject; // Referenz auf das rote Licht-GameObject
    private Light redLight; // Light-Komponente des roten Lichts
    private bool isBlueLightOn = true; // Gibt an, welches Licht gerade an ist

    void Start()
    {
        // Holen der Light-Komponenten von den zugewiesenen GameObjects
        bluelight = blueLightObject.GetComponent<Light>();
        redLight = redLightObject.GetComponent<Light>();

        // Sicherstellen, dass beide Lichter initial ausgeschaltet sind
        bluelight.enabled = false;
        redLight.enabled = false;
    }

    void Update()
    {
        // Überprüfen, ob die Taste "F" gedrückt wurde
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlashing = !isFlashing; // Umschalten des Blinkzustands
            if (!isFlashing)
            {
                // Beide Lichter ausschalten, wenn das Blinken gestoppt wird
                bluelight.enabled = false;
                redLight.enabled = false;
            }
        }

        // Wenn die Lichter blinken sollen
        if (isFlashing)
        {
            timer += Time.deltaTime;

            // Umschalten der Lichter nach dem festgelegten Intervall
            if (timer >= toggleInterval)
            {
                if (isBlueLightOn)
                {
                    bluelight.enabled = true;
                    redLight.enabled = false;
                }
                else
                {
                    bluelight.enabled = false;
                    redLight.enabled = true;
                }

                isBlueLightOn = !isBlueLightOn; // Umschalten zwischen blauem und rotem Licht
                timer = 0f; // Timer zurücksetzen
            }
        }
    }
}
