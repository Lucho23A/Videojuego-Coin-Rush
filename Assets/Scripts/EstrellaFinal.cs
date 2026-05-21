using UnityEngine;
using UnityEngine.SceneManagement;
public class EstrellaFinal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);

            // Nombre del nivel según el índice actual
            string[] nombreNiveles = { "", "Pradera del Inicio", "Montaña Helada", "Lago de los Lirios" };
            int idx = GameManager.Instance.currentLevelIndex;
            string nombreNivel = (idx >= 0 && idx < nombreNiveles.Length)
                ? nombreNiveles[idx] : "Nivel Desconocido";

            var entry = HighscoreSystem.CreateEntry(
                playerName:    "Jugador",
                characterName: $"Personaje {GameManager.Instance.selectedCharacterIndex + 1}",
                levelIndex:    idx,
                levelName:     nombreNivel,
                score:         GameManager.Instance.currentScore,
                timeUsed:      0f
            );
            HighscoreSystem.AddEntry(entry);
        }

        SceneManager.LoadScene("LevelSelect");
    }
}