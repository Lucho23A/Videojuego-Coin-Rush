using UnityEngine;
public class GoombaIA : MonoBehaviour
{
    Transform jugador;
    float velocidad = 2f;
    float rangoDeteccion = 6f;
    Vector3 puntoA, puntoB;
    bool haciaPuntoB = true;

    // Gravedad manual para que el Goomba caiga al suelo
    float velocidadY = 0f;
    float gravedad = -20f;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        puntoA = transform.position + Vector3.left * 4f;
        puntoB = transform.position + Vector3.right * 4f;
    }

    void Update()
    {
        if (jugador == null) return;

        // ── Movimiento horizontal ──────────────────────
        Vector3 moverHacia;
        if (Vector3.Distance(transform.position, jugador.position) < rangoDeteccion)
        {
            moverHacia = (jugador.position - transform.position).normalized;
            transform.LookAt(new Vector3(jugador.position.x, transform.position.y, jugador.position.z));
        }
        else
        {
            Vector3 destino = haciaPuntoB ? puntoB : puntoA;
            moverHacia = (destino - transform.position).normalized;
            if (Vector3.Distance(transform.position, destino) < 0.2f)
                haciaPuntoB = !haciaPuntoB;
        }

        // ── Gravedad ───────────────────────────────────
        velocidadY += gravedad * Time.deltaTime;

        // Raycast hacia abajo para detectar suelo
        bool enSuelo = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (enSuelo && velocidadY < 0f) velocidadY = 0f;

        Vector3 movimiento = moverHacia * velocidad * Time.deltaTime;
        movimiento.y = velocidadY * Time.deltaTime;
        transform.position += movimiento;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance?.LoseLife();
    }
}