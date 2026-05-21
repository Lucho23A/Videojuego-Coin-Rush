using UnityEngine;

/// <summary>
/// Generador del Nivel 2 — Montaña Helada.
/// Crea todo el nivel por código: base nevada, plataformas que suben
/// la montaña, lago de hielo (zona peligrosa), bloques rompibles,
/// 20 monedas, 3 gemas azules, 4 Goombas de hielo, 1 Cuervo y la estrella.
/// Tiempo límite: 4 minutos (240 s).
/// </summary>
public class Level2Generator : MonoBehaviour
{
    [Header("Modelo del personaje (arrastra Luis.glb aquí)")]
    [SerializeField] private GameObject modeloPersonaje;

    void Start()
    {
        GenerarNivel();
    }

    void GenerarNivel()
    {
        ConfigurarCielo();
        CrearSuelo();
        CrearLagoHielo();
        CrearMontana();
        CrearPlataformasMoviles();
        CrearBloquesRompibles();
        CrearMonedas();
        CrearGemas();
        CrearEnemigos();
        CrearEstrella(new Vector3(90f, 46f, 0f));
        CrearJugador(new Vector3(0f, 1.5f, 0f));
        CrearHUD();
    }

    // ══════════════════════════════════════════════════════════════
    //  ENTORNO
    // ══════════════════════════════════════════════════════════════

    void ConfigurarCielo()
    {
        RenderSettings.skybox = null;
        Camera.main.backgroundColor = new Color(0.75f, 0.88f, 0.96f); // azul pálido nevado
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // Luz fría, ángulo bajo (invierno)
        GameObject sol = new GameObject("Sol");
        Light luz = sol.AddComponent<Light>();
        luz.type      = LightType.Directional;
        luz.intensity = 0.9f;
        luz.color     = new Color(0.88f, 0.93f, 1f); // blanco azulado
        sol.transform.rotation = Quaternion.Euler(25f, -40f, 0f);

        // Luz de relleno suave (niebla de montaña)
        RenderSettings.ambientLight = new Color(0.55f, 0.65f, 0.75f);
        RenderSettings.fog          = true;
        RenderSettings.fogColor     = new Color(0.78f, 0.88f, 0.95f);
        RenderSettings.fogDensity   = 0.008f;
    }

    void CrearSuelo()
    {
        // Base plana nevada en el inicio
        CrearPlataforma(new Vector3(0f, 0f, 0f), new Vector3(22f, 1f, 22f),
            ColorNieve(), "SueloBase");

        // Suelo general de la montaña (rampa diagonal de plataformas)
        // Las plataformas se crean en CrearMontana()
    }

    void CrearLagoHielo()
    {
        // ── Visual del lago helado ─────────────────────────────────
        GameObject lago = GameObject.CreatePrimitive(PrimitiveType.Plane);
        lago.name = "LagoHielo";
        lago.transform.position   = new Vector3(-20f, -0.1f, 0f);
        lago.transform.localScale = new Vector3(5f, 1f, 5f);
        Material matLago = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matLago.color = new Color(0.5f, 0.78f, 0.95f, 0.85f); // azul hielo semitransparente
        lago.GetComponent<Renderer>().material = matLago;
        lago.GetComponent<Collider>().enabled  = false; // no físico

        // ── Zona de daño ─────────────────────────────────────────
        GameObject zona = new GameObject("ZonaDañoHielo");
        zona.transform.position = new Vector3(-20f, -1.5f, 0f);
        BoxCollider bc = zona.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size      = new Vector3(60f, 1f, 60f);
        ZonaAgua za  = zona.AddComponent<ZonaAgua>();
        za.puntoRespawn = new Vector3(0f, 2f, 0f);
    }

    // ══════════════════════════════════════════════════════════════
    //  MONTAÑA (plataformas en zigzag ascendente)
    // ══════════════════════════════════════════════════════════════

