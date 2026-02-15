using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;

    [Header("Stats")]
    public float maxStamina = 100f;
    public float staminaRecoveryRate = 10f;
    public float maxHealth = 100f;

    [Header("UI")]
    public GameObject mobileUI;
    public GameObject gameOverPanel;
    public Slider healthBar;
    public Slider staminaBar;

    private float currentStamina;
    private float currentHealth;
    private Rigidbody2D rb2d;
    private Animator animator;
    private bool isGrounded;
    private bool isGameOver;

    private float horizontalInput;
    private float mobileMoveInput;
    private bool isMobileSprint;
    private Vector3 originalScale;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;

        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthBar) healthBar.maxValue = maxHealth;
        if (staminaBar) staminaBar.maxValue = maxStamina;
        if (gameOverPanel) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        if (mobileUI) mobileUI.SetActive(true);
#endif
    }

    void Update()
    {
        if (isGameOver) return;

        float keyboardH = Input.GetAxisRaw("Horizontal");
        horizontalInput = (keyboardH != 0) ? keyboardH : mobileMoveInput;

        if (horizontalInput > 0)
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

        bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || isMobileSprint) && currentStamina > 1f && horizontalInput != 0;
        float speed = isSprinting ? runSpeed : moveSpeed;

        if (Input.GetKeyDown(KeyCode.Space)) HandleJump();
        if (Input.GetKeyDown(KeyCode.J)) HandleAttack();

        rb2d.velocity = new Vector2(horizontalInput * speed, rb2d.velocity.y);

        if (isSprinting)
            currentStamina -= 30f * Time.deltaTime;
        else
            currentStamina += staminaRecoveryRate * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        UpdateUI();
        UpdateAnimation(isSprinting);
    }

    public void HandleJump()
    {
        if (isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }
    }

    public void HandleAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void MoveLeftDown() => mobileMoveInput = -1f;
    public void MoveRightDown() => mobileMoveInput = 1f;
    public void MoveStop() => mobileMoveInput = 0f;

    public void RunDown() => isMobileSprint = true;
    public void RunUp() => isMobileSprint = false;


    void UpdateAnimation(bool isSprinting)
    {
        float animSpeed = 0;
        if (horizontalInput != 0)
            animSpeed = isSprinting ? 10f : 5f;

        animator.SetFloat("Speed", animSpeed);
        animator.SetBool("isGrounded", isGrounded);
    }

    void UpdateUI()
    {
        if (staminaBar) staminaBar.value = currentStamina;
        if (healthBar) healthBar.value = currentHealth;
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
        if (currentHealth <= 0) GameOver();
    }

    void GameOver()
    {
        isGameOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}