using UnityEngine;

public class LakeLevel : MonoBehaviour
{
    [Header("Prefabs necesarios")]
    public GameObject prefabPlataforma;
    public GameObject prefabPlataformaMovil;
    public GameObject prefabAgua;
    public GameObject prefabJugador;
    public GameObject prefabMeta;
    public GameObject prefabPez;

    [Header("Configuración")]
    public float velocidadPlataformas = 1.2f;

    // Material transparente para el agua
    private Material materialAgua;

    void Start()
    {
        GenerarNivel();
    }

    void GenerarNivel()
    {
        CrearDecoracion();
        CrearAgua();
        CrearPlataformas();
        CrearPeces();
        CrearMeta();
        CrearJugador();
    }

    // ── DECORACIÓN ────────────────────────────────────────────────
    void CrearDecoracion()
    {
        Camera.main.backgroundColor = new Color(0.2f, 0.5f, 0.8f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.4f, 0.65f, 0.9f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 40f;
        RenderSettings.fogEndDistance = 120f;
        RenderSettings.ambientLight = new Color(0.35f, 0.55f, 0.75f);

        Light[] luces = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light luz in luces)
        {
            if (luz.type == LightType.Directional)
            {
                luz.color = new Color(0.9f, 0.95f, 1f);
                luz.intensity = 0.8f;
                luz.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            }
        }
    }

    // ── AGUA TRANSPARENTE ─────────────────────────────────────────
    void CrearAgua()
    {
        GameObject agua = Instantiate(prefabAgua,
            new Vector3(25f, -0.5f, 0f), Quaternion.identity);
        agua.transform.localScale = new Vector3(70f, 0.2f, 20f);

        Renderer r = agua.GetComponent<Renderer>();
        if (r != null)
            r.material.color = new Color(0.0f, 0.4f, 0.8f);

        ZonaAgua zonaAgua = agua.GetComponent<ZonaAgua>();
        if (zonaAgua != null)
            zonaAgua.puntoRespawn = new Vector3(0f, 3f, 0f);
    }

    // ── PLATAFORMAS ───────────────────────────────────────────────
    void CrearPlataformas()
    {
        Color verde = new Color(0.2f, 0.55f, 0.25f);
        Color azul = new Color(0.15f, 0.45f, 0.6f);

        // Inicio grande y seguro
        CrearPlataFija(new Vector3(0f, 0f, 0f), new Vector3(8f, 0.5f, 5f), verde);

        // Zona 1 — plataformas cercanas fáciles
        CrearPlataFija(new Vector3(10f, 0f, 0f), new Vector3(4f, 0.5f, 4f), verde);
        CrearPlataFija(new Vector3(16f, 0f, 0f), new Vector3(4f, 0.5f, 4f), verde);
        CrearPlataFija(new Vector3(22f, 0f, 0f), new Vector3(4f, 0.5f, 4f), verde);

        // Zona 2 — una móvil fácil
        CrearPlataMovil(
            new Vector3(28f, 0f, -2f), new Vector3(28f, 0f, 2f),
            new Vector3(3f, 0.5f, 3f), velocidadPlataformas, azul);
        CrearPlataFija(new Vector3(34f, 0f, 0f), new Vector3(4f, 0.5f, 4f), verde);

        // Zona 3 — dos móviles medianas
        CrearPlataMovil(
            new Vector3(40f, 0f, -2f), new Vector3(40f, 0f, 2f),
            new Vector3(2.5f, 0.5f, 2.5f), velocidadPlataformas * 1.3f, azul);
        CrearPlataMovil(
            new Vector3(46f, 0f, -2f), new Vector3(46f, 0f, 2f),
            new Vector3(2.5f, 0.5f, 2.5f), velocidadPlataformas * 1.5f, azul);

        // Plataforma final con meta
        CrearPlataFija(new Vector3(53f, 0f, 0f), new Vector3(8f, 0.5f, 5f), verde);
    }

    // ── PECES VISIBLES BAJO EL AGUA ───────────────────────────────
    void CrearPeces()
    {
        if (prefabPez == null) return;

        Color[] colores = {
        new Color(1f, 0.5f, 0f),
        new Color(1f, 0.8f, 0f),
        new Color(0f, 0.8f, 0.8f),
        new Color(1f, 0.2f, 0.2f),
        new Color(0.5f, 0f, 0.8f),
        new Color(0f, 0.8f, 0.3f),
    };

        for (int i = 0; i < 50; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-5f, 65f),
                Random.Range(0.3f, 0.8f), // ENCIMA del agua para que se vean
                Random.Range(-8f, 8f));

            GameObject pez = Instantiate(prefabPez, pos, Quaternion.identity);

            float tamano = Random.Range(0.3f, 0.8f);
            pez.transform.localScale = new Vector3(
                tamano * 0.4f, tamano * 0.2f, tamano);

            Renderer r = pez.GetComponent<Renderer>();
            if (r != null)
                r.material.color = colores[Random.Range(0, colores.Length)];

            PezNadador nadador = pez.GetComponent<PezNadador>();
            if (nadador != null)
            {
                nadador.velocidad = Random.Range(1f, 4f);
                nadador.rangoMovimiento = Random.Range(6f, 15f);
            }
        }
    }
    // ── META ──────────────────────────────────────────────────────
    void CrearMeta()
    {
        GameObject meta = Instantiate(prefabMeta,
            new Vector3(53f, 2f, 0f), Quaternion.identity);

        Renderer r = meta.GetComponent<Renderer>();
        if (r != null)
            r.material.color = new Color(1f, 0.85f, 0f);
    }

    // ── JUGADOR ───────────────────────────────────────────────────
    void CrearJugador()
    {
        Instantiate(prefabJugador, new Vector3(0f, 3f, 0f), Quaternion.identity);
    }

    // ── HELPERS ───────────────────────────────────────────────────
    void CrearPlataFija(Vector3 posicion, Vector3 escala, Color color)
    {
        GameObject p = Instantiate(prefabPlataforma, posicion, Quaternion.identity);
        p.transform.localScale = escala;
        Renderer r = p.GetComponent<Renderer>();
        if (r != null) r.material.color = color;
    }

    void CrearPlataMovil(Vector3 puntoA, Vector3 puntoB,
                         Vector3 escala, float velocidad, Color color)
    {
        Vector3 centro = (puntoA + puntoB) / 2f;
        GameObject p = Instantiate(prefabPlataformaMovil, centro, Quaternion.identity);
        p.transform.localScale = escala;

        Renderer r = p.GetComponent<Renderer>();
        if (r != null) r.material.color = color;

        PlataformaMovil pm = p.GetComponent<PlataformaMovil>();
        if (pm != null)
        {
            pm.puntoA = puntoA;
            pm.puntoB = puntoB;
            pm.velocidad = velocidad;
        }
    }
}