using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 5f;
    
    [Header("Joystick Virtual (Opcional)")]
    public Joystick floatingJoystick; // Arrastra tu joystick aquí
    
    private Rigidbody rb;
    private Animator animator;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        
        if (playerInput != null)
            playerInput.notificationBehavior = PlayerNotifications.BroadcastMessages;
        
        Application.targetFrameRate = 60;
    }
    
    // Lo llama el Input System con el teclado
    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }
    
    void FixedUpdate()
    {

        Vector2 finalInput = movementInput;
        // --- MEZCLA DE INPUTS ---
        if (floatingJoystick != null)
        {
            Vector2 joystickInput = new Vector2(floatingJoystick.Horizontal, floatingJoystick.Vertical);
            
            // Debug para ver qué está leyendo del joystick
            Debug.Log("Joystick Horizontal: " + floatingJoystick.Horizontal + " Vertical: " + floatingJoystick.Vertical);
            
            if (joystickInput.magnitude > 0.1f)
            {
                finalInput = joystickInput;
                movementInput = joystickInput;
                Debug.Log("¡Usando joystick! Dirección: " + finalInput);
            }
        }
        else
        {
            Debug.LogError("¡No hay joystick asignado en el inspector!");
        }
        // --- FIN DE LA MEZCLA ---
        
        Vector3 moveDirection = new Vector3(finalInput.x, 0f, finalInput.y).normalized;
        
        // Movimiento con velocity (funciona mejor en móvil y con joystick virtual)
        rb.linearVelocity = moveDirection * speed;
        
        // Animaciones
        if (finalInput.magnitude > 0.1f)
        {
            animator.SetBool("IsWalking", true);
            int direction = GetDirection(finalInput);
            animator.SetInteger("Direction", direction);
        }
        else
        {
            animator.SetBool("IsWalking", false);
            rb.linearVelocity = Vector3.zero;
        }
    }
    
    private int GetDirection(Vector2 input)
    {
        // Añadimos umbrales para mayor precisión con joystick
        if (input.x > 0.1f && input.y < -0.1f) return 0; // SE
        if (input.x < -0.1f && input.y < -0.1f) return 1; // SW
        if (input.x < -0.1f && input.y > 0.1f) return 2; // NW
        if (input.x > 0.1f && input.y > 0.1f) return 3; // NE
        
        return animator.GetInteger("Direction");
    }
}