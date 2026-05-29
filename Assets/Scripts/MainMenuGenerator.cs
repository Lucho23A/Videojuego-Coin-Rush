using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuGenerator : MonoBehaviour
{
    [Header("Arrastra tus sprites aquí")]
    public Sprite fondoSprite;
    public Sprite logoSprite;
    public Sprite logoUniversidad;

    [Header("Música de fondo")]
    public AudioClip musicaMenu;

    [Header("Nombre de tu escena de juego")]
    public string gameSceneName = "LevelSelect";

    GameObject panelInstrucciones;
    GameObject panelAgradecimientos;

    void Start()
    {
        BuildMenu();
        if (musicaMenu != null) PlayMusic();
    }

    void PlayMusic()
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip   = musicaMenu;
        audio.loop   = true;
        audio.volume = 0.5f;
        audio.Play();
    }

    void BuildMenu()
    {
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // FONDO
        GameObject fondo = CreateImage(canvasGO, "Fondo", fondoSprite);
        StretchFull(fondo);

        // ESTRELLAS
        for (int i = 0; i < 40; i++) CreateStar(canvasGO, i);

        // MONEDAS
        for (int i = 0; i < 15; i++) CreateCoin(canvasGO, i);

        // LOGO UNIVERSIDAD arriba izquierda
        if (logoUniversidad != null)
        {
            GameObject logoUni = CreateImage(canvasGO, "LogoUniversidad", logoUniversidad);
            RectTransform logoUniRT = logoUni.GetComponent<RectTransform>();
            logoUniRT.anchorMin = new Vector2(0, 1);
            logoUniRT.anchorMax = new Vector2(0, 1);
            logoUniRT.pivot     = new Vector2(0, 1);
            logoUniRT.sizeDelta = new Vector2(280, 90);
            logoUniRT.anchoredPosition = new Vector2(20, -20);
        }

        // LOGO JUEGO
        GameObject logo = CreateImage(canvasGO, "Logo", logoSprite);
        RectTransform logoRT = logo.GetComponent<RectTransform>();
        logoRT.sizeDelta        = new Vector2(700, 280);
        logoRT.anchoredPosition = new Vector2(0, 180);
        logo.AddComponent<LogoBounce>();

        // BOTONES PRINCIPALES
        string[] textos = { "JUGAR", "INSTRUCCIONES", "AGRADECIMIENTOS", "PUNTUACIONES" };
        Color[]  colores = {
            HexColor("#FFB300"),
            HexColor("#0288D1"),
            HexColor("#7B1FA2"),
            HexColor("#388E3C")
        };
        float[] posY = { 20f, -70f, -160f, -250f };

        for (int i = 0; i < textos.Length; i++)
        {
            string texto = textos[i];
            GameObject btn = CreateButton(canvasGO, textos[i], colores[i],
                             new Vector2(0, posY[i]), new Vector2(420, 75));
            btn.AddComponent<ButtonGlow>();
            btn.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(texto));
        }

        // PANEL INSTRUCCIONES
        panelInstrucciones = CrearPanel(canvasGO, "PanelInstrucciones",
            "📋 INSTRUCCIONES",
            "🎮 CONTROLES:\n" +
            "• W / A / S / D — Mover al personaje\n" +
            "• Mouse — Controlar la cámara\n" +
            "• ESPACIO — Saltar\n" +
            "• SHIFT — Correr\n\n" +
            "🎯 OBJETIVO:\n" +
            "• Recolecta todas las monedas del nivel\n" +
            "• Evita a los enemigos o perderás vidas\n" +
            "• Llega a la estrella dorada para completar el nivel\n" +
            "• ¡Completa los 3 niveles y logra el mejor puntaje!\n\n" +
            "❤️ VIDAS: Tienes 3 vidas por partida\n" +
            "⏱️ TIEMPO: Cada nivel tiene un límite de tiempo",
            HexColor("#0288D1"));
        panelInstrucciones.SetActive(false);

        // PANEL AGRADECIMIENTOS
        panelAgradecimientos = CrearPanel(canvasGO, "PanelAgradecimientos",
            "🙏 AGRADECIMIENTOS",
            "Este videojuego fue desarrollado con dedicación\n" +
            "como proyecto académico de la\n\n" +
            "🎓 UNIVERSIDAD CENTRAL\n\n" +
            "👨‍🏫 DOCENTE:\n" +
            "Carlos Iván Pinzón\n\n" +
            "👾 DESARROLLADO POR:\n" +
            "• Maria Fernanda Ceballos Otero\n" +
            "• Laura Vanessa Gutiérrez Guzmán\n" +
            "• Luis Alberto Diuche Peña\n\n" +
            "🛠️ HERRAMIENTAS UTILIZADAS:\n" +
            "Unity 6 • C# • Avaturn • Mixamo\n" +
            "TextMeshPro • Starter Assets • ProBuilder\n\n" +
            "¡Gracias por jugar Coin Rush! 🪙⭐",
            HexColor("#7B1FA2"));
        panelAgradecimientos.SetActive(false);

        // EventSystem
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    GameObject CrearPanel(GameObject parent, string nombre, string titulo, string contenido, Color color)
    {
        // FONDO OSCURO
        GameObject overlay = new GameObject(nombre + "_Overlay");
        overlay.transform.SetParent(parent.transform, false);
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 0.85f);
        RectTransform overlayRT = overlay.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.sizeDelta = Vector2.zero;
        Button overlayBtn = overlay.AddComponent<Button>();
        overlayBtn.onClick.AddListener(() => overlay.SetActive(false));

        // PANEL
        GameObject panel = new GameObject(nombre);
        panel.transform.SetParent(overlay.transform, false);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.03f, 0.15f, 0.98f);
        Outline panelOutline = panel.AddComponent<Outline>();
        panelOutline.effectColor    = color;
        panelOutline.effectDistance = new Vector2(3, -3);
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = panelRT.anchorMax = panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta        = new Vector2(900, 620);
        panelRT.anchoredPosition = Vector2.zero;

        // BORDE SUPERIOR
        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(panel.transform, false);
        borde.AddComponent<Image>().color = color;
        RectTransform bordeRT = borde.GetComponent<RectTransform>();
        bordeRT.anchorMin = new Vector2(0, 1);
        bordeRT.anchorMax = new Vector2(1, 1);
        bordeRT.pivot     = new Vector2(0.5f, 1);
        bordeRT.sizeDelta = new Vector2(0, 10);
        bordeRT.anchoredPosition = Vector2.zero;

        // TITULO
        GameObject tituloGO = new GameObject("Titulo");
        tituloGO.transform.SetParent(panel.transform, false);
        TextMeshProUGUI tituloTMP = tituloGO.AddComponent<TextMeshProUGUI>();
        tituloTMP.text      = titulo;
        tituloTMP.fontSize  = 42;
        tituloTMP.fontStyle = FontStyles.Bold;
        tituloTMP.color     = color;
        tituloTMP.alignment = TextAlignmentOptions.Center;
        tituloGO.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.8f);
        RectTransform tituloRT = tituloGO.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0, 0.82f);
        tituloRT.anchorMax = new Vector2(1, 1f);
        tituloRT.sizeDelta = Vector2.zero;

        // CONTENIDO
        GameObject contenidoGO = new GameObject("Contenido");
        contenidoGO.transform.SetParent(panel.transform, false);
        TextMeshProUGUI contenidoTMP = contenidoGO.AddComponent<TextMeshProUGUI>();
        contenidoTMP.text      = contenido;
        contenidoTMP.fontSize  = 24;
        contenidoTMP.color     = Color.white;
        contenidoTMP.alignment = TextAlignmentOptions.Center;
        RectTransform contenidoRT = contenidoGO.GetComponent<RectTransform>();
        contenidoRT.anchorMin = new Vector2(0.05f, 0.12f);
        contenidoRT.anchorMax = new Vector2(0.95f, 0.82f);
        contenidoRT.sizeDelta = Vector2.zero;

        // BOTON CERRAR
        GameObject btnCerrar = CreateButton(panel, "CERRAR", new Color(0.8f, 0.1f, 0.1f),
                               new Vector2(0, -270f), new Vector2(250, 60));
        btnCerrar.GetComponent<Button>().onClick.AddListener(() => overlay.SetActive(false));

        return overlay;
    }

    void OnButtonClick(string boton)
    {
        if (boton == "JUGAR")
            SceneManager.LoadScene(gameSceneName);
        else if (boton == "PUNTUACIONES")
            SceneManager.LoadScene("Highscores");
        else if (boton == "INSTRUCCIONES")
            panelInstrucciones.SetActive(true);
        else if (boton == "AGRADECIMIENTOS")
            panelAgradecimientos.SetActive(true);
    }

    void CreateStar(GameObject parent, int index)
    {
        GameObject star = new GameObject("Star_" + index);
        star.transform.SetParent(parent.transform, false);
        Image img = star.AddComponent<Image>();
        img.color = new Color(1f, 1f, 0.6f, Random.Range(0.4f, 1f));
        RectTransform rt = star.GetComponent<RectTransform>();
        float size = Random.Range(6f, 18f);
        rt.sizeDelta = new Vector2(size, size);
        rt.anchorMin = rt.anchorMax = new Vector2(
            Random.Range(0.02f, 0.98f),
            Random.Range(0.02f, 0.98f));
        rt.anchoredPosition = Vector2.zero;
        star.AddComponent<StarBlink>();
    }

    void CreateCoin(GameObject parent, int index)
    {
        GameObject coin = new GameObject("Coin_" + index);
        coin.transform.SetParent(parent.transform, false);
        Image img = coin.AddComponent<Image>();
        img.color = new Color(1f, 0.85f, 0f, 0.85f);
        RectTransform rt = coin.GetComponent<RectTransform>();
        float size = Random.Range(20f, 45f);
        rt.sizeDelta = new Vector2(size, size);
        rt.anchorMin = rt.anchorMax = new Vector2(Random.Range(0f, 1f), 1.1f);
        rt.anchoredPosition = Vector2.zero;
        CoinFall cf = coin.AddComponent<CoinFall>();
        cf.speed  = Random.Range(80f, 220f);
        cf.delay  = Random.Range(0f, 5f);
        cf.startX = Random.Range(-900f, 900f);
    }

    GameObject CreateImage(GameObject parent, string name, Sprite sprite)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        Image img = go.AddComponent<Image>();
        if (sprite != null) img.sprite = sprite;
        img.preserveAspect = true;
        return go;
    }

    GameObject CreateButton(GameObject parent, string label, Color color,
                             Vector2 position, Vector2 size)
    {
        GameObject btnGO = new GameObject("Btn_" + label);
        btnGO.transform.SetParent(parent.transform, false);
        Image img = btnGO.AddComponent<Image>();
        img.color = color;
        img.type  = Image.Type.Sliced;
        Button btn = btnGO.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor      = color;
        cb.highlightedColor = color * 1.2f;
        cb.pressedColor     = color * 0.7f;
        btn.colors = cb;
        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.sizeDelta        = size;
        rt.anchorMin        = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        Shadow shadow = btnGO.AddComponent<Shadow>();
        shadow.effectColor    = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(5, -5);
        Outline outline = btnGO.AddComponent<Outline>();
        outline.effectColor    = new Color(1f, 1f, 1f, 0.4f);
        outline.effectDistance = new Vector2(2, -2);

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 34;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin        = Vector2.zero;
        textRT.anchorMax        = Vector2.one;
        textRT.sizeDelta        = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
        return btnGO;
    }

    void StretchFull(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.sizeDelta        = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}