    void CrearMontana()
    {
        // Tramo 1 — base (y: 0 → 10)
        CrearPlataforma(new Vector3(15f,  3f,  4f), new Vector3(8f, 1f, 6f), ColorNieve(), "Plat_1a");
        CrearPlataforma(new Vector3(24f,  6f, -3f), new Vector3(8f, 1f, 6f), ColorNieve(), "Plat_1b");
        CrearPlataforma(new Vector3(34f,  9f,  4f), new Vector3(7f, 1f, 6f), ColorNieve(), "Plat_1c");

        // Tramo 2 — ladera media (y: 10 → 25)
        CrearPlataforma(new Vector3(44f, 13f, -2f), new Vector3(7f, 1f, 6f), ColorNieve(), "Plat_2a");
        CrearPlataforma(new Vector3(53f, 17f,  5f), new Vector3(6f, 1f, 5f), ColorNieve(), "Plat_2b");
        CrearPlataforma(new Vector3(61f, 21f, -1f), new Vector3(6f, 1f, 5f), ColorNieve(), "Plat_2c");
        CrearPlataforma(new Vector3(69f, 25f,  3f), new Vector3(6f, 1f, 5f), ColorNieve(), "Plat_2d");

        // Tramo 3 — cima (y: 25 → 45)
        CrearPlataforma(new Vector3(75f, 30f, -2f), new Vector3(5f, 1f, 5f), ColorHielo(), "Plat_3a");
        CrearPlataforma(new Vector3(80f, 35f,  3f), new Vector3(5f, 1f, 5f), ColorHielo(), "Plat_3b");
        CrearPlataforma(new Vector3(85f, 40f, -1f), new Vector3(5f, 1f, 5f), ColorHielo(), "Plat_3c");

        // Cima final (plataforma grande con la estrella)
        CrearPlataforma(new Vector3(90f, 45f,  0f), new Vector3(12f, 1f, 12f), ColorHielo(), "Cima");

        // Decoración: picos de montaña (conos simulados con esferas)
        CrearPicoNieve(new Vector3(20f, 12f,  15f));
        CrearPicoNieve(new Vector3(50f, 22f, -12f));
        CrearPicoNieve(new Vector3(78f, 35f,  12f));
    }

    // ══════════════════════════════════════════════════════════════
    //  PLATAFORMAS MÓVILES (nieve flotante entre saltos difíciles)
    // ══════════════════════════════════════════════════════════════

    void CrearPlataformasMoviles()
    {
        // Entre tramo 1 y 2
        CrearPlatMovil(
            new Vector3(40f, 11f,  2f),
            new Vector3(40f, 11f, -4f), vel: 1.0f);

        // Entre tramo 2 y 3
        CrearPlatMovil(
            new Vector3(72f, 27f,  5f),
            new Vector3(72f, 27f, -3f), vel: 1.3f);

        // Acceso a la cima
        CrearPlatMovil(
            new Vector3(87f, 42f, -3f),
            new Vector3(87f, 42f,  4f), vel: 0.9f);
    }

    void CrearPlatMovil(Vector3 a, Vector3 b, float vel)
    {
        GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plat.name = "PlatMovilNieve";
        plat.transform.position   = a;
        plat.transform.localScale = new Vector3(4f, 0.6f, 4f);

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.92f, 0.96f, 1f);
        plat.GetComponent<Renderer>().material = mat;

