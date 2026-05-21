using UnityEngine;
public class GoombaIA : MonoBehaviour
{
    Transform jugador;
    float velocidad = 2f;
    float rangoDeteccion = 6f;
    Vector3 puntoA, puntoB;
    bool haciaPuntoB = true;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        puntoA = transform.position + Vector3.left * 4f;
        puntoB = transform.position + Vector3.right * 4f;
    }

    void Update()
    {
        if (jugador == null) return;
        if (Vector3.Distance(transform.position, jugador.position) < rangoDeteccion)
        {
            Vector3 dir = (jugador.position - transform.position).normalized;
            transform.position += dir * velocidad * Time.deltaTime;
            transform.LookAt(jugador);
        }
        else
        {
            Vector3 destino = haciaPuntoB ? puntoB : puntoA;
            transform.position = Vector3.MoveTowards(
                transform.position, destino, velocidad * Time.deltaTime);
            if (Vector3.Distance(transform.position, destino) < 0.2f)
                haciaPuntoB = !haciaPuntoB;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance?.LoseLife();
    }
}