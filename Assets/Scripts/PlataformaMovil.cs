using UnityEngine;

/// <summary>
/// Plataforma que oscila entre dos puntos (hoja flotante del lago).
/// No requiere configuración manual: los puntos se calculan al crear.
/// </summary>
public class PlataformaMovil : MonoBehaviour
{
    [HideInInspector] public Vector3 puntoA;
    [HideInInspector] public Vector3 puntoB;
    [HideInInspector] public float velocidad = 1f;

    // Offset de fase para que no todas se muevan igual
    private float faseInicial;

    private void Start()
    {
        faseInicial = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        // Seno entre 0 y 1 → movimiento suave de ida y vuelta
        float t = (Mathf.Sin(Time.time * velocidad + faseInicial) + 1f) / 2f;
        transform.position = Vector3.Lerp(puntoA, puntoB, t);
    }
}
