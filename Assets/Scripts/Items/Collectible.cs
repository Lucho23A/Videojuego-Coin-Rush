using UnityEngine;

/// <summary>
/// Objeto recolectable. Se asigna a monedas, gemas, estrellas, etc.
/// </summary>
public class Collectible : MonoBehaviour
{
    public enum CollectibleType { Coin, Gem, Star, ExtraLife }

    [Header("Configuración")]
    [SerializeField] private CollectibleType type = CollectibleType.Coin;
    [SerializeField] private int scoreValue = 10;

    [Header("FX (opcional)")]
    [SerializeField] private GameObject pickupVFXPrefab;
    [SerializeField] private AudioClip pickupSFX;

    [Header("Animación")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobHeight = 0.2f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Animación: rotar y flotar
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Collect();
    }

    private void Collect()
    {
        if (GameManager.Instance != null)
        {
            if (type == CollectibleType.Coin)
                GameManager.Instance.CollectCoin();
            else
                GameManager.Instance.AddScore(scoreValue);
        }

        if (pickupVFXPrefab != null)
            Instantiate(pickupVFXPrefab, transform.position, Quaternion.identity);

        if (AudioManager.Instance != null && pickupSFX != null)
            AudioManager.Instance.PlaySFX(pickupSFX);

        Destroy(gameObject);
    }
}
