using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("UI References")]
    [SerializeField] private Image healthFillImage; // La imagen FillLife

    [Header("Damage Settings")]
    [SerializeField] private float invulnerabilityTime = 0.5f;
    [SerializeField] private float hitAnimationDuration = 0.4f; // Duración de la animación Hit
    private bool isInvulnerable = false;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Método para recibir daño (25% por golpe = 25 de daño)
    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateHealthUI();
        
        // Iniciar invulnerabilidad temporal
        StartCoroutine(InvulnerabilityCoroutine());

        // Reproducir animación de golpe (HIt)
        if (currentHealth > 0 && animator != null)
        {
            StartCoroutine(PlayHitAnimation());
        }

        Debug.Log($"Jugador recibió {damage} de daño. Vida restante: {currentHealth}/{maxHealth} ({GetHealthPercent() * 100:F0}%)");

        // Morir si la vida llega a 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Corrutina para manejar la animación de Hit
    private IEnumerator PlayHitAnimation()
    {
        // Activar animación de Hit
        animator.SetBool("Hit", true);
        
        // Esperar la duración de la animación
        yield return new WaitForSeconds(hitAnimationDuration);
        
        // Desactivar animación de Hit para volver a Idle/Walk
        animator.SetBool("Hit", false);
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    // Método para curar
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        
        Debug.Log($"Jugador curado {amount} HP. Vida actual: {currentHealth}/{maxHealth}");
    }

    // Actualizar la barra de vida con Fill Amount
    private void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            // Calcular el porcentaje de vida (0.0 a 1.0)
            float healthPercent = (float)currentHealth / maxHealth;
            
            // Actualizar el Fill Amount (esto reduce la barra horizontalmente)
            healthFillImage.fillAmount = healthPercent;
            
            // NO cambiamos el color, solo se reduce la barra verde
        }
    }

    // Método de muerte
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        Debug.Log("¡El jugador ha muerto!");

        // Reproducir animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Desactivar controles
        PlayerMove playerMove = GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.enabled = false;
        }

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // yield return new WaitForSeconds(2f);
        // Debug.Log("GAME OVER");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("GAME OVER");

        // ✅ Mostrar pantalla de Game Over
        if (GameOver.Instance != null)
            GameOver.Instance.ShowGameOver();
        else
            Debug.LogWarning("No se encontró GameOver en la escena");
    }

    // Método para reiniciar
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        isInvulnerable = false;
        UpdateHealthUI();
        
        PlayerMove playerMove = GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.enabled = true;
        }
        
        if (animator != null)
        {
            animator.SetBool("Hit", false);
        }
    }

    // Getters públicos
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
    public bool IsInvulnerable() => isInvulnerable;
    public float GetHealthPercent() => (float)currentHealth / maxHealth;
}