using UnityEngine;
using System.Collections;

public class InteractItemScore : MonoBehaviour
{
    public EntityType type;
    public AudioClip collectSound;

    public enum EntityType { Star, GoldStar, RainbowStar }

    private int pointValue;

    private Vector3 startPos;
    private bool isCollected = false;

    [Header("Floating Effect")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;
    public float rotateSpeed = 50f;

    private void Start()
    {
        startPos = transform.position;

        switch (type)
        {
            case EntityType.Star:
                pointValue = 1;
                break;
            case EntityType.GoldStar:
                pointValue = 3;
                break;
            case EntityType.RainbowStar:
                pointValue = 5;
                break;
        }
    }

    private void Update()
    {
        if (isCollected) return;

        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            ScoreManager.Instance.AddScore(pointValue);

            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            StartCoroutine(CollectEffect());
        }
    }

    IEnumerator CollectEffect()
    {
        float duration = 0.2f;
        float time = 0;

        Vector3 originalScale = transform.localScale;

        while (time < duration)
        {
            float t = time / duration;

            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
