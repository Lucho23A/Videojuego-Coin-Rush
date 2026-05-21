using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectGenerator : MonoBehaviour
{
    [Header("Fondo")]
    public Sprite fondoSprite;

    [Header("Fotos de personajes")]
    public Sprite fotoLaura;
    public Sprite fotoLuis;
    public Sprite fotoMafe;

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
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0f, 0f, 0f, 0.4f);
        RectTransform overlayRT = overlay.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero; overlayRT.anchorMax = Vector2.one; overlayRT.sizeDelta = Vector2.zero;

        // TITULO
        GameObject titulo = new GameObject("Titulo");
        titulo.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI tituloTMP = titulo.AddComponent<TextMeshProUGUI>();
        tituloTMP.text = "ELIGE TU PERSONAJE";
        tituloTMP.fontSize = 80;
        tituloTMP.fontStyle = FontStyles.Bold;
        tituloTMP.color = new Color(1f, 0.85f, 0f);
        tituloTMP.alignment = TextAlignmentOptions.Center;
        Shadow tituloShadow = titulo.AddComponent<Shadow>();
        tituloShadow.effectColor = new Color(1f, 0.4f, 0f, 1f);
        tituloShadow.effectDistance = new Vector2(4, -4);
        RectTransform tituloRT = titulo.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0, 0.82f);
        tituloRT.anchorMax = new Vector2(1, 1f);
        tituloRT.sizeDelta = Vector2.zero;

        // MONEDAS DECORATIVAS
        for (int i = 0; i < 8; i++)
            CreateDecoMoneda(canvasGO, new Vector2(Random.Range(-900f, 900f), Random.Range(380f, 480f)));

        // CARTAS
        string[] nombres = { "Laura", "Luis", "Mafe" };
        string[] descripciones = { "Velocidad maxima\nDoble salto en el aire", "Fuerza brutal\nGolpe especial de poder", "Equilibrio perfecto\nEscudo magico protector" };
        string[] habilidades = { "Doble Salto", "Super Golpe", "Escudo Magico" };
        Color[] colores = {
            new Color(1f, 0.4f, 0.05f),
            new Color(0.1f, 0.55f, 1f),
            new Color(0.1f, 0.85f, 0.3f)
        };
        Sprite[] fotos = { fotoLaura, fotoLuis, fotoMafe };
        float[] posX = { -500f, 0f, 500f };

        for (int i = 0; i < 3; i++)
            CreateCard(canvasGO, nombres[i], descripciones[i], habilidades[i], colores[i], fotos[i], posX[i], i);

        // BOTON VOLVER
        CreateButton(canvasGO, "VOLVER", new Color(0.8f, 0.1f, 0.1f), new Vector2(0, -450f), new Vector2(280, 65))
            .GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

        // EVENT SYSTEM
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    void CreateCard(GameObject parent, string nombre, string desc, string habilidad, Color color, Sprite foto, float posX, int index)
    {
        // CARTA BASE
        GameObject card = new GameObject("Card_" + nombre);
        card.transform.SetParent(parent.transform, false);
        Image cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(0.08f, 0.05f, 0.18f, 0.95f);
        RectTransform cardRT = card.GetComponent<RectTransform>();
        cardRT.anchorMin = cardRT.anchorMax = cardRT.pivot = new Vector2(0.5f, 0.5f);
        cardRT.sizeDelta = new Vector2(390, 560);
        cardRT.anchoredPosition = new Vector2(posX, 10f);
        Outline cardOutline = card.AddComponent<Outline>();
        cardOutline.effectColor = Color.white;
        cardOutline.effectDistance = new Vector2(3, -3);
        Shadow cardShadow = card.AddComponent<Shadow>();
        cardShadow.effectColor = new Color(color.r, color.g, color.b, 0.6f);
        cardShadow.effectDistance = new Vector2(0, -8);

        // BORDE SUPERIOR DE COLOR
        GameObject borde = new GameObject("BorderTop");
        borde.transform.SetParent(card.transform, false);
        Image bordeImg = borde.AddComponent<Image>();
        bordeImg.color = color;
        RectTransform bordeRT = borde.GetComponent<RectTransform>();
        bordeRT.anchorMin = new Vector2(0, 1);
        bordeRT.anchorMax = new Vector2(1, 1);
        bordeRT.pivot = new Vector2(0.5f, 1);
        bordeRT.sizeDelta = new Vector2(0, 10);
        bordeRT.anchoredPosition = Vector2.zero;

        // FOTO O ICONO
        GameObject iconoGO = new GameObject("Icono");
        iconoGO.transform.SetParent(card.transform, false);
        Image iconoImg = iconoGO.AddComponent<Image>();
        if (foto != null)
        {
            iconoImg.sprite = foto;
            iconoImg.color = Color.white;
            iconoImg.preserveAspect = true;
        }
        else
        {
            iconoImg.color = new Color(color.r * 0.4f, color.g * 0.4f, color.b * 0.4f);
        }
        Outline iconoOutline = iconoGO.AddComponent<Outline>();
        iconoOutline.effectColor = color;
        iconoOutline.effectDistance = new Vector2(3, -3);
        RectTransform iconoRT = iconoGO.GetComponent<RectTransform>();
        iconoRT.anchorMin = iconoRT.anchorMax = iconoRT.pivot = new Vector2(0.5f, 0.5f);
        iconoRT.sizeDelta = new Vector2(170, 220);
        iconoRT.anchoredPosition = new Vector2(0, 155f);

        // NOMBRE
        GameObject nombreGO = new GameObject("Nombre");
        nombreGO.transform.SetParent(card.transform, false);
        TextMeshProUGUI nombreTMP = nombreGO.AddComponent<TextMeshProUGUI>();
        nombreTMP.text = nombre.ToUpper();
        nombreTMP.fontSize = 44;
        nombreTMP.fontStyle = FontStyles.Bold;
        nombreTMP.color = color;
        nombreTMP.alignment = TextAlignmentOptions.Center;
        nombreGO.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.8f);
        RectTransform nombreRT = nombreGO.GetComponent<RectTransform>();
        nombreRT.anchorMin = nombreRT.anchorMax = nombreRT.pivot = new Vector2(0.5f, 0.5f);
        nombreRT.sizeDelta = new Vector2(360, 60);
        nombreRT.anchoredPosition = new Vector2(0, 20f);

        // DESCRIPCION
        GameObject descGO = new GameObject("Desc");
        descGO.transform.SetParent(card.transform, false);
        TextMeshProUGUI descTMP = descGO.AddComponent<TextMeshProUGUI>();
        descTMP.text = desc;
        descTMP.fontSize = 22;
        descTMP.color = new Color(0.9f, 0.9f, 0.9f);
        descTMP.alignment = TextAlignmentOptions.Center;
        RectTransform descRT = descGO.GetComponent<RectTransform>();
        descRT.anchorMin = descRT.anchorMax = descRT.pivot = new Vector2(0.5f, 0.5f);
        descRT.sizeDelta = new Vector2(350, 90);
        descRT.anchoredPosition = new Vector2(0, -80f);

        // BADGE HABILIDAD
        GameObject habGO = new GameObject("Habilidad");
        habGO.transform.SetParent(card.transform, false);
        Image habImg = habGO.AddComponent<Image>();
        habImg.color = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f);
        Outline habOutline = habGO.AddComponent<Outline>();
        habOutline.effectColor = color;
        habOutline.effectDistance = new Vector2(2, -2);
        RectTransform habRT = habGO.GetComponent<RectTransform>();
        habRT.anchorMin = habRT.anchorMax = habRT.pivot = new Vector2(0.5f, 0.5f);
        habRT.sizeDelta = new Vector2(320, 45);
        habRT.anchoredPosition = new Vector2(0, -158f);

        GameObject habTextoGO = new GameObject("HabTexto");
        habTextoGO.transform.SetParent(habGO.transform, false);
        TextMeshProUGUI habTMP = habTextoGO.AddComponent<TextMeshProUGUI>();
        habTMP.text = "* " + habilidad;
        habTMP.fontSize = 22;
        habTMP.fontStyle = FontStyles.Bold;
        habTMP.color = color;
        habTMP.alignment = TextAlignmentOptions.Center;
        RectTransform habTextoRT = habTextoGO.GetComponent<RectTransform>();
        habTextoRT.anchorMin = Vector2.zero; habTextoRT.anchorMax = Vector2.one;
        habTextoRT.sizeDelta = Vector2.zero; habTextoRT.anchoredPosition = Vector2.zero;

        // BOTON SELECCIONAR
        int idx = index;
        CreateButton(card, "SELECCIONAR", color, new Vector2(0, -228f), new Vector2(320, 65))
            .GetComponent<Button>().onClick.AddListener(() =>
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.selectedCharacterIndex = idx;
                SceneManager.LoadScene("LevelSelect");
            });
    }

    void CreateDecoMoneda(GameObject parent, Vector2 pos)
    {
        GameObject moneda = new GameObject("DecoMoneda");
        moneda.transform.SetParent(parent.transform, false);
        Image img = moneda.AddComponent<Image>();
        img.color = new Color(1f, 0.85f, 0f, 0.7f);
        RectTransform rt = moneda.GetComponent<RectTransform>();
        float size = Random.Range(15f, 35f);
        rt.sizeDelta = new Vector2(size, size);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        moneda.AddComponent<StarBlink>();
    }

    GameObject CreateButton(GameObject parent, string label, Color color, Vector2 pos, Vector2 size)
    {
        // SOMBRA
        GameObject shadowGO = new GameObject("Shadow_" + label);
        shadowGO.transform.SetParent(parent.transform, false);
        Image shadowImg = shadowGO.AddComponent<Image>();
        shadowImg.color = new Color(0, 0, 0, 0.5f);
        RectTransform shadowRT = shadowGO.GetComponent<RectTransform>();
        shadowRT.anchorMin = shadowRT.anchorMax = shadowRT.pivot = new Vector2(0.5f, 0.5f);
        shadowRT.sizeDelta = new Vector2(size.x + 8, size.y + 8);
        shadowRT.anchoredPosition = new Vector2(pos.x + 5, pos.y - 5);

        // BORDE BLANCO
        GameObject bordeGO = new GameObject("Borde_" + label);
        bordeGO.transform.SetParent(parent.transform, false);
        Image bordeImg = bordeGO.AddComponent<Image>();
        bordeImg.color = Color.white;
        RectTransform bordeRT = bordeGO.GetComponent<RectTransform>();
        bordeRT.anchorMin = bordeRT.anchorMax = bordeRT.pivot = new Vector2(0.5f, 0.5f);
        bordeRT.sizeDelta = new Vector2(size.x + 10, size.y + 10);
        bordeRT.anchoredPosition = pos;

        // BOTON
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

        // BRILLO SUPERIOR
        GameObject shine = new GameObject("Shine");
        shine.transform.SetParent(btnGO.transform, false);
        Image shineImg = shine.AddComponent<Image>();
        shineImg.color = new Color(1f, 1f, 1f, 0.2f);
        RectTransform shineRT = shine.GetComponent<RectTransform>();
        shineRT.anchorMin = new Vector2(0, 0.55f);
        shineRT.anchorMax = new Vector2(1, 1f);
        shineRT.sizeDelta = Vector2.zero;
        shineRT.anchoredPosition = Vector2.zero;

        // TEXTO
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