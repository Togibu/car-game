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

    public GameObject light1;
    private Light light1Component; // Light-Komponente des ersten Lichts
    public GameObject light2;
    private Light light2Component; // Light-Komponente des zweiten Lichts


    public GameObject highBeam1; // Fernlicht 1
    private Light highBeam1Component;
    public GameObject highBeam2; // Fernlicht 2
    private Light highBeam2Component;

    private int lightMode = 0; // Speichert den aktuellen Lichtmodus

    void Start()
    {
        // Holen der Light-Komponenten von den zugewiesenen GameObjects
        bluelight = blueLightObject.GetComponent<Light>();
        redLight = redLightObject.GetComponent<Light>();
        light1Component = light1.GetComponent<Light>();
        light2Component = light2.GetComponent<Light>();
        highBeam1Component = highBeam1.GetComponent<Light>();
        highBeam2Component = highBeam2.GetComponent<Light>();

        // Sicherstellen, dass alle Lichter initial ausgeschaltet sind
        bluelight.enabled = false;
        redLight.enabled = false;
        light1Component.enabled = false;
        light2Component.enabled = false;
        highBeam1Component.enabled = false;
        highBeam2Component.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Lichtmodus durchschalten
            lightMode = (lightMode + 1) % 3; // Es gibt 3 Modi (0 bis 2)

            // Alle Lichter ausschalten
            light1Component.enabled = false;
            light2Component.enabled = false;
            highBeam1Component.enabled = false;
            highBeam2Component.enabled = false;

            // Lichtmodus aktivieren
            switch (lightMode)
            {
                case 0: // Alle Lichter aus
                    break;
                case 1: // Licht an
                    light1Component.enabled = true;
                    light2Component.enabled = true;
                    break;
                case 2: // Alles an (Fernlicht und Licht)
                    light1Component.enabled = true;
                    light2Component.enabled = true;
                    highBeam1Component.enabled = true;
                    highBeam2Component.enabled = true;
                    break;
            }
        }

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
