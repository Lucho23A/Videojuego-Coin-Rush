using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controla la pantalla de selección de nivel.
/// Muestra 3 tarjetas con nombre, mejor puntaje y botón de jugar.
/// </summary>
public class LevelSelectController : MonoBehaviour
{
    [System.Serializable]
    public class LevelCard
    {
        public string levelName;           // Nombre visible ("Pradera del Inicio")
        public string sceneName;           // Nombre exacto de la escena en Unity
        public float  timeLimit;           // Segundos del temporizador
        public Sprite previewImage;        // Imagen/captura del nivel (opcional)

        [Header("Referencias UI de esta tarjeta")]
        public Button      playButton;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI bestScoreText;
        public TextMeshProUGUI timeLimitText;
        public Image       preview;
    }

    [Header("Tarjetas de nivel (una por nivel)")]
    [SerializeField] private LevelCard[] levels = new LevelCard[3];

    [Header("Botón volver")]
    [SerializeField] private Button backButton;

    [Header("Panel de personaje seleccionado (opcional)")]
    [SerializeField] private TextMeshProUGUI selectedCharacterText;

    private void Start()
    {
        // Mostrar qué personaje se eligió
        RefreshCharacterLabel();

        // Inicializar cada tarjeta
        for (int i = 0; i < levels.Length; i++)
        {
            int index = i; // captura para el lambda
            LevelCard card = levels[i];

            // Nombre del nivel
            if (card.nameText != null)
                card.nameText.text = $"Nivel {index + 1}\n<size=70%>{card.levelName}</size>";

            // Tiempo límite formateado
            if (card.timeLimitText != null)
                card.timeLimitText.text = "⏱ " + HighscoreSystem.FormatTime(card.timeLimit);

            // Mejor puntaje guardado
            RefreshBestScore(card, index + 1);

            // Imagen de preview
            if (card.preview != null && card.previewImage != null)
                card.preview.sprite = card.previewImage;

            // Botón jugar
            if (card.playButton != null)
                card.playButton.onClick.AddListener(() => LoadLevel(index));
        }

        // Botón volver
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }

    private void Update()
    {
        // Escape vuelve al menú anterior
        if (Input.GetKeyDown(KeyCode.Escape))
            GoBack();
    }

    // ─── Cargar nivel ─────────────────────────────────────────────────────────

    private void LoadLevel(int index)
    {
        string scene = levels[index].sceneName;

        if (string.IsNullOrEmpty(scene))
        {
            Debug.LogWarning($"[LevelSelect] La escena del nivel {index + 1} no está asignada.");
            return;
        }

        // Guardar cuál nivel se va a jugar en el GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.currentLevelIndex = index + 1;

        Debug.Log($"[LevelSelect] Cargando nivel {index + 1}: {scene}");
        SceneManager.LoadScene(scene);
    }

    // ─── Navegación ───────────────────────────────────────────────────────────

    private void GoBack()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    // ─── Helpers de UI ────────────────────────────────────────────────────────

    private void RefreshBestScore(LevelCard card, int levelIndex)
    {
        if (card.bestScoreText == null) return;

        // Busca el mejor score de este nivel en el archivo JSON
        var allScores = HighscoreSystem.LoadAll();
        int best = 0;
        foreach (var entry in allScores)
        {
            if (entry.levelIndex == levelIndex && entry.score > best)
                best = entry.score;
        }

        card.bestScoreText.text = best > 0 ? $"🏆 Mejor: {best} pts" : "🏆 Sin récord";
    }

    private void RefreshCharacterLabel()
    {
        if (selectedCharacterText == null || GameManager.Instance == null) return;

        string[] nombres = { "El Ágil", "La Saltarina", "El Fuerte" };
        int idx = GameManager.Instance.selectedCharacterIndex;
        if (idx >= 0 && idx < nombres.Length)
            selectedCharacterText.text = $"Personaje: {nombres[idx]}";
    }
}
