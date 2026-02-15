using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float damage = 10f;

    [Header("AI Logic")]
    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float wanderRange = 3f;

    private Transform player;
    private Vector2 targetPosition;
    private float wanderTimer;
    private float attackCooldown;
    private bool isChasing;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        SetNewWanderTarget();
    }

    void Update()
    {
        if (health <= 0) return;

        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }

        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
    }

    void Wander()
    {
        isChasing = false;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= 2f)
            {
                SetNewWanderTarget();
                wanderTimer = 0;
            }
        }
    }

    void SetNewWanderTarget()
    {
        float randomX = Random.Range(-wanderRange, wanderRange);
        targetPosition = new Vector2(transform.position.x + randomX, transform.position.y);
    }

    void ChasePlayer()
    {
        isChasing = true;
        Vector2 direction = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, direction, chaseSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        if (attackCooldown <= 0)
        {
            Debug.Log("ศัตรูโจมตีผู้เล่น!");
            CharacterController playerScript = player.GetComponent<CharacterController>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
            }
            attackCooldown = 1.5f;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0) Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}