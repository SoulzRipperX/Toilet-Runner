using UnityEngine;

public class InteractItemPoint : MonoBehaviour
{
    public EntityType type;
    public AudioClip collectSound;

    public enum EntityType {Star, GoldStar, RainbowStar}

    private int pointValue;

    private void Start()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreManager.Instance.AddScore(pointValue);
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

            Destroy(gameObject);
        }
    }
}
