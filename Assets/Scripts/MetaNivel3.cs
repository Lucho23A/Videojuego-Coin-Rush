using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaNivel3 : MonoBehaviour
{
    public float tiempoEspera = 3f;
    private bool yaGano = false;

    private void OnTriggerEnter(Collider other)
    {
        if (yaGano) return;
        if (!other.CompareTag("Player")) return;

        yaGano = true;

        GameObject panel = GameObject.Find("PanelGanaste");
        if (panel != null)
            panel.SetActive(true);

        Invoke(nameof(IrAHighscores), tiempoEspera);
    }

    void IrAHighscores()
    {
        SceneManager.LoadScene("Highscores");
    }
}