using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestor principal del juego. Persiste entre escenas.
/// Maneja personaje seleccionado, score, vidas, y nivel actual.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Personaje")]
    public int selectedCharacterIndex = 0; // 0, 1 o 2

    [Header("Estado del juego")]
    public int currentScore = 0;
    public int currentLives = 3;
    public int coinsCollected = 0;
    public int totalCoinsInLevel = 0;

    [Header("Nivel")]
    public int currentLevelIndex = 0;
    public float levelTimeRemaining = 0f;

    private void Awake()
    {
        // Patrón Singleton: solo una instancia, persistente entre escenas
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log($"[GameManager] Score: {currentScore}");
    }

    public void CollectCoin()
    {
        coinsCollected++;
        AddScore(10);
    }

    public void LoseLife()
    {
        currentLives--;
        Debug.Log($"[GameManager] Vidas restantes: {currentLives}");
        if (currentLives <= 0) GameOver();
    }

    public void GameOver()
    {
        Debug.Log("[GameManager] GAME OVER");
        // Pendiente: cargar pantalla de Game Over
    }

    public void LoadLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        ResetLevelStats();
        SceneManager.LoadScene($"Level{levelIndex}_" + GetLevelName(levelIndex));
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetLevelStats()
    {
        currentScore = 0;
        coinsCollected = 0;
        currentLives = 3;
    }

    private string GetLevelName(int index)
    {
        switch (index)
        {
            case 1: return "Meadow";
            case 2: return "Mountain";
            case 3: return "Lake";
            case 4: return "Castle";
            default: return "Meadow";
        }
    }
}
