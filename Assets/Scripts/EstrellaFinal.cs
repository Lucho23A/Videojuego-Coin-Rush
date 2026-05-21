using UnityEngine;
using UnityEngine.SceneManagement;
public class EstrellaFinal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance?.AddScore(100);
            SceneManager.LoadScene("LevelSelect");
        }
    }
}