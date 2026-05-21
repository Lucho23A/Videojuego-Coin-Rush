using UnityEngine;

/// <summary>
/// Gema azul: coleccionable especial del nivel 2 que otorga 50 puntos.
/// </summary>
public class GemaAzul : MonoBehaviour
{
    private Vector3 posInicial;

    private void Start()
    {
        posInicial = transform.position;
    }

    private void Update()
    {
        // Flotación + rotación
        transform.position = posInicial + Vector3.up * Mathf.Sin(Time.time * 2f) * 0.2f;
        transform.Rotate(Vector3.up, 80f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance?.AddScore(50);
        Destroy(gameObject);
    }
}
