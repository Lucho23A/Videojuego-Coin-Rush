using UnityEngine;
using UnityEngine.UI;

public class StarBlink : MonoBehaviour
{
    Image img;
    float speed;
    float offset;

    void Start()
    {
        img    = GetComponent<Image>();
        speed  = Random.Range(1f, 3f);
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float alpha = (Mathf.Sin(Time.time * speed + offset) + 1f) / 2f;
        img.color   = new Color(1f, 1f, 0.6f, alpha * 0.9f + 0.1f);
        float scale = 0.8f + alpha * 0.4f;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
