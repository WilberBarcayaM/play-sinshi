using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("UI References")]
    [SerializeField] private Image healthBarForeground;
    [SerializeField] private GameObject healthBarCanvas;

    [Header("Health Bar Settings")]
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private Color healthColor = new Color(1f, 0f, 0f, 1f);

    private Animator animator;
    private bool isDead = false;

    void Awake()
    {
        // BUSCAR AUTOMÁTICAMENTE la barra de vida en los hijos de ESTE Demon
        if (healthBarCanvas == null)
        {
            Transform healthBarTransform = transform.Find("HealthBar");
            if (healthBarTransform != null)
            {
                healthBarCanvas = healthBarTransform.gameObject;
                Debug.Log($"[{gameObject.name}] HealthBar encontrado automáticamente");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] No se encontró HealthBar como hijo!");
            }
        }

        if (healthBarForeground == null && healthBarCanvas != null)
        {
            Transform foregroundTransform = healthBarCanvas.transform.Find("Background/Foreground");
            if (foregroundTransform != null)
            {
                healthBarForeground = foregroundTransform.GetComponent<Image>();
                Debug.Log($"[{gameObject.name}] Foreground encontrado automáticamente");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] No se encontró Foreground en HealthBar/Background/Foreground");
            }
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (healthBarForeground != null)
        {
            healthBarForeground.color = healthColor;
        }

        Debug.Log($"[{gameObject.name}] Inicializado con {currentHealth}/{maxHealth} HP");
    }

    // Método para recibir daño
    public void TakeDamage(int damage)
{
    if (isDead) return;

    Debug.Log($"[TakeDamage] {gameObject.name} (ID: {GetInstanceID()})");
    Debug.Log($"[TakeDamage] HealthBar: {healthBarCanvas?.name} (ID: {healthBarCanvas?.GetInstanceID()})");
    Debug.Log($"[TakeDamage] Foreground: {healthBarForeground?.name} (ID: {healthBarForeground?.GetInstanceID()})");

    currentHealth -= damage;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    UpdateHealthBar();

    Debug.Log($"[{gameObject.name}] Recibió {damage} de daño. Vida restante: {currentHealth}/{maxHealth}");

    if (currentHealth <= 0)
    {
        Die();
    }
}

    // Actualizar la barra de vida
    private void UpdateHealthBar()
    {
        if (healthBarForeground != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            healthBarForeground.fillAmount = healthPercent;
        }

        // Mostrar u ocultar la barra según configuración
        if (healthBarCanvas != null && hideWhenFull)
        {
            healthBarCanvas.SetActive(currentHealth < maxHealth);
        }
    }

    // Método de muerte
    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log($"[{gameObject.name}] Ha muerto!");

        // Detener ataques
        StopAllCoroutines();

        // Destruir el enemigo inmediatamente
        Destroy(gameObject);

        // Si usas Object Pooling, usa esto en su lugar:
        // EnemyPool.Instance.ReturnToPool(gameObject);
    }

    // Método para curar (opcional)
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // Getters públicos
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
    public float GetHealthPercent() => (float)currentHealth / maxHealth;
}