using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuGenerator : MonoBehaviour
{
    [Header("Arrastra tus sprites aquí")]
    public Sprite fondoSprite;
    public Sprite logoSprite;

    [Header("Música de fondo")]
    public AudioClip musicaMenu;

    [Header("Nombre de tu escena de juego")]
    public string gameSceneName = "LevelSelect";

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
        // ── CANVAS ──────────────────────────────────
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── FONDO ────────────────────────────────────
        GameObject fondo = CreateImage(canvasGO, "Fondo", fondoSprite);
        StretchFull(fondo);

        // ── ESTRELLAS PARPADEANTES ───────────────────
        for (int i = 0; i < 40; i++)
        {
            GameObject star = new GameObject("Star_" + i);
            star.transform.SetParent(canvasGO.transform, false);
            Image img = star.AddComponent<Image>();
            img.color = new Color(1f, 1f, 0.6f, 0.8f);
            RectTransform rt = star.GetComponent<RectTransform>();
            float size = Random.Range(6f, 18f);
            rt.sizeDelta   = new Vector2(size, size);
            rt.anchorMin   = rt.anchorMax = new Vector2(
                Random.Range(0.02f, 0.98f),
                Random.Range(0.02f, 0.98f));
            rt.anchoredPosition = Vector2.zero;
            star.AddComponent<StarBlink>();
        }

        // ── MONEDAS CAYENDO ──────────────────────────
        for (int i = 0; i < 15; i++)
        {
            GameObject coin = new GameObject("Coin_" + i);
            coin.transform.SetParent(canvasGO.transform, false);
            Image img = coin.AddComponent<Image>();
            img.color = new Color(1f, 0.85f, 0f, 0.9f);
            RectTransform rt = coin.GetComponent<RectTransform>();
            float size = Random.Range(20f, 45f);
            rt.sizeDelta        = new Vector2(size, size);
            rt.anchorMin        = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(
                Random.Range(-900f, 900f), 600f);
            CoinFall cf = coin.AddComponent<CoinFall>();
            cf.speed  = Random.Range(80f, 220f);
            cf.delay  = Random.Range(0f, 5f);
            cf.startX = Random.Range(-900f, 900f);
        }

        // ── LOGO ─────────────────────────────────────
        GameObject logo = CreateImage(canvasGO, "Logo", logoSprite);
        RectTransform logoRT = logo.GetComponent<RectTransform>();
        logoRT.sizeDelta        = new Vector2(700, 280);
        logoRT.anchoredPosition = new Vector2(0, 180);
        logo.AddComponent<LogoBounce>();

        // ── BOTONES ──────────────────────────────────
        string[] textos = { ">>  JUGAR", ">>  PUNTUACIONES", ">>  OPCIONES" };
        string[] ids    = { "JUGAR", "PUNTUACIONES", "OPCIONES" };
        Color[]  colores = {
            HexColor("#FFB300"),
            HexColor("#0288D1"),
            HexColor("#388E3C")
        };
        float[] posY = { 20f, -80f, -180f };

        for (int i = 0; i < textos.Length; i++)
        {
            string id = ids[i];
            GameObject btn = CreateButton(canvasGO, textos[i], colores[i],
                                          new Vector2(0, posY[i]),
                                          new Vector2(420, 75));
            btn.AddComponent<ButtonGlow>();
            btn.GetComponent<Button>().onClick.AddListener(() =>
                OnButtonClick(id));
        }

        // ── EventSystem ───────────────────────────────
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    void OnButtonClick(string boton)
    {
        if (boton == "JUGAR")
            SceneManager.LoadScene(gameSceneName);
        else if (boton == "PUNTUACIONES")
            SceneManager.LoadScene("Highscores");
        else if (boton == "OPCIONES")
            Debug.Log("Opciones próximamente");
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

        RectTransform rt    = btnGO.GetComponent<RectTransform>();
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

        RectTransform textRT    = textGO.GetComponent<RectTransform>();
        textRT.anchorMin        = Vector2.zero;
        textRT.anchorMax        = Vector2.one;
        textRT.sizeDelta        = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;

        return btnGO;
    }

    void StretchFull(GameObject go)
    {
        RectTransform rt    = go.GetComponent<RectTransform>();
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