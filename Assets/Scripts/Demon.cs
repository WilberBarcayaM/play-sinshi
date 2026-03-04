using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public float attackInterval = 3f;
    public float attackDuration = 1f;
    public int attackDamage = 20; // Cambiado de 20 a 25 (25% de daño por golpe)

    [Header("Rango de Ataque")]
    public float attackRange = 3f; // Tu rango actual

    [Header("Referencias")]
    public Animator animator;
    
    [Header("Audio")]
    public AudioClip attackSound;
    private AudioSource audioSource;

    private bool isAttacking = false;
    private Transform playerTransform;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (animator == null)
        {
            Debug.LogError("No se encontró Animator en el Demon!");
            return;
        }

        // Buscar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró el jugador con tag 'Player'");
        }

        StartCoroutine(AttackCycle());
    }

    IEnumerator AttackCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            PerformAttack();
            yield return new WaitForSeconds(attackDuration);
            ResetToIdle();
        }
    }

    void PerformAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("IsAttacking", true);
            // DEBUG: Verificar el daño configurado
        Debug.Log($"[DEBUG] Demon atacando con {attackDamage} de daño");
        
            
            // Reproducir sonido de ataque
            if (attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            // Causar daño al jugador si está en rango
            DamagePlayer();
        }
    }

    void DamagePlayer()
    {
        if (playerTransform == null)
        {
            // Intentar encontrar al jugador si no lo tenemos
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                return;
            }
        }

        // Calcular distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Si el jugador está dentro del rango de ataque
        if (distanceToPlayer <= attackRange)
        {
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
            
            if (playerHealth != null && !playerHealth.IsDead())
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"¡Demon golpeó al jugador! Daño: {attackDamage} HP (distancia: {distanceToPlayer:F2})");
            }
        }
        else
        {
            Debug.Log($"Jugador fuera de rango. Distancia: {distanceToPlayer:F2} / Rango: {attackRange}");
        }
    }

    void ResetToIdle()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    public void FinishAttack()
    {
        ResetToIdle();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Demon recibió {damage} de daño");
    }

    // Dibujar el rango de ataque en el editor (útil para debugging)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}