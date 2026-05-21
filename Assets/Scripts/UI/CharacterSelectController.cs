using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controla la pantalla de selección de personaje.
/// Muestra los 3 CharacterData en orden, permite navegar con flechas
/// y guarda la elección en GameManager antes de cargar LevelSelect.
/// </summary>
public class CharacterSelectController : MonoBehaviour
{
    [Header("Datos de personajes (arrastrar los 3 ScriptableObjects en orden)")]
    [SerializeField] private CharacterData[] characters;

    [Header("UI — Panel de información")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image portraitImage;

    [Header("UI — Barras de stats")]
    [SerializeField] private Slider speedBar;
    [SerializeField] private Slider jumpBar;
    [SerializeField] private Slider healthBar;

    [Header("UI — Botones de navegación")]
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button backButton;

    [Header("UI — Indicadores de posición (puntos)")]
    [SerializeField] private Image[] dotIndicators;
    [SerializeField] private Color dotActive = Color.white;
    [SerializeField] private Color dotInactive = new Color(1f, 1f, 1f, 0.3f);

    [Header("Vista 3D del personaje (opcional)")]
    [SerializeField] private Transform characterDisplayPoint;

    // Valores máximos para normalizar las barras (coinciden con el mayor stat del GDD)
    private const float MAX_SPEED = 10f;
    private const float MAX_JUMP  = 11f;
    private const float MAX_HP    = 5f;

    private int currentIndex = 0;
    private GameObject currentCharacterPreview;

    private void Start()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("[CharacterSelect] No hay CharacterData asignados.");
            return;
        }

        leftArrowButton.onClick.AddListener(PreviousCharacter);
        rightArrowButton.onClick.AddListener(NextCharacter);
        selectButton.onClick.AddListener(ConfirmSelection);
        backButton.onClick.AddListener(GoBack);

        // Si hay un personaje ya seleccionado, arranca en ese índice
        if (GameManager.Instance != null)
            currentIndex = Mathf.Clamp(GameManager.Instance.selectedCharacterIndex, 0, characters.Length - 1);

        ShowCharacter(currentIndex);
    }

    private void Update()
    {
        // Navegación con teclado (A/D o flechas)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            PreviousCharacter();
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            NextCharacter();
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            ConfirmSelection();
    }

    private void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        ShowCharacter(currentIndex);
    }

    private void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        ShowCharacter(currentIndex);
    }

    private void ShowCharacter(int index)
    {
        CharacterData data = characters[index];

        // Texto
        nameText.text = $"{data.characterName}\n<size=60%><i>{data.nickname}</i></size>";
        descriptionText.text = $"{data.description}\n\n<b>Habilidad especial:</b> {data.specialAbility}";

        // Retrato
        if (portraitImage != null)
            portraitImage.sprite = data.portrait;

        // Barras de stats (normalizadas 0-1)
        speedBar.value  = data.moveSpeed  / MAX_SPEED;
        jumpBar.value   = data.jumpForce  / MAX_JUMP;
        healthBar.value = (float)data.maxHealth / MAX_HP;

        // Puntos indicadores
        for (int i = 0; i < dotIndicators.Length; i++)
            dotIndicators[i].color = (i == index) ? dotActive : dotInactive;

        // Mostrar botones de flecha según si hay personaje anterior/siguiente
        leftArrowButton.interactable  = characters.Length > 1;
        rightArrowButton.interactable = characters.Length > 1;

        // Preview 3D: destruir el anterior e instanciar el nuevo
        ShowCharacterPreview(data);
    }

    private void ShowCharacterPreview(CharacterData data)
    {
        if (characterDisplayPoint == null || data.characterPrefab == null) return;

        if (currentCharacterPreview != null)
            Destroy(currentCharacterPreview);

        currentCharacterPreview = Instantiate(data.characterPrefab, characterDisplayPoint.position, characterDisplayPoint.rotation);

        // Desactivar físicas y scripts del personaje en la pantalla de selección
        if (currentCharacterPreview.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = true;
        foreach (var script in currentCharacterPreview.GetComponents<MonoBehaviour>())
            script.enabled = false;
    }

    private void ConfirmSelection()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.selectedCharacterIndex = currentIndex;

        Debug.Log($"[CharacterSelect] Personaje seleccionado: {characters[currentIndex].characterName}");
        SceneManager.LoadScene("LevelSelect");
    }

    private void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
