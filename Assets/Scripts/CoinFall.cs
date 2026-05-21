using UnityEngine;

public class CoinFall : MonoBehaviour
{
    public float speed  = 150f;
    public float delay  = 0f;
    public float startX = 0f;

    RectTransform rt;
    float timer  = 0f;
    bool  active = false;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(startX, 600f);
    }

    void Update()
    {
        if (!active)
        {
            timer += Time.deltaTime;
            if (timer >= delay) active = true;
            return;
        }

        rt.anchoredPosition += new Vector2(
            Mathf.Sin(Time.time + startX) * 30f * Time.deltaTime,
            -speed * Time.deltaTime);

        transform.Rotate(0, 0, 180f * Time.deltaTime);

        if (rt.anchoredPosition.y < -650f)
            rt.anchoredPosition = new Vector2(startX, 600f);
    }
}
