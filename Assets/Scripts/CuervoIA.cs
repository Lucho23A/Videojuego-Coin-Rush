using UnityEngine;

/// <summary>
/// IA del Cuervo volador (Nivel 2).
/// Patrulla en círculo a una altura fija y cuando detecta al jugador
/// baja en picado. Tras el ataque sube de nuevo a su altura de vuelo.
/// </summary>
public class CuervoIA : MonoBehaviour
{
    // ── Parámetros de vuelo ──────────────────────────────────────
    private float alturaVuelo    = 12f;  // altura a la que patrulla
    private float radioPatrulla  = 14f;  // radio del círculo de patrulla
    private float velocidadVuelo = 5f;
    private float rangoDeteccion = 15f;

    // ── Parámetros del picado ────────────────────────────────────
    private float velocidadPicado = 10f;
    private float distanciaAtaque = 1.8f;  // distancia para aplicar daño
    private float cooldownAtaque  = 3f;

    // ── Estado ───────────────────────────────────────────────────
    private enum EstadoCuervo { Patrulla, Picado, Subiendo }
    private EstadoCuervo estado = EstadoCuervo.Patrulla;

    private Transform jugador;
    private Vector3   centroPatrulla;
    private float     anguloActual = 0f;
    private float     timerCooldown = 0f;
    private Vector3   posicionSubida; // dónde subirá al terminar el picado

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jugador = p.transform;

        centroPatrulla = new Vector3(
            transform.position.x,
            alturaVuelo,
            transform.position.z);
    }

    private void Update()
    {
        if (jugador == null) return;

        timerCooldown -= Time.deltaTime;

        switch (estado)
        {
            case EstadoCuervo.Patrulla: Patrullar(); break;
            case EstadoCuervo.Picado:   Picar();     break;
            case EstadoCuervo.Subiendo: Subir();     break;
        }
    }

    // ── Patrullaje circular ──────────────────────────────────────
    private void Patrullar()
    {
        anguloActual += velocidadVuelo * Time.deltaTime;

        Vector3 objetivo = centroPatrulla + new Vector3(
            Mathf.Cos(anguloActual) * radioPatrulla,
            0f,
            Mathf.Sin(anguloActual) * radioPatrulla);

        transform.position = Vector3.MoveTowards(
            transform.position, objetivo, velocidadVuelo * Time.deltaTime);
        transform.LookAt(objetivo);

        // ¿Detecta al jugador?
        float dist = Vector3.Distance(transform.position, jugador.position);
        if (dist <= rangoDeteccion && timerCooldown <= 0f)
            estado = EstadoCuervo.Picado;
    }

    // ── Picado en línea recta hacia el jugador ───────────────────
    private void Picar()
    {
        Vector3 dir = (jugador.position - transform.position).normalized;
        transform.position += dir * velocidadPicado * Time.deltaTime;
        transform.LookAt(jugador.position);

        float dist = Vector3.Distance(transform.position, jugador.position);

        // Daño al jugador
        if (dist <= distanciaAtaque)
        {
            GameManager.Instance?.LoseLife();
            timerCooldown = cooldownAtaque;
            posicionSubida = centroPatrulla + new Vector3(
                Mathf.Cos(anguloActual) * radioPatrulla, 0f,
                Mathf.Sin(anguloActual) * radioPatrulla);
            estado = EstadoCuervo.Subiendo;
        }

        // Si se aleja demasiado (jugador se movió), vuelve a patrullar
        if (dist > rangoDeteccion * 1.5f)
            estado = EstadoCuervo.Patrulla;
    }

    // ── Sube de nuevo a la altura de patrulla ────────────────────
    private void Subir()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, posicionSubida, velocidadVuelo * Time.deltaTime);

        if (Vector3.Distance(transform.position, posicionSubida) < 0.5f)
            estado = EstadoCuervo.Patrulla;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}
