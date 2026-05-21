using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Genera y actualiza el HUD del nivel en tiempo de ejecución.
/// Se crea solo: agregar este script a un GameObject vacío en la escena.
/// Muestra: vidas, timer, score y monedas. Gestiona pausa y Game Over.
/// </summary>
public class HUDManager : MonoBehaviour
{
    [Header("Configuración del nivel")]
    [SerializeField] private float tiempoInicial = 180f; // 3 minutos para nivel 1
    [SerializeField] private int totalMonedas = 15;

    /// <summary>
    /// Permite que el generador del nivel configure el HUD antes de Start().
    /// Llamar ANTES de que Unity ejecute Start (en el mismo frame de creación).
    /// </summary>
    public void Inicializar(float tiempoInicial, int totalMonedas)
    {
        this.tiempoInicial = tiempoInicial;
        this.totalMonedas  = totalMonedas;
    }

    // ── Referencias UI generadas en BuildHUD() ──────────────────────
    private TextMeshProUGUI textoVidas;
    private TextMeshProUGUI textoTimer;
    private TextMeshProUGUI textoScore;
    private TextMeshProUGUI textoMonedas;
    private GameObject panelPausa;
    private GameObject panelGameOver;

    // ── Estado interno ───────────────────────────────────────────────
    private float tiempoRestante;
    private bool corriendo = true;
    private bool pausado   = false;

    private int vidasAnterior = -1;
    private int scoreAnterior = -1;
    private int monedasAnterior = -1;

    // ────────────────────────────────────────────────────────────────
    void Start()
    {
        tiempoRestante = tiempoInicial;
        BuildHUD();
        ActualizarTodo();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePausa();

        if (!corriendo || pausado) return;

        // ── Timer ────────────────────────────────────────────────────
        tiempoRestante -= Time.deltaTime;
        ActualizarTimer();

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            corriendo = false;
            MostrarGameOver("⏱ ¡TIEMPO AGOTADO!");
        }

