using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HighscoresGenerator : MonoBehaviour
{
    [Header("Fondo")]
    public Sprite fondoSprite;

    void Start()
    {
        var oldCanvas = FindFirstObjectByType<Canvas>();
        if (oldCanvas != null) Destroy(oldCanvas.gameObject);
        BuildUI();
    }

    void BuildUI()
    {
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // FONDO
        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(canvasGO.transform, false);
        Image fondoImg = fondo.AddComponent<Image>();
        if (fondoSprite != null) { fondoImg.sprite = fondoSprite; fondoImg.preserveAspect = false; }
        else fondoImg.color = new Color(0.08f, 0.04f, 0.25f);
        RectTransform fondoRT = fondo.GetComponent<RectTransform>();
        fondoRT.anchorMin = Vector2.zero; fondoRT.anchorMax = Vector2.one; fondoRT.sizeDelta = Vector2.zero;

        // OVERLAY
        GameObject overlay = new GameObject("Overlay");
        overlay.transform.SetParent(canvasGO.transform, false);
        overlay.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        RectTransform overlayRT = overlay.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero; overlayRT.anchorMax = Vector2.one; overlayRT.sizeDelta = Vector2.zero;

        // TITULO
        GameObject titulo = new GameObject("Titulo");
        titulo.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI tituloTMP = titulo.AddComponent<TextMeshProUGUI>();
        tituloTMP.text = "TABLA DE PUNTUACIONES";
        tituloTMP.fontSize = 72;
        tituloTMP.fontStyle = FontStyles.Bold;
        tituloTMP.color = new Color(1f, 0.85f, 0f);
        tituloTMP.alignment = TextAlignmentOptions.Center;
        Shadow tShadow = titulo.AddComponent<Shadow>();
        tShadow.effectColor = new Color(1f, 0.4f, 0f, 1f);
        tShadow.effectDistance = new Vector2(4, -4);
        RectTransform tituloRT = titulo.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0, 0.82f);
        tituloRT.anchorMax = new Vector2(1, 1f);
        tituloRT.sizeDelta = Vector2.zero;

        // TABLA
        GameObject tabla = new GameObject("Tabla");
        tabla.transform.SetParent(canvasGO.transform, false);
        Image tablaImg = tabla.AddComponent<Image>();
        tablaImg.color = new Color(0.05f, 0.03f, 0.15f, 0.9f);
        Outline tablaOutline = tabla.AddComponent<Outline>();
        tablaOutline.effectColor = new Color(1f, 0.85f, 0f, 0.8f);
        tablaOutline.effectDistance = new Vector2(3, -3);
        RectTransform tablaRT = tabla.GetComponent<RectTransform>();
        tablaRT.anchorMin = tablaRT.anchorMax = tablaRT.pivot = new Vector2(0.5f, 0.5f);
        tablaRT.sizeDelta = new Vector2(900, 500);
        tablaRT.anchoredPosition = new Vector2(0, 30f);

        // HEADER de la tabla
        CrearFilaHeader(tabla);

        // SCORES — intentar cargar del sistema, si no hay datos poner ejemplos
        var scores = HighscoreSystem.LoadAll();
        if (scores == null || scores.Count == 0)
        {
            // Datos de ejemplo para la entrega
            CrearFila(tabla, 1, "---", "Nivel 1", 0, "-:--", new Color(1f, 0.85f, 0f));
            CrearFila(tabla, 2, "---", "Nivel 2", 0, "-:--", Color.white);
            CrearFila(tabla, 3, "---", "Nivel 3", 0, "-:--", Color.white);
        }
        else
        {
            // Ordenar por score descendente
            scores.Sort((a, b) => b.score.CompareTo(a.score));
            Color[] rowColors = { new Color(1f, 0.85f, 0f), Color.white, new Color(0.8f, 0.8f, 0.8f) };
            for (int i = 0; i < Mathf.Min(scores.Count, 10); i++)
            {
                var s = scores[i];
                Color c = i < rowColors.Length ? rowColors[i] : Color.white;
                string nivelNombre = s.levelIndex == 1 ? "Prado" : s.levelIndex == 2 ? "Montana" : "Lago";
                CrearFila(tabla, i + 1, s.playerName ?? "???", nivelNombre, s.score, HighscoreSystem.FormatTime(s.timeUsed), c);
            }
        }

        // BOTON VOLVER
        CreateButton(canvasGO, "VOLVER AL MENU", new Color(0.8f, 0.1f, 0.1f), new Vector2(0, -450f), new Vector2(350, 65))
            .GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    void CrearFilaHeader(GameObject parent)
    {
        GameObject fila = new GameObject("Header");
        fila.transform.SetParent(parent.transform, false);
        Image filaImg = fila.AddComponent<Image>();
        filaImg.color = new Color(1f, 0.6f, 0f, 0.9f);
        RectTransform filaRT = fila.GetComponent<RectTransform>();
        filaRT.anchorMin = new Vector2(0, 1);
        filaRT.anchorMax = new Vector2(1, 1);
        filaRT.pivot = new Vector2(0.5f, 1);
        filaRT.sizeDelta = new Vector2(0, 55);
        filaRT.anchoredPosition = Vector2.zero;

        string[] headers = { "#", "JUGADOR", "NIVEL", "PUNTOS", "TIEMPO" };
        float[] posXHeaders = { -370f, -180f, 0f, 180f, 370f };

        for (int i = 0; i < headers.Length; i++)
        {
            GameObject h = new GameObject("H_" + headers[i]);
            h.transform.SetParent(fila.transform, false);
            TextMeshProUGUI hTMP = h.AddComponent<TextMeshProUGUI>();
            hTMP.text = headers[i];
            hTMP.fontSize = 26;
            hTMP.fontStyle = FontStyles.Bold;
            hTMP.color = Color.white;
            hTMP.alignment = TextAlignmentOptions.Center;
            RectTransform hRT = h.GetComponent<RectTransform>();
            hRT.anchorMin = hRT.anchorMax = hRT.pivot = new Vector2(0.5f, 0.5f);
            hRT.sizeDelta = new Vector2(160, 50);
            hRT.anchoredPosition = new Vector2(posXHeaders[i], 0);
        }
    }

    void CrearFila(GameObject parent, int pos, string jugador, string nivel, int puntos, string tiempo, Color color)
    {
        GameObject fila = new GameObject("Fila_" + pos);
        fila.transform.SetParent(parent.transform, false);
        Image filaImg = fila.AddComponent<Image>();
        filaImg.color = pos % 2 == 0 ? new Color(0.1f, 0.06f, 0.25f, 0.8f) : new Color(0.08f, 0.04f, 0.2f, 0.8f);
        RectTransform filaRT = fila.GetComponent<RectTransform>();
        filaRT.anchorMin = new Vector2(0, 1);
        filaRT.anchorMax = new Vector2(1, 1);
        filaRT.pivot = new Vector2(0.5f, 1);
        filaRT.sizeDelta = new Vector2(0, 50);
        filaRT.anchoredPosition = new Vector2(0, -55 - (pos - 1) * 50f);

        string[] valores = { pos.ToString(), jugador, nivel, puntos.ToString(), tiempo };
        float[] posXVals = { -370f, -180f, 0f, 180f, 370f };

        for (int i = 0; i < valores.Length; i++)
        {
            GameObject v = new GameObject("V_" + i);
            v.transform.SetParent(fila.transform, false);
            TextMeshProUGUI vTMP = v.AddComponent<TextMeshProUGUI>();
            vTMP.text = valores[i];
            vTMP.fontSize = 24;
            vTMP.fontStyle = i == 0 ? FontStyles.Bold : FontStyles.Normal;
            vTMP.color = i == 0 ? color : Color.white;
            vTMP.alignment = TextAlignmentOptions.Center;
            RectTransform vRT = v.GetComponent<RectTransform>();
            vRT.anchorMin = vRT.anchorMax = vRT.pivot = new Vector2(0.5f, 0.5f);
            vRT.sizeDelta = new Vector2(160, 45);
            vRT.anchoredPosition = new Vector2(posXVals[i], 0);
        }
    }

    GameObject CreateButton(GameObject parent, string label, Color color, Vector2 pos, Vector2 size)
    {
        GameObject shadowGO = new GameObject("Shadow");
        shadowGO.transform.SetParent(parent.transform, false);
        shadowGO.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        RectTransform shadowRT = shadowGO.GetComponent<RectTransform>();
        shadowRT.anchorMin = shadowRT.anchorMax = shadowRT.pivot = new Vector2(0.5f, 0.5f);
        shadowRT.sizeDelta = new Vector2(size.x + 8, size.y + 8);
        shadowRT.anchoredPosition = new Vector2(pos.x + 5, pos.y - 5);

        GameObject bordeGO = new GameObject("Borde");
        bordeGO.transform.SetParent(parent.transform, false);
        bordeGO.AddComponent<Image>().color = Color.white;
        RectTransform bordeRT = bordeGO.GetComponent<RectTransform>();
        bordeRT.anchorMin = bordeRT.anchorMax = bordeRT.pivot = new Vector2(0.5f, 0.5f);
        bordeRT.sizeDelta = new Vector2(size.x + 10, size.y + 10);
        bordeRT.anchoredPosition = pos;

        GameObject btnGO = new GameObject("Btn_" + label);
        btnGO.transform.SetParent(parent.transform, false);
        Image img = btnGO.AddComponent<Image>();
        img.color = color;
        Button btn = btnGO.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.highlightedColor = Color.white;
        cb.pressedColor = color * 0.6f;
        btn.colors = cb;
        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        btnGO.AddComponent<ButtonGlow>();

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 30;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform trt = textGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero; trt.anchoredPosition = Vector2.zero;
        return btnGO;
    }
}