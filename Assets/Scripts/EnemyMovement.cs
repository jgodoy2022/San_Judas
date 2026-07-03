using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Configuración de Patrulla")]
    public float speed = 1.5f;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Configuración de Persecución")]
    public Transform playerTransform; 
    public bool isChasing = false;

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // Igualamos la estabilidad del jugador
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationY | 
                         RigidbodyConstraints.FreezeRotationZ | 
                         RigidbodyConstraints.FreezePositionY;
    }

    void FixedUpdate()
    {
        // 1. Decidir objetivo
        Vector3 targetPos = (isChasing && playerTransform != null) ? playerTransform.position : waypoints[currentWaypointIndex].position;
        
        // 2. Calcular dirección (ignorando el eje Y para evitar que se mueva hacia arriba/abajo)
        Vector3 flatPosition = new Vector3(transform.position.x, targetPos.y, transform.position.z);
        Vector3 direction = (targetPos - flatPosition).normalized;

        // 3. Chequeo de distancia (Aumentamos el margen a 0.7f por si acaso)
        if (Vector3.Distance(flatPosition, targetPos) > 0.7f) 
        {
            rb.linearVelocity = new Vector3(direction.x, 0f, direction.z) * speed;

            // Animación
            if (animator != null)
            {
                float animX = direction.x;
                if (Mathf.Abs(animX) < 0.1f) animX = (direction.z > 0) ? 0.1f : -0.1f;
                animator.SetFloat("MoveX", animX);
                animator.SetFloat("MoveY", direction.z);
            }
        }
        else if (!isChasing) 
        {
            // Hemos llegado al waypoint
            rb.linearVelocity = Vector3.zero;
            if (animator != null) animator.SetBool("IsWalking", false);
            
            // Cambiar al siguiente waypoint
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
            
            Debug.Log("Llegué al waypoint y cambié al índice: " + currentWaypointIndex);
        }
    }

    // --- Detección Integrada (Ahora el propio enemigo detecta al jugador) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = true;
            playerTransform = other.transform;
            Debug.Log("Jugador detectado por el padre");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
        }
    }
}