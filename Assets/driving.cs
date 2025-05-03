using UnityEngine;

public class driving : MonoBehaviour
{
    public Transform[] targets; // Array von Zielen, die das Auto anfahren soll
    public float speed = 5f; // Geschwindigkeit des Autos
    public float rotationSpeed = 2f; // Rotationsgeschwindigkeit
    private int currentTargetIndex = 0; // Index des aktuellen Ziels

    void Update()
    {
        if (targets.Length == 0) return; // Falls keine Ziele gesetzt sind, nichts tun

        Transform target = targets[currentTargetIndex]; // Aktuelles Ziel
        Vector3 direction = (target.position - transform.position).normalized; // Richtung zum Ziel

        // Drehe das Auto in Richtung des Ziels
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Bewege das Auto nach vorne
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Überprüfen, ob das Auto das Ziel erreicht hat
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % targets.Length; // Zum nächsten Ziel wechseln
        }
    }
}
