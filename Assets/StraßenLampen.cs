using UnityEngine;

public class StraßenLampen : MonoBehaviour
{
    public GameObject lightPrefab; // Prefab für die Straßenlampe
    private Light lightComponent; // Referenz zur Light-Komponente der Straßenlampe
    public GameObject Sun; // Referenz zur Sonne

    // Start is called before the first Frame-Update
    void Start()
    {
        lightComponent = lightPrefab.GetComponent<Light>();
    }

    // Update wird einmal pro Frame aufgerufen
    void Update()
    {
        // Berechne den Winkel der Sonne zur Weltachse
        float sunAngle = Sun.transform.eulerAngles.x;

        // Wenn die Sonne untergeht (z. B. Winkel zwischen 90° und 360°), Lampen einschalten
        if (sunAngle > 90f && sunAngle < 360f)
        {
            lightComponent.intensity = 3; // Straßenlampe einschalten
        }
        else
        {
            lightComponent.intensity = 0; // Straßenlampe ausschalten
        }
    }
}
