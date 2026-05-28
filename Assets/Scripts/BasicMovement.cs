using UnityEngine;
using UnityEngine.InputSystem; 

public class BasicMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 5f;

    private Rigidbody rb;
    private Vector2 movementInput;
    private PlayerInput playerInput; // Añadimos esto

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Buscamos el componente Player Input en el mismo objeto
        playerInput = GetComponent<PlayerInput>();
        
        // Súper Hack: Cambiamos el comportamiento por código a "Broadcast Messages"
        // Así no tenemos que arrastrar nada a la pestaña Events
        playerInput.notificationBehavior = PlayerNotifications.BroadcastMessages;
    }

    // Al usar Broadcast Messages, Unity busca automáticamente un método llamado "OnMove"
    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }
}