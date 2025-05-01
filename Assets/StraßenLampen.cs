using UnityEngine;

public class StraßenLampen : MonoBehaviour
{
    public GameObject lightPrefab; // Prefab für die Straßenlampe
    private Light lightComponent; // Referenz zur Light-Komponente der Straßenlampe
    public GameObject Sun; // Referenz zur Sonne

    // Start is called before the first frame update
    void Start()
    {
        lightComponent = lightPrefab.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        // Berechne den Winkel der Sonne zur Weltachse
        float sunAngle = Sun.transform.eulerAngles.x;

        // Wenn die Sonne untergeht (z. B. Winkel zwischen 180° und 360°), Lampen einschalten
        if (sunAngle > 180f && sunAngle < 360f)
        {
            lightComponent.intensity = 3; // Straßenlampe einschalten
        }
        else
        {
            lightComponent.intensity = 0; // Straßenlampe ausschalten
        }
    }
}
