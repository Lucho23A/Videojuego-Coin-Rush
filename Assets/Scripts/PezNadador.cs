using UnityEngine;

public class PezNadador : MonoBehaviour
{
    public float velocidad = 2f;
    public float rangoMovimiento = 10f;
    private Vector3 destino;
    private Vector3 limiteInicial;

    void Start()
    {
        limiteInicial = transform.position;
        ElegirNuevoDestino();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, destino, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino) < 0.5f)
            ElegirNuevoDestino();

        // Rotar hacia donde nada
        if (destino != transform.position)
        {
            Vector3 direccion = (destino - transform.position).normalized;
            if (direccion != Vector3.zero)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direccion),
                    Time.deltaTime * 3f);
        }
    }

    void ElegirNuevoDestino()
    {
        destino = limiteInicial + new Vector3(
            Random.Range(-rangoMovimiento, rangoMovimiento),
            Random.Range(-0.3f, 0.3f),
            Random.Range(-rangoMovimiento, rangoMovimiento));
    }
}