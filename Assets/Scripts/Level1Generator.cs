using UnityEngine;
using UnityEngine.AI;

public class Level1Generator : MonoBehaviour
{
    [Header("Modelos de personajes")]
    public GameObject modeloLaura;
    public GameObject modeloLuis;
    public GameObject modeloMafe;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // ── LUZ ─────────────────────────────────────
        GameObject sun = new GameObject("Sun");
        Light luz = sun.AddComponent<Light>();
        luz.type = LightType.Directional;
        luz.intensity = 1.2f;
        luz.color = new Color(1f, 0.95f, 0.8f);
        sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // ── SUELO ────────────────────────────────────
        GameObject suelo = GameObject.CreatePrimitive(PrimitiveType.Plane);
        suelo.name = "Suelo";
        suelo.transform.localScale = new Vector3(20, 1, 20);
        suelo.transform.position = Vector3.zero;
        Material matSuelo = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matSuelo.color = new Color(0.3f, 0.7f, 0.2f);
        suelo.GetComponent<Renderer>().material = matSuelo;

        // ── AGUA ─────────────────────────────────────
        GameObject agua = GameObject.CreatePrimitive(PrimitiveType.Plane);
        agua.name = "Agua";
        agua.transform.position = new Vector3(60, -0.1f, 0);
        agua.transform.localScale = new Vector3(5, 1, 20);
        Material matAgua = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matAgua.color = new Color(0.1f, 0.4f, 0.9f, 0.7f);
        agua.GetComponent<Renderer>().material = matAgua;

        // ── COLINAS ───────────────────────────────────
        CrearColina(new Vector3(30, 1.5f, 20), new Vector3(8, 3, 8));
        CrearColina(new Vector3(-40, 1f, -30), new Vector3(6, 2, 6));
        CrearColina(new Vector3(70, 2f, -20), new Vector3(10, 4, 10));

        // ── PLATAFORMAS ───────────────────────────────
        CrearPlataforma(new Vector3(20, 2f, 0), new Vector3(4, 0.5f, 4));
        CrearPlataforma(new Vector3(35, 4f, 0), new Vector3(4, 0.5f, 4));
        CrearPlataforma(new Vector3(50, 6f, 0), new Vector3(4, 0.5f, 4));
        CrearPlataforma(new Vector3(65, 4f, 0), new Vector3(4, 0.5f, 4));

        // ── PUENTE ────────────────────────────────────
        CrearPlataforma(new Vector3(58, 0.3f, 0), new Vector3(2, 0.3f, 20));

        // ── ÁRBOLES ───────────────────────────────────
        CrearArbol(new Vector3(-20, 0, 15));
        CrearArbol(new Vector3(-30, 0, -10));
        CrearArbol(new Vector3(10, 0, 30));
        CrearArbol(new Vector3(15, 0, -25));
        CrearArbol(new Vector3(80, 0, 10));

        // ── MONEDAS ───────────────────────────────────
        float[] monedasX = { 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };
        for (int i = 0; i < monedasX.Length; i++)
            CrearMoneda(new Vector3(monedasX[i], 1.5f, Random.Range(-5f, 5f)));

        // ── ESTRELLA FINAL ────────────────────────────
        CrearEstrella(new Vector3(90, 2f, 0));

        // ── ENEMIGOS GOOMBA ───────────────────────────
        CrearGoomba(new Vector3(25, 1f, 5));
        CrearGoomba(new Vector3(45, 1f, -5));
        CrearGoomba(new Vector3(70, 1f, 3));

        // ── JUGADOR ───────────────────────────────────
        CrearJugador(new Vector3(0, 1f, 0));

        // ── SKYBOX ────────────────────────────────────
        RenderSettings.skybox = null;
        Camera.main.backgroundColor = new Color(0.4f, 0.7f, 1f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // ── HUD ───────────────────────────────────────
        GameObject hud = new GameObject("HUDManager");
        hud.AddComponent<HUDManager>();
    }

