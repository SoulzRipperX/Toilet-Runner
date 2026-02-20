using UnityEngine;

public class InteractItemHealth : MonoBehaviour
{
    [Header("Heal Settings")]
    public float healAmount = 20f;
    public bool destroyOnPickup = true;

    [Header("Floating Effect")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;
    public float rotateSpeed = 50f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

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
