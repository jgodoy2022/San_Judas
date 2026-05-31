using UnityEngine;
using UnityEngine.InputSystem; 

public class BasicMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 5f;

    private Rigidbody rb;
    private Vector2 movementInput;
    private PlayerInput playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Componente Player Input en el mismo objeto
        playerInput = GetComponent<PlayerInput>();
        
        playerInput.notificationBehavior = PlayerNotifications.BroadcastMessages;


        // 60 FPS estables en Android
        Application.targetFrameRate = 60;
    }

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