    void CrearColina(Vector3 pos, Vector3 escala)
    {
        GameObject colina = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        colina.name = "Colina";
        colina.transform.position = pos;
        colina.transform.localScale = escala;
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.25f, 0.6f, 0.15f);
        colina.GetComponent<Renderer>().material = mat;
    }

    void CrearPlataforma(Vector3 pos, Vector3 escala)
    {
        GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plat.name = "Plataforma";
        plat.transform.position = pos;
        plat.transform.localScale = escala;
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.6f, 0.4f, 0.2f);
        plat.GetComponent<Renderer>().material = mat;
    }

    void CrearArbol(Vector3 pos)
    {
        GameObject tronco = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tronco.name = "Arbol";
        tronco.transform.position = pos + Vector3.up * 1.5f;
        tronco.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
        Material matT = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matT.color = new Color(0.4f, 0.25f, 0.1f);
        tronco.GetComponent<Renderer>().material = matT;

        GameObject copa = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        copa.transform.position = pos + Vector3.up * 4f;
        copa.transform.localScale = new Vector3(3f, 3f, 3f);
        Material matC = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matC.color = new Color(0.1f, 0.5f, 0.1f);
        copa.GetComponent<Renderer>().material = matC;
        copa.GetComponent<Collider>().isTrigger = true;
    }

    void CrearMoneda(Vector3 pos)
    {
        GameObject moneda = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        moneda.name = "Moneda";
        moneda.transform.position = pos;
        moneda.transform.localScale = new Vector3(0.5f, 0.05f, 0.5f);
        moneda.transform.rotation = Quaternion.Euler(90, 0, 0);
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(1f, 0.85f, 0f);
        moneda.GetComponent<Renderer>().material = mat;
        moneda.GetComponent<Collider>().isTrigger = true;
        moneda.AddComponent<CollectibleMoneda>();
        moneda.AddComponent<RotarObjeto>();
    }

    void CrearEstrella(Vector3 pos)
    {
        GameObject estrella = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        estrella.name = "EstrellaFinal";
        estrella.transform.position = pos;
        estrella.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(1f, 1f, 0f);
        estrella.GetComponent<Renderer>().material = mat;
        estrella.GetComponent<Collider>().isTrigger = true;
        estrella.AddComponent<EstrellaFinal>();
        estrella.AddComponent<RotarObjeto>();
    }

    void CrearGoomba(Vector3 pos)
    {
        GameObject goomba = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        goomba.name = "Goomba";
        goomba.transform.position = pos;
        goomba.transform.localScale = new Vector3(1f, 1f, 1f);
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.5f, 0.3f, 0.1f);
        goomba.GetComponent<Renderer>().material = mat;
        goomba.GetComponent<Collider>().isTrigger = true;
        goomba.AddComponent<GoombaIA>();
    }

    void CrearJugador(Vector3 pos)
    {
        GameObject jugador = new GameObject("Jugador");
        jugador.transform.position = pos;
        jugador.tag = "Player";

        // Elegir modelo según personaje seleccionado
        GameObject[] modelos = { modeloLaura, modeloLuis, modeloMafe };
        int idx = (GameManager.Instance != null) ? GameManager.Instance.selectedCharacterIndex : 1;
        idx = Mathf.Clamp(idx, 0, modelos.Length - 1);
        GameObject modeloElegido = modelos[idx];

        if (modeloElegido != null)
        {
            GameObject modelo = Instantiate(modeloElegido, jugador.transform);
            modelo.name = "ModeloPersonaje";
            modelo.transform.localPosition = new Vector3(0, -1f, 0);
            modelo.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            modelo.transform.localScale    = new Vector3(1f, 1f, 1f);
            foreach (Collider col in modelo.GetComponentsInChildren<Collider>())
                col.enabled = false;
            foreach (Rigidbody rb in modelo.GetComponentsInChildren<Rigidbody>())
                rb.isKinematic = true;
        }
        else
        {
            // Cápsula de respaldo
            GameObject capsula = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsula.name = "Cuerpo";
            capsula.transform.SetParent(jugador.transform);
            capsula.transform.localPosition = Vector3.zero;
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.2f, 0.5f, 1f);
            capsula.GetComponent<Renderer>().material = mat;
            Destroy(capsula.GetComponent<Collider>());
        }

        // Física
        CharacterController cc = jugador.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.radius = 0.4f;
        cc.center = new Vector3(0f, 1f, 0f);

        jugador.AddComponent<JugadorController>();
        Camera.main.transform.SetParent(null);
        jugador.AddComponent<CamaraFollow>().target = jugador.transform;
    }
}