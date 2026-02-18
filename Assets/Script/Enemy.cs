using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float damage = 10f;

    public Transform attackPoint;
    public float attackRadius = 0.6f;
    public LayerMask playerLayer;
    public float attackCooldownTime = 1.5f;

    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float wanderRange = 3f;

    private Transform player;
    private Vector2 startPosition;
    private Vector2 targetPosition;

    private float attackCooldown;
    private float stateTimer;

    private Animator animator;

    private enum State { Idle, Wander, Chase, Attack, Hit }
    private State currentState;

    private bool facingRight = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        startPosition = transform.position;
        SetNewWanderTarget();
        currentState = State.Wander;
    }

    void Update()
    {
        if (health <= 0) return;

        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        float distance = player != null
            ? Vector2.Distance(transform.position, player.position)
            : Mathf.Infinity;

        switch (currentState)
        {
            case State.Wander:
                Wander();
                if (distance <= detectionRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                Chase();
                if (distance <= attackRange)
                    currentState = State.Attack;
                else if (distance > detectionRange)
                    currentState = State.Wander;
                break;

            case State.Attack:
                TryAttack();
                break;

            case State.Hit:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                    currentState = State.Wander;
                break;
        }
    }

    void Wander()
    {
        animator.SetFloat("Speed", moveSpeed);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewWanderTarget();
        }
    }

    void SetNewWanderTarget()
    {
        float randomX = Random.Range(-wanderRange, wanderRange);
        targetPosition = new Vector2(startPosition.x + randomX, transform.position.y);
        Flip(targetPosition.x > transform.position.x);
    }

    void Chase()
    {
        animator.SetFloat("Speed", chaseSpeed);

        Vector2 target = new Vector2(player.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            chaseSpeed * Time.deltaTime);

        Flip(player.position.x > transform.position.x);
    }

    void TryAttack()
    {
        if (attackCooldown > 0)
        {
            currentState = State.Chase;
            return;
        }

        animator.SetTrigger("Attack");
        attackCooldown = attackCooldownTime;
        stateTimer = 0.5f;
        currentState = State.Chase;
    }

    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer);

        if (hit != null)
        {
            var p = hit.GetComponent<CharacterController>();
            if (p != null)
                p.TakeDamage(damage);
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        animator.SetTrigger("Hit");
        currentState = State.Hit;
        stateTimer = 0.3f;

        if (health <= 0)
            Destroy(gameObject);
    }

    void Flip(bool shouldFaceRight)
    {
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
