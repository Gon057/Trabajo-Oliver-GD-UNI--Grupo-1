using UnityEngine;

public class DetectorJugador : MonoBehaviour
{
    private Discos disco;

    private void Awake()
    {
        disco = GetComponentInParent<Discos>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            disco.GolpearPlayer(other.gameObject);
        }
    }
}
