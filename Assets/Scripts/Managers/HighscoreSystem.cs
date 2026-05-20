using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Sistema de puntuaciones. Guarda y carga los 10 mejores scores en JSON.
/// </summary>
public static class HighscoreSystem
{
    private static readonly string FILE_PATH =
        Path.Combine(Application.persistentDataPath, "highscores.json");

    private const int MAX_ENTRIES = 10;

    // ─── Modelo de datos ──────────────────────────────────────────

    [Serializable]
    public class HighscoreEntry
    {
        public string playerName;
        public string characterName;
        public int levelIndex;
        public string levelName;
        public int score;
        public float timeUsed;      // segundos que tardó
        public string date;          // fecha en formato dd/MM/yyyy
    }

    [Serializable]
    private class HighscoreList
    {
        public List<HighscoreEntry> entries = new List<HighscoreEntry>();
    }

    // ─── Guardar ──────────────────────────────────────────────────

    /// <summary>
    /// Añade una nueva entrada y guarda. Si ya hay 10, reemplaza la peor.
    /// </summary>
    public static void AddEntry(HighscoreEntry newEntry)
    {
        List<HighscoreEntry> entries = LoadAll();

        entries.Add(newEntry);

        // Ordena de mayor a menor score
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        // Mantiene solo los 10 mejores
        if (entries.Count > MAX_ENTRIES)
            entries.RemoveRange(MAX_ENTRIES, entries.Count - MAX_ENTRIES);

        SaveAll(entries);
        Debug.Log($"[HighscoreSystem] Entrada guardada: {newEntry.playerName} - {newEntry.score} pts");
    }

    // ─── Cargar ───────────────────────────────────────────────────

    /// <summary>
    /// Devuelve la lista ordenada de highscores.
    /// </summary>
    public static List<HighscoreEntry> LoadAll()
    {
        if (!File.Exists(FILE_PATH))
            return new List<HighscoreEntry>();

        try
        {
            string json = File.ReadAllText(FILE_PATH);
            HighscoreList list = JsonUtility.FromJson<HighscoreList>(json);
            return list?.entries ?? new List<HighscoreEntry>();
        }
        catch (Exception e)
        {
            Debug.LogError($"[HighscoreSystem] Error al cargar: {e.Message}");
            return new List<HighscoreEntry>();
        }
    }

    // ─── Borrar ───────────────────────────────────────────────────

    /// <summary>
    /// Borra todos los highscores (útil para el botón "Resetear").
    /// </summary>
    public static void ClearAll()
    {
        if (File.Exists(FILE_PATH))
            File.Delete(FILE_PATH);
        Debug.Log("[HighscoreSystem] Highscores borrados.");
    }

    // ─── Privados ─────────────────────────────────────────────────

    private static void SaveAll(List<HighscoreEntry> entries)
    {
        try
        {
            HighscoreList list = new HighscoreList { entries = entries };
            string json = JsonUtility.ToJson(list, prettyPrint: true);
            File.WriteAllText(FILE_PATH, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[HighscoreSystem] Error al guardar: {e.Message}");
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────

    /// <summary>
    /// Crea una entrada lista para guardar al terminar un nivel.
    /// Llama esto desde el GameManager al completar un nivel.
    /// </summary>
    public static HighscoreEntry CreateEntry(
        string playerName,
        string characterName,
        int levelIndex,
        string levelName,
        int score,
        float timeUsed)
    {
        return new HighscoreEntry
        {
            playerName = playerName,
            characterName = characterName,
            levelIndex = levelIndex,
            levelName = levelName,
            score = score,
            timeUsed = timeUsed,
            date = DateTime.Now.ToString("dd/MM/yyyy")
        };
    }

    /// <summary>
    /// Formatea segundos a string MM:SS
    /// </summary>
    public static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }
}
