using UnityEngine;
public class CollectibleMoneda : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance?.CollectCoin();
            Destroy(gameObject);
        }
    }
}