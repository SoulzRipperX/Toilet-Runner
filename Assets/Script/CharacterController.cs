using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;

    [Header("Combat Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;
    public float attackDamage = 20f;
    public float attackCooldown = 0.5f;

    [Header("Stats")]
    public float maxStamina = 100f;
    public float staminaRecoveryRate = 10f;
    public float maxHealth = 100f;
    public float healthRecoveryRate = 5f;
    public float healthRegenDelay = 3f;

    [Header("UI")]
    public GameObject mobileUI;
    public GameObject gameOverPanel;
    public Slider healthBar;
    public Slider staminaBar;
    public Toggle mobileUIToggle;

    private float currentStamina;
    private float currentHealth;

    private Rigidbody2D rb2d;
    private Animator animator;

    private bool isGrounded;
    private bool isGameOver;

    private float horizontalInput;
    private float mobileMoveInput;
    private bool isMobileSprint;

    private float currentSpeed;
    private Vector3 originalScale;

    private float lastAttackTime;
    private float lastDamageTime;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;

        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthBar)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (staminaBar)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }

        if (gameOverPanel)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        if (mobileUIToggle != null)
        {
            mobileUIToggle.onValueChanged.AddListener(SetMobileUI);
            SetMobileUI(mobileUIToggle.isOn);
        }
        else if (mobileUI != null)
        {
            mobileUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isGameOver) return;

        HandleInput();
        HandleStamina();
        HandleHealthRegen();
        UpdateAnimation();
        UpdateUI();
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(horizontalInput * currentSpeed, rb2d.velocity.y);
    }

    void HandleInput()
    {
        float keyboardH = Input.GetAxisRaw("Horizontal");
        horizontalInput = (keyboardH != 0) ? keyboardH : mobileMoveInput;

        if (horizontalInput > 0)
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

        bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || isMobileSprint)
                           && currentStamina > 1f
                           && horizontalInput != 0;

        currentSpeed = isSprinting ? runSpeed : moveSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
            HandleJump();

        if (Input.GetKeyDown(KeyCode.J))
            HandleAttack();
    }

    void HandleAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        animator.SetTrigger("Attack");

        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
                enemyScript.TakeDamage(attackDamage);
        }
    }

    void HandleJump()
    {
        if (!isGrounded) return;

        isGrounded = false;

        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        AudioManager.instance.PlaySFX(AudioManager.instance.jumpClip);
        animator.SetTrigger("Jump");
    }

    void HandleStamina()
    {
        bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || isMobileSprint)
                           && horizontalInput != 0
                           && currentStamina > 1f;

        if (isSprinting)
            currentStamina -= 30f * Time.deltaTime;
        else
            currentStamina += staminaRecoveryRate * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void HandleHealthRegen()
    {
        if (currentHealth <= 0) return;

        if (Time.time - lastDamageTime >= healthRegenDelay)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += healthRecoveryRate * Time.deltaTime;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            }
        }
    }

    void UpdateAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("isGrounded", isGrounded);
    }

    void UpdateUI()
    {
        if (staminaBar)
            staminaBar.value = currentStamina;

        if (healthBar)
            healthBar.value = currentHealth;
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        lastDamageTime = Time.time;

        animator.SetTrigger("Hit");

        UpdateUI();

        if (currentHealth <= 0)
            GameOver();
    }

    void GameOver()
    {
        isGameOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.instance.StopBGM();
        AudioManager.instance.PlaySFX(AudioManager.instance.loseClip);
    }

    public void SetMobileUI(bool state)
    {
        if (mobileUI == null) return;
        mobileUI.SetActive(state);
    }

    public void MobileJump() => HandleJump();
    public void MobileAttack() => HandleAttack();
    public void MoveLeftDown() => mobileMoveInput = -1f;
    public void MoveRightDown() => mobileMoveInput = 1f;
    public void MoveStop() => mobileMoveInput = 0f;
    public void RunDown() => isMobileSprint = true;
    public void RunUp() => isMobileSprint = false;

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
