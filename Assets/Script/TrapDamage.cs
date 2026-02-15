using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public float damage = 50f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}
