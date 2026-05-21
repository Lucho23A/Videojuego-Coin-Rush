using UnityEngine;

/// <summary>
/// Zona de daño del agua. Cuando el jugador cae al agua pierde una vida
/// y reaparece en el último checkpoint (o en el inicio si no hay ninguno).
/// </summary>
public class ZonaAgua : MonoBehaviour
{
    [HideInInspector] public Vector3 puntoRespawn = new Vector3(0f, 2f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Reposicionar al jugador antes de quitar la vida
        // (evita que OnTriggerEnter se dispare varias veces seguidas)
        other.transform.position = puntoRespawn;

        if (GameManager.Instance != null)
            GameManager.Instance.LoseLife();
    }
}
