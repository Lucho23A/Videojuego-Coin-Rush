using UnityEngine;

public class LogoBounce : MonoBehaviour
{
    float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        float escala = 1f + Mathf.Sin(timer * 2f) * 0.04f;
        transform.localScale = new Vector3(escala, escala, 1f);
    }
}
