using UnityEngine;
using UnityEngine.InputSystem; 

public class BasicMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 5f;

    private Rigidbody rb;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Componente Player Input en el mismo objeto
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();

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
        Vector3 moveDirection = new Vector3(
            movementInput.x, 
            0f, 
            movementInput.y
            ).normalized;

        rb.MovePosition(
            rb.position + moveDirection * speed * Time.fixedDeltaTime
            );

        if(movementInput != Vector2.zero)
        {
            Debug.Log("Moviendo - Direction: " + GetDirection(movementInput));
            animator.SetBool("IsWalking", true);
            int direction = GetDirection(movementInput);
            animator.SetInteger("Direction", direction);
            
        }
        else{
            animator.SetBool("IsWalking", false);
        }

        if(animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log("ESTADO ACTUAL: " + state.fullPathHash + " - IsWalking " + animator.GetBool("IsWalking"));
        }
    }

    private int GetDirection(Vector2 input)
    {
        if (input.x > 0 && input.y < 0) return 0; // SE (walk_1)
        if (input.x < 0 && input.y < 0) return 1; // SW (walk_2)
        if (input.x < 0 && input.y > 0) return 2; // NW (walk_3)
        if (input.x > 0 && input.y > 0) return 3; // NE (walk_4)

        return animator.GetInteger("Direction");
    }
}