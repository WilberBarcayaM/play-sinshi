using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // El jugador
    
    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f); // Offset de la cámara
    [SerializeField] private float smoothSpeed = 0.125f; // Velocidad de suavizado (0 = instantáneo, 1 = muy lento)
    [SerializeField] private bool smoothFollow = true; // Seguimiento suave o instantáneo
    
    [Header("Bounds Settings (Opcional)")]
    [SerializeField] private bool useBounds = false; // Limitar movimiento de cámara
    [SerializeField] private Vector2 minBounds; // Límite inferior izquierdo
    [SerializeField] private Vector2 maxBounds; // Límite superior derecho

    void Start()
    {
        // Si no se asignó un target, buscar automáticamente al jugador
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("[CameraFollow] Player encontrado automáticamente");
            }
            else
            {
                Debug.LogError("[CameraFollow] No se encontró un GameObject con tag 'Player'");
            }
        }

        // Establecer posición inicial
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calcular posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Aplicar límites si están activados
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }

        // Seguimiento suave o instantáneo
        if (smoothFollow)
        {
            // Interpolación suave (Lerp)
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else
        {
            // Seguimiento instantáneo
            transform.position = desiredPosition;
        }
    }

    // Dibujar los límites en el editor
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.yellow;
        
        // Dibujar rectángulo de límites
        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0);
        Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0);
        Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}