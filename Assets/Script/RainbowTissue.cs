using UnityEngine;

public class RainbowTissue : MonoBehaviour
{
    public float speed = 1f;

    private SpriteRenderer spriteRenderer;
    private float hue;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        hue += Time.deltaTime * speed;
        if (hue > 1f) hue -= 1f;

        Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
        spriteRenderer.color = rainbowColor;
    }
}