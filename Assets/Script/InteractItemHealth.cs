using UnityEngine;

public class InteractItemHealth : MonoBehaviour
{
    [Header("Heal Settings")]
    public float healAmount = 20f;
    public bool destroyOnPickup = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null)
        {
            player.Heal(healAmount);
        }

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
