using UnityEngine;

/// <summary>
/// Bloque de hielo que se rompe al pisarlo.
/// El jugador puede pararse en él; tras 1.2 segundos cae y desaparece.
/// </summary>
public class BloqueRompible : MonoBehaviour
{
    private float tiempoHastaRomper = 1.2f;
    private bool  activado          = false;
    private float timer             = 0f;
    private Vector3 posicionOriginal;

    private void Start()
    {
        posicionOriginal = transform.position;
    }

    private void Update()
    {
        if (!activado) return;

        timer += Time.deltaTime;

        // Vibración visual antes de caer
        if (timer < tiempoHastaRomper)
        {
            float temblor = Mathf.Sin(timer * 40f) * 0.04f;
            transform.position = posicionOriginal + new Vector3(temblor, 0f, temblor);
        }
        else
        {
            // Caída y destrucción
            transform.position += Vector3.down * 8f * Time.deltaTime;
            if (transform.position.y < posicionOriginal.y - 10f)
                Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (!activado && col.gameObject.CompareTag("Player"))
            activado = true;
    }
}
