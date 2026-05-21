using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuGenerator : MonoBehaviour
{
    [Header("Arrastra tus sprites aquí")]
    public Sprite fondoSprite;
    public Sprite logoSprite;

    [Header("Nombre de tu escena de juego")]
    public string gameSceneName = "GameScene";

    void Start()
    {
        BuildMenu();
    }

    void BuildMenu()
    {
        // ── CANVAS ──────────────────────────────────────────
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── FONDO ────────────────────────────────────────────
        GameObject fondo = CreateImage(canvasGO, "Fondo", fondoSprite);
        StretchFull(fondo);

        // ── LOGO ─────────────────────────────────────────────
        GameObject logo = CreateImage(canvasGO, "Logo", logoSprite);
        RectTransform logoRT = logo.GetComponent<RectTransform>();
        logoRT.sizeDelta = new Vector2(700, 280);
        logoRT.anchoredPosition = new Vector2(0, 180);

        // Animación bounce al logo
        logo.AddComponent<LogoBounce>();

        // ── BOTONES ──────────────────────────────────────────
        string[] nombres   = { "🎮  JUGAR", "🏆  PUNTUACIONES", "⚙️  OPCIONES" };
        Color[]  colores   = {
            HexColor("#FFB300"),
            HexColor("#0288D1"),
            HexColor("#388E3C")
        };
        float[] posicionesY = { 20f, -80f, -180f };

        for (int i = 0; i < nombres.Length; i++)
        {
            int index = i; // captura para el lambda
            GameObject btn = CreateButton(
                canvasGO,
                nombres[i],
                colores[i],
                new Vector2(0, posicionesY[i]),
                new Vector2(420, 75)
            );

            // Asigna acción a cada botón
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnButtonClick(nombres[index]);
            });
        }

        // ── EventSystem (necesario para los botones) ─────────
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    // ── ACCIONES DE BOTONES ──────────────────────────────────
    void OnButtonClick(string boton)
    {
        if (boton.Contains("JUGAR"))
            SceneManager.LoadScene(gameSceneName);
        else if (boton.Contains("PUNTUACIONES"))
            Debug.Log("Mostrar puntuaciones");
        else if (boton.Contains("OPCIONES"))
            Debug.Log("Mostrar opciones");
    }

    // ── HELPERS ──────────────────────────────────────────────
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
        // Contenedor del botón
        GameObject btnGO = new GameObject("Btn_" + label);
        btnGO.transform.SetParent(parent.transform, false);

        Image img = btnGO.AddComponent<Image>();
        img.color = color;

        // Bordes redondeados con sprite sliced
        img.type = Image.Type.Sliced;

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor    = color;
        cb.highlightedColor = color * 1.15f;
        cb.pressedColor   = color * 0.75f;
        btn.colors = cb;

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;

        // Sombra del botón
        Shadow shadow = btnGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.4f);
        shadow.effectDistance = new Vector2(4, -4);

        // Texto TMP
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 36;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;

        return btnGO;
    }

    void StretchFull(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}

