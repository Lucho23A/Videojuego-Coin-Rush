using UnityEngine;
using UnityEngine.UI;

public class ButtonGlow : MonoBehaviour
{
    Image img;
    Color baseColor;
    float speed = 2f;

    void Start()
    {
        img       = GetComponent<Image>();
        baseColor = img.color;
    }

    void Update()
    {
        float glow = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        img.color  = Color.Lerp(baseColor, baseColor * 1.3f, glow * 0.3f);
    }
}

