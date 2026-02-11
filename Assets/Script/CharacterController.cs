using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [Header("Character Setting")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;

    public float maxStamina = 100f;
    public float staminaRecoveryRate = 5f;

    public float maxHealth = 100f;
    public float healthRecoveryRate = 0f;

    private float currentStamina;
    private float currentHealth;

    private Rigidbody2D rb2d;
    private bool isGrounded;
    private float moveInput;
    private float currentSpeed;

    CharacterController currentCharacter;

    [Header("UI")]
    public GameObject gameOverPanel;
    public Slider healthBar;
    public Slider staminaBar;
    public Image healthFillImage;
    public Image staminaFillImage;

    bool isGameOver;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;

        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;

        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && moveInput != 0;
        currentSpeed = isRunning ? runSpeed : moveSpeed;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }

        if (isRunning)
        {
            currentStamina -= 30f * Time.deltaTime;
        }
        else
        { 
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }




        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaBar.value = currentStamina;

        currentHealth = Mathf.Clamp(currentHealth + healthRecoveryRate * Time.deltaTime, 0, maxHealth);
        healthBar.value = currentHealth;
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(moveInput * currentSpeed, rb2d.velocity.y);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
