using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public float horizontalSpeed = 7f;
    public float jumpSpeed = 50f;

    private float horizontalMove;
    private bool lookRight = true;

    Rigidbody2D rb;
    public Animator animator;

    public AudioClip walkSound;
    public AudioClip punchSound;
    public AudioClip swordSound;
    public AudioClip jumpSound;
    private AudioSource audioSource;
    private AudioSource audioSourceSFX;

    [Header("Attack Settings")]
    public int punchDamage = 10;
    public int swordDamage = 25;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        audioSourceSFX = gameObject.AddComponent<AudioSource>();
        audioSourceSFX.playOnAwake = false;
        audioSourceSFX.loop = false;
    }

    void Update()
    {
        // Attacks: prefer mobile buttons, ignore mouse clicks over UI
        bool punchInput = false;
        bool swordInput = false;

        // Desktop mouse clicks (SOLO cuando NO estés sobre UI)
        if (!IsPointerOverUIObject())
        {
            if (Input.GetMouseButtonDown(0)) punchInput = true;
            if (Input.GetMouseButtonDown(1)) swordInput = true;
        }

        // Keyboard fallbacks
        if (Input.GetKeyDown(KeyCode.Z)) punchInput = true;
        if (Input.GetKeyDown(KeyCode.X)) swordInput = true;

        // Mobile buttons (tienen prioridad)
        if (MobileControls.Instance != null)
        {
            if (MobileControls.Instance.punchPressed) punchInput = true;
            if (MobileControls.Instance.swordPressed) swordInput = true;
        }

        if (punchInput && Mathf.Abs(horizontalMove) < 0.1f)
        {
            animator.SetBool("AttackPunch", true);
            audioSourceSFX.PlayOneShot(punchSound);
            
            DealDamage(punchDamage);
            
            StartCoroutine(ResetPunchAttack());

            if (MobileControls.Instance != null) MobileControls.Instance.punchPressed = false;
        }

        if (swordInput && Mathf.Abs(horizontalMove) < 0.1f)
        {
            animator.SetBool("AttackSword", true);
            audioSourceSFX.PlayOneShot(swordSound);
            
            DealDamage(swordDamage);
            
            StartCoroutine(ResetSwordAttack());

            if (MobileControls.Instance != null) MobileControls.Instance.swordPressed = false;
        }

        ChangeDirection(horizontalMove);
    }

    // NUEVO MÉTODO: Detectar si el toque está sobre un botón UI
    private bool IsPointerOverUIObject()
    {
        // Verificar si estamos tocando sobre UI en móvil
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return true;
            }
        }

        // Verificar si el mouse está sobre UI en PC
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        return false;
    }

    IEnumerator ResetPunchAttack()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("AttackPunch", false);
    }

    IEnumerator ResetSwordAttack()
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("AttackSword", false);
    }

    // Método para causar daño a enemigos
    private void DealDamage(int damage)
    {
        Vector2 attackPosition = transform.position;
        
        if (lookRight)
        {
            attackPosition.x += attackRange / 2f;
        }
        else
        {
            attackPosition.x -= attackRange / 2f;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            
            if (enemyHealth != null && !enemyHealth.IsDead())
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"¡Golpeaste a {enemy.name} causando {damage} de daño!");
            }
        }
    }

    private void FixedUpdate()
    {
        // Combine keyboard and mobile horizontal input
        float keyboardInput = Input.GetAxis("Horizontal");
        float mobileInput = (MobileControls.Instance != null) ? MobileControls.Instance.GetHorizontalInput() : 0f;
        horizontalMove = keyboardInput + mobileInput;
        horizontalMove = Mathf.Clamp(horizontalMove, -1f, 1f);

        if (SoilCheck.checkCollision)
        { 
            rb.velocity = new Vector2(horizontalSpeed * horizontalMove, 0f);
            animator.SetBool("isWalking", horizontalMove != 0);

            if (horizontalMove != 0)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = walkSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            audioSource.Stop();
        }

        // Jump: keyboard or mobile
        bool jumpInput = Input.GetKey(KeyCode.Space) || (MobileControls.Instance != null && MobileControls.Instance.jumpPressed);
        if (jumpInput && SoilCheck.checkCollision)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            audioSourceSFX.PlayOneShot(jumpSound);
        }

        animator.SetBool("Jump", !SoilCheck.checkCollision);
    }

    public void ChangeDirection(float h)
    {
        if (h > 0 && !lookRight || h < 0 && lookRight)
        {
            lookRight = !lookRight;
            Vector3 turn = transform.localScale;
            turn.x *= -1;
            transform.localScale = turn;
        }
    }

    public void FinishPunchAttack()
    {
        animator.SetBool("AttackPunch", false);
    }

    public void FinishSwordAttack()
    {
        animator.SetBool("AttackSword", false);
    }

    // Dibujar el rango de ataque en el editor (para debugging)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        Vector2 attackPosition = transform.position;
        
        if (lookRight)
        {
            attackPosition.x += attackRange / 2f;
        }
        else
        {
            attackPosition.x -= attackRange / 2f;
        }
        
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
}