        // ── Detectar cambios en GameManager ─────────────────────────
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.currentScore != scoreAnterior)
        {
            scoreAnterior = GameManager.Instance.currentScore;
            textoScore.text = $"⭐ {scoreAnterior}";
        }

        if (GameManager.Instance.coinsCollected != monedasAnterior)
        {
            monedasAnterior = GameManager.Instance.coinsCollected;
            textoMonedas.text = $"🪙 {monedasAnterior}/{totalMonedas}";
        }

        if (GameManager.Instance.currentLives != vidasAnterior)
        {
            vidasAnterior = GameManager.Instance.currentLives;
            ActualizarVidas();

            if (vidasAnterior <= 0)
                MostrarGameOver("💀 ¡GAME OVER!");
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  PÚBLICO — llamado desde fuera
    // ════════════════════════════════════════════════════════════════

    public void PausarTimer()  => corriendo = false;
    public void ReanudarTimer() => corriendo = true;
    public float GetTiempoRestante() => tiempoRestante;

    // ════════════════════════════════════════════════════════════════
    //  CONSTRUCCIÓN DEL HUD
    // ════════════════════════════════════════════════════════════════

    void BuildHUD()
    {
        // ── Canvas principal ─────────────────────────────────────────
        GameObject canvasGO = new GameObject("Canvas_HUD");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem (si no existe) — usa New Input System
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // ── Barra superior (fondo semitransparente) ──────────────────
        GameObject barraTop = CrearPanel(canvasGO, "BarraTop",
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, -50f), new Vector2(0f, 100f),
            new Color(0f, 0f, 0f, 0.5f));

        // ── VIDAS (izquierda) ────────────────────────────────────────
        textoVidas = CrearTexto(barraTop, "TextoVidas",
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
            new Vector2(20f, 0f), new Vector2(300f, 50f),
            "❤️❤️❤️", 36, TextAlignmentOptions.MidlineLeft);

        // ── TIMER (centro) ───────────────────────────────────────────
        textoTimer = CrearTexto(barraTop, "TextoTimer",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, 0f), new Vector2(200f, 50f),
            "03:00", 42, TextAlignmentOptions.Midline);
        textoTimer.fontStyle = FontStyles.Bold;

        // ── SCORE (derecha arriba) ───────────────────────────────────
        textoScore = CrearTexto(barraTop, "TextoScore",
            new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
            new Vector2(-20f, 10f), new Vector2(250f, 30f),
            "⭐ 0", 28, TextAlignmentOptions.MidlineRight);

        // ── MONEDAS (derecha abajo) ──────────────────────────────────
        textoMonedas = CrearTexto(barraTop, "TextoMonedas",
            new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
            new Vector2(-20f, -15f), new Vector2(250f, 30f),
            $"🪙 0/{totalMonedas}", 28, TextAlignmentOptions.MidlineRight);

        // ── PANEL PAUSA ──────────────────────────────────────────────
        panelPausa = BuildPanelPausa(canvasGO);
        panelPausa.SetActive(false);

        // ── PANEL GAME OVER ──────────────────────────────────────────
        panelGameOver = BuildPanelGameOver(canvasGO);
        panelGameOver.SetActive(false);
    }

    // ════════════════════════════════════════════════════════════════
    //  ACTUALIZACIÓN DE UI
    // ════════════════════════════════════════════════════════════════

    void ActualizarTodo()
    {
        ActualizarTimer();
        ActualizarVidas();
        if (GameManager.Instance != null)
        {
            textoScore.text   = $"⭐ {GameManager.Instance.currentScore}";
            textoMonedas.text = $"🪙 {GameManager.Instance.coinsCollected}/{totalMonedas}";
        }
    }

    void ActualizarTimer()
    {
        int min = Mathf.FloorToInt(tiempoRestante / 60f);
        int seg = Mathf.FloorToInt(tiempoRestante % 60f);
        textoTimer.text  = $"{min:00}:{seg:00}";
        textoTimer.color = tiempoRestante <= 30f ? Color.red : Color.white;
    }

    void ActualizarVidas()
    {
        int vidas = GameManager.Instance != null ? GameManager.Instance.currentLives : 3;
        vidas = Mathf.Max(0, vidas);
        textoVidas.text = vidas > 0
            ? new string('❤', vidas).Replace("❤", "❤️")
            : "💀";
    }

    // ════════════════════════════════════════════════════════════════
    //  PAUSA
    // ════════════════════════════════════════════════════════════════

    void TogglePausa()
    {
        pausado = !pausado;
        Time.timeScale = pausado ? 0f : 1f;
        panelPausa.SetActive(pausado);
    }

    // ════════════════════════════════════════════════════════════════
    //  GAME OVER
    // ════════════════════════════════════════════════════════════════

    void MostrarGameOver(string mensaje)
    {
        corriendo = false;
        Time.timeScale = 0f;

        // Actualizar mensaje en el panel
        TextMeshProUGUI[] textos = panelGameOver.GetComponentsInChildren<TextMeshProUGUI>();
        if (textos.Length > 0) textos[0].text = mensaje;

        panelGameOver.SetActive(true);
    }

    // ════════════════════════════════════════════════════════════════
    //  PANELES MODALES
    // ════════════════════════════════════════════════════════════════

    GameObject BuildPanelPausa(GameObject parent)
    {
        GameObject panel = CrearPanel(parent, "Panel_Pausa",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(400f, 350f),
            new Color(0f, 0f, 0f, 0.85f));

        CrearTexto(panel, "TituloPausa",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -40f), new Vector2(350f, 60f),
            "⏸ PAUSA", 48, TextAlignmentOptions.Midline).fontStyle = FontStyles.Bold;

        CrearBoton(panel, "BtnReanudar",
            new Vector2(0f, 80f), new Vector2(320f, 60f),
            "▶  REANUDAR", new Color(0.2f, 0.7f, 0.3f),
            () => TogglePausa());

        CrearBoton(panel, "BtnReintentar",
            new Vector2(0f, 0f), new Vector2(320f, 60f),
            "🔄  REINTENTAR", new Color(0.2f, 0.4f, 0.8f),
            () =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });

        CrearBoton(panel, "BtnMenu",
            new Vector2(0f, -80f), new Vector2(320f, 60f),
            "🏠  MENÚ PRINCIPAL", new Color(0.7f, 0.2f, 0.2f),
            () =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            });

        return panel;
    }

    GameObject BuildPanelGameOver(GameObject parent)
    {
        GameObject panel = CrearPanel(parent, "Panel_GameOver",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(450f, 320f),
            new Color(0f, 0f, 0f, 0.9f));

        CrearTexto(panel, "TituloGameOver",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -50f), new Vector2(400f, 70f),
            "💀 ¡GAME OVER!", 44, TextAlignmentOptions.Midline).fontStyle = FontStyles.Bold;

        CrearBoton(panel, "BtnReintentar",
            new Vector2(0f, 30f), new Vector2(360f, 65f),
            "🔄  REINTENTAR", new Color(0.2f, 0.4f, 0.8f),
            () =>
            {
                Time.timeScale = 1f;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentLives = 3;
                    GameManager.Instance.currentScore = 0;
                    GameManager.Instance.coinsCollected = 0;
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });

        CrearBoton(panel, "BtnMenu",
            new Vector2(0f, -50f), new Vector2(360f, 65f),
            "🏠  MENÚ PRINCIPAL", new Color(0.7f, 0.2f, 0.2f),
            () =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            });

        return panel;
    }

    // ════════════════════════════════════════════════════════════════
    //  HELPERS DE CREACIÓN DE UI
    // ════════════════════════════════════════════════════════════════

    GameObject CrearPanel(GameObject parent, string nombre,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 posicion, Vector2 tamano, Color color)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent.transform, false);

        Image img = go.AddComponent<Image>();
        img.color = color;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamano;

        return go;
    }

    TextMeshProUGUI CrearTexto(GameObject parent, string nombre,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 posicion, Vector2 tamano,
        string contenido, float fontSize,
        TextAlignmentOptions alineacion)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent.transform, false);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = contenido;
        tmp.fontSize  = fontSize;
        tmp.color     = Color.white;
        tmp.alignment = alineacion;

        // Sombra para legibilidad
        go.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.6f);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamano;

        return tmp;
    }

    void CrearBoton(GameObject parent, string nombre,
        Vector2 posicion, Vector2 tamano,
        string label, Color color, UnityEngine.Events.UnityAction accion)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent.transform, false);

        Image img = go.AddComponent<Image>();
        img.color = color;

        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = color * 1.2f;
        cb.pressedColor     = color * 0.7f;
        btn.colors = cb;
        btn.onClick.AddListener(accion);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamano;

        // Texto del botón
        GameObject textGO = new GameObject("Texto");
        textGO.transform.SetParent(go.transform, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 30;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Midline;
        tmp.fontStyle = FontStyles.Bold;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
    }
}