        PlataformaMovil pm = plat.AddComponent<PlataformaMovil>();
        pm.puntoA    = a;
        pm.puntoB    = b;
        pm.velocidad = vel;
    }

    // ══════════════════════════════════════════════════════════════
    //  BLOQUES ROMPIBLES DE HIELO
    // ══════════════════════════════════════════════════════════════

    void CrearBloquesRompibles()
    {
        // Grupo de bloques en el tramo medio — el jugador debe pisarlos rápido
        Vector3[] posBlocks =
        {
            new Vector3(47f, 14.5f, -2f),
            new Vector3(51f, 14.5f, -2f),
            new Vector3(55f, 18.5f,  5f),
            new Vector3(59f, 22.5f, -1f),
        };

        foreach (var pos in posBlocks)
        {
            GameObject bloque = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bloque.name = "BloqueHielo";
            bloque.transform.position   = pos;
            bloque.transform.localScale = new Vector3(3f, 0.5f, 3f);

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.6f, 0.85f, 0.98f, 0.9f); // azul hielo claro
            bloque.GetComponent<Renderer>().material = mat;

            bloque.AddComponent<BloqueRompible>();
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  COLECCIONABLES
    // ══════════════════════════════════════════════════════════════

    void CrearMonedas()
    {
        // 20 monedas a lo largo del camino ascendente
        Vector3[] pos =
        {
            // Base
            new Vector3( 3f, 1.5f,  3f),
            new Vector3(-3f, 1.5f, -3f),
            new Vector3( 6f, 1.5f, -4f),
            // Tramo 1
            new Vector3(15f,  4.5f,  4f),
            new Vector3(24f,  7.5f, -3f),
            new Vector3(34f, 10.5f,  4f),
            new Vector3(34f, 10.5f,  0f),
            // Tramo 2
            new Vector3(44f, 14.5f, -2f),
            new Vector3(53f, 18.5f,  5f),
            new Vector3(61f, 22.5f, -1f),
            new Vector3(61f, 22.5f,  3f),
            new Vector3(69f, 26.5f,  3f),
            new Vector3(69f, 26.5f, -1f),
            // Tramo 3
            new Vector3(75f, 31.5f, -2f),
            new Vector3(80f, 36.5f,  3f),
            new Vector3(80f, 36.5f, -1f),
            new Vector3(85f, 41.5f, -1f),
            // Cima
            new Vector3(88f, 46.5f, -3f),
            new Vector3(91f, 46.5f,  3f),
            new Vector3(90f, 46.5f,  0f),
        };

        foreach (var p in pos)
            CrearMoneda(p);
    }

    void CrearMoneda(Vector3 pos)
    {
        GameObject moneda = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        moneda.name = "Moneda";
        moneda.transform.position   = pos;
        moneda.transform.localScale = new Vector3(0.5f, 0.05f, 0.5f);
        moneda.transform.rotation   = Quaternion.Euler(90f, 0f, 0f);

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(1f, 0.85f, 0f);
        moneda.GetComponent<Renderer>().material = mat;
        moneda.GetComponent<Collider>().isTrigger = true;
        moneda.AddComponent<CollectibleMoneda>();
        moneda.AddComponent<RotarObjeto>();
    }

    void CrearGemas()
    {
        // 3 gemas azules en puntos difíciles (fuera del camino principal)
        CrearGema(new Vector3(20f,  5f, -9f));   // salto lateral en tramo 1
        CrearGema(new Vector3(65f, 23f,  9f));   // borde de plataforma tramo 2
        CrearGema(new Vector3(86f, 42f, -8f));   // salto arriesgado cerca de la cima
    }

    void CrearGema(Vector3 pos)
    {
        // Forma: cubo rotado 45° → parece un diamante
        GameObject gema = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gema.name = "GemaAzul";
        gema.transform.position   = pos;
        gema.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        gema.transform.rotation   = Quaternion.Euler(45f, 45f, 0f);

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.1f, 0.5f, 1f);
        gema.GetComponent<Renderer>().material = mat;
        gema.GetComponent<Collider>().isTrigger = true;
        gema.AddComponent<GemaAzul>();
        gema.AddComponent<RotarObjeto>();
    }

    // ══════════════════════════════════════════════════════════════
    //  ENEMIGOS
    // ══════════════════════════════════════════════════════════════

    void CrearEnemigos()
    {
        // 4 Goombas de hielo (reutiliza GoombaIA con color diferente)
        CrearGoombaHielo(new Vector3(16f,  4.5f,  4f));
        CrearGoombaHielo(new Vector3(35f, 10.5f,  1f));
        CrearGoombaHielo(new Vector3(54f, 18.5f,  3f));
        CrearGoombaHielo(new Vector3(70f, 26.5f, -1f));

        // 1 Cuervo volador — patrulla la zona media-alta
        CrearCuervo(new Vector3(60f, 30f, 0f));
    }

    void CrearGoombaHielo(Vector3 pos)
    {
        GameObject goomba = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        goomba.name = "GoombaHielo";
        goomba.transform.position   = pos;
        goomba.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

        // Color blanco-azulado (versión de hielo)
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.7f, 0.88f, 1f);
        goomba.GetComponent<Renderer>().material = mat;
        goomba.GetComponent<Collider>().isTrigger = true;
        goomba.AddComponent<GoombaIA>(); // reutiliza la misma IA
    }

    void CrearCuervo(Vector3 pos)
    {
        // Cuerpo: esfera oscura achatada
        GameObject cuervo = new GameObject("Cuervo");
        cuervo.transform.position = pos;

        GameObject cuerpo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cuerpo.transform.SetParent(cuervo.transform);
        cuerpo.transform.localPosition = Vector3.zero;
        cuerpo.transform.localScale    = new Vector3(1.2f, 0.5f, 1.8f);
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.1f, 0.1f, 0.12f); // negro azabache
        cuerpo.GetComponent<Renderer>().material = mat;
        cuerpo.GetComponent<Collider>().isTrigger = true;

        // Alas (dos cubos a los lados)
        CrearAla(cuervo.transform,  new Vector3( 1.2f, 0f, 0f));
        CrearAla(cuervo.transform, new Vector3(-1.2f, 0f, 0f));

        cuervo.AddComponent<CuervoIA>();
    }

    void CrearAla(Transform padre, Vector3 offset)
    {
        GameObject ala = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ala.name = "Ala";
        ala.transform.SetParent(padre);
        ala.transform.localPosition = offset;
        ala.transform.localScale    = new Vector3(1f, 0.1f, 0.7f);
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.08f, 0.08f, 0.1f);
        ala.GetComponent<Renderer>().material = mat;
        Destroy(ala.GetComponent<Collider>());
    }

    // ══════════════════════════════════════════════════════════════
    //  ESTRELLA FINAL
    // ══════════════════════════════════════════════════════════════

    void CrearEstrella(Vector3 pos)
    {
        GameObject estrella = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        estrella.name = "EstrellaFinal";
        estrella.transform.position   = pos;
        estrella.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(1f, 1f, 0.1f);
        estrella.GetComponent<Renderer>().material = mat;
        estrella.GetComponent<Collider>().isTrigger = true;
        estrella.AddComponent<EstrellaFinal>();
        estrella.AddComponent<RotarObjeto>();
    }

    // ══════════════════════════════════════════════════════════════
    //  JUGADOR Y CÁMARA
    // ══════════════════════════════════════════════════════════════

    void CrearJugador(Vector3 pos)
    {
        GameObject jugador = new GameObject("Jugador");
        jugador.transform.position = pos;
        jugador.tag = "Player";

        if (modeloPersonaje != null)
        {
            GameObject modelo = Instantiate(modeloPersonaje, jugador.transform);
            modelo.name = "ModeloPersonaje";
            modelo.transform.localPosition = Vector3.zero;
            modelo.transform.localRotation = Quaternion.identity;
            foreach (Collider col in modelo.GetComponentsInChildren<Collider>())
                col.enabled = false;
            foreach (Rigidbody rb in modelo.GetComponentsInChildren<Rigidbody>())
                rb.isKinematic = true;
        }
        else
        {
            GameObject capsula = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsula.name = "Cuerpo";
            capsula.transform.SetParent(jugador.transform);
            capsula.transform.localPosition = Vector3.zero;
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.2f, 0.5f, 1f);
            capsula.GetComponent<Renderer>().material = mat;
            Destroy(capsula.GetComponent<Collider>());
        }

        CharacterController cc = jugador.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.radius = 0.4f;
        cc.center = new Vector3(0f, 1f, 0f);

        jugador.AddComponent<JugadorController>();
        Camera.main.transform.SetParent(null);
        jugador.AddComponent<CamaraFollow>().target = jugador.transform;
    }

    // ══════════════════════════════════════════════════════════════
    //  HUD
    // ══════════════════════════════════════════════════════════════

    void CrearHUD()
    {
        GameObject hudGO = new GameObject("HUDManager");
        HUDManager hud   = hudGO.AddComponent<HUDManager>();
        hud.Inicializar(tiempoInicial: 240f, totalMonedas: 20);
    }

    // ══════════════════════════════════════════════════════════════
    //  HELPERS DE CONSTRUCCIÓN
    // ══════════════════════════════════════════════════════════════

    void CrearPlataforma(Vector3 pos, Vector3 escala, Color color, string nombre)
    {
        GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plat.name = nombre;
        plat.transform.position   = pos;
        plat.transform.localScale = escala;
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        plat.GetComponent<Renderer>().material = mat;
    }

    void CrearPicoNieve(Vector3 pos)
    {
        // Simula un pico de montaña con una esfera alargada
        GameObject pico = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pico.name = "PicoNieve";
        pico.transform.position   = pos;
        pico.transform.localScale = new Vector3(4f, 8f, 4f);
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.95f, 0.97f, 1f);
        pico.GetComponent<Renderer>().material = mat;
        pico.GetComponent<Collider>().isTrigger = true; // solo decorativo
    }

    // ── Colores reutilizables ────────────────────────────────────
    Color ColorNieve() => new Color(0.93f, 0.96f, 1f);
    Color ColorHielo() => new Color(0.7f,  0.88f, 0.98f);
}
