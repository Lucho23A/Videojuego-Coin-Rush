using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectGenerator : MonoBehaviour
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
        overlay.AddComponent<Image>().color = new Color(0, 0, 0, 0.4f);
        RectTransform overlayRT = overlay.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero; overlayRT.anchorMax = Vector2.one; overlayRT.sizeDelta = Vector2.zero;

        // TITULO
        GameObject titulo = new GameObject("Titulo");
        titulo.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI tituloTMP = titulo.AddComponent<TextMeshProUGUI>();
        tituloTMP.text = "SELECCIONA UN NIVEL";
        tituloTMP.fontSize = 75;
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

        // NIVELES
        string[] nombres = { "PRADO", "MONTANA", "LAGO" };
        string[] escenas = { "Level1_Meadow", "Level2_Mountain", "Level3_Lake" };
        Color[] colores = {
            new Color(0.1f, 0.8f, 0.2f),
            new Color(0.6f, 0.4f, 0.1f),
            new Color(0.1f, 0.5f, 1f)
        };
        string[] emojis = { "1", "2", "3" };
        float[] posX = { -450f, 0f, 450f };

        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            string escena = escenas[i];
            CreateLevelCard(canvasGO, nombres[i], emojis[i], colores[i], posX[i], escena);
        }

        // BOTON VOLVER
        CreateButton(canvasGO, "VOLVER", new Color(0.8f, 0.1f, 0.1f), new Vector2(0, -450f), new Vector2(280, 65))
            .GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("CharacterSelect"));

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    void CreateLevelCard(GameObject parent, string nombre, string numero, Color color, float posX, string escena)
    {
        // CARTA
        GameObject card = new GameObject("Card_" + nombre);
        card.transform.SetParent(parent.transform, false);
        Image cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(0.08f, 0.05f, 0.18f, 0.95f);
        RectTransform cardRT = card.GetComponent<RectTransform>();
        cardRT.anchorMin = cardRT.anchorMax = cardRT.pivot = new Vector2(0.5f, 0.5f);
        cardRT.sizeDelta = new Vector2(380, 460);
        cardRT.anchoredPosition = new Vector2(posX, 20f);
        Outline cardOutline = card.AddComponent<Outline>();
        cardOutline.effectColor = Color.white;
        cardOutline.effectDistance = new Vector2(3, -3);
        card.AddComponent<Shadow>().effectColor = new Color(color.r, color.g, color.b, 0.5f);

        // BORDE SUPERIOR
        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(card.transform, false);
        Image bordeImg = borde.AddComponent<Image>();
        bordeImg.color = color;
        RectTransform bordeRT = borde.GetComponent<RectTransform>();
        bordeRT.anchorMin = new Vector2(0, 1);
        bordeRT.anchorMax = new Vector2(1, 1);
        bordeRT.pivot = new Vector2(0.5f, 1);
        bordeRT.sizeDelta = new Vector2(0, 10);
        bordeRT.anchoredPosition = Vector2.zero;

        // NUMERO DEL NIVEL
        GameObject numGO = new GameObject("Numero");
        numGO.transform.SetParent(card.transform, false);
        Image numImg = numGO.AddComponent<Image>();
        numImg.color = color;
        RectTransform numRT = numGO.GetComponent<RectTransform>();
        numRT.anchorMin = numRT.anchorMax = numRT.pivot = new Vector2(0.5f, 0.5f);
        numRT.sizeDelta = new Vector2(160, 160);
        numRT.anchoredPosition = new Vector2(0, 120f);

        GameObject numTextoGO = new GameObject("NumTexto");
        numTextoGO.transform.SetParent(numGO.transform, false);
        TextMeshProUGUI numTMP = numTextoGO.AddComponent<TextMeshProUGUI>();
        numTMP.text = numero;
        numTMP.fontSize = 90;
        numTMP.fontStyle = FontStyles.Bold;
        numTMP.color = Color.white;
        numTMP.alignment = TextAlignmentOptions.Center;
        RectTransform numTextoRT = numTextoGO.GetComponent<RectTransform>();
        numTextoRT.anchorMin = Vector2.zero; numTextoRT.anchorMax = Vector2.one;
        numTextoRT.sizeDelta = Vector2.zero; numTextoRT.anchoredPosition = Vector2.zero;

        // NOMBRE NIVEL
        GameObject nombreGO = new GameObject("Nombre");
        nombreGO.transform.SetParent(card.transform, false);
        TextMeshProUGUI nombreTMP = nombreGO.AddComponent<TextMeshProUGUI>();
        nombreTMP.text = "NIVEL " + numero + "\n" + nombre;
        nombreTMP.fontSize = 38;
        nombreTMP.fontStyle = FontStyles.Bold;
        nombreTMP.color = color;
        nombreTMP.alignment = TextAlignmentOptions.Center;
        nombreGO.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.8f);
        RectTransform nombreRT = nombreGO.GetComponent<RectTransform>();
        nombreRT.anchorMin = nombreRT.anchorMax = nombreRT.pivot = new Vector2(0.5f, 0.5f);
        nombreRT.sizeDelta = new Vector2(340, 100);
        nombreRT.anchoredPosition = new Vector2(0, -30f);

        // BOTON JUGAR
        string escenaFinal = escena;
        CreateButton(card, "JUGAR", color, new Vector2(0, -168f), new Vector2(300, 65))
            .GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(escenaFinal));
    }

    GameObject CreateButton(GameObject parent, string label, Color color, Vector2 pos, Vector2 size)
    {
        GameObject shadowGO = new GameObject("Shadow_" + label);
        shadowGO.transform.SetParent(parent.transform, false);
        shadowGO.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        RectTransform shadowRT = shadowGO.GetComponent<RectTransform>();
        shadowRT.anchorMin = shadowRT.anchorMax = shadowRT.pivot = new Vector2(0.5f, 0.5f);
        shadowRT.sizeDelta = new Vector2(size.x + 8, size.y + 8);
        shadowRT.anchoredPosition = new Vector2(pos.x + 5, pos.y - 5);

        GameObject bordeGO = new GameObject("Borde_" + label);
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
        Shadow textShadow = textGO.AddComponent<Shadow>();
        textShadow.effectColor = new Color(0, 0, 0, 0.8f);
        textShadow.effectDistance = new Vector2(2, -2);
        RectTransform trt = textGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero; trt.anchoredPosition = Vector2.zero;

        return btnGO;
    }
}