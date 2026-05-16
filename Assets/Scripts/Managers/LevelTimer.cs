using UnityEngine;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Temporizador del nivel. Cuando llega a 0, dispara OnTimeUp.
/// </summary>
public class LevelTimer : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float startTime = 180f; // 3 minutos por defecto
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Eventos")]
    public UnityEvent OnTimeUp;

    private float timeRemaining;
    private bool isRunning = true;

    private void Start()
    {
        timeRemaining = startTime;
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;
        UpdateUI();

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            OnTimeUp?.Invoke();
        }
    }

    private void UpdateUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";

        // Cambia a rojo si quedan menos de 30 segundos
        timerText.color = (timeRemaining <= 30f) ? Color.red : Color.white;
    }

    public void Pause() => isRunning = false;
    public void Resume() => isRunning = true;
    public void AddTime(float seconds) => timeRemaining += seconds;
    public float GetTimeRemaining() => timeRemaining;
}