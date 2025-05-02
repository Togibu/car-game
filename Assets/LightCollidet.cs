using UnityEngine;

public class LightCollidet : MonoBehaviour
{
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Verhindert Bewegung durch Physik
    }

    

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = false; // Aktiviert Physik, wenn der Spieler die Laterne berührt

        
        }
    }
}