using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.SceneManagement; 

public class PlayerMovements : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 2f; 

    [Header("Configuración Final del Juego")]
    public string escenaCreditos = "04_Credits"; 

    [Header("Referencias de UI (se asigna sola al iniciar)")]
    public InventarioUI inventarioUI;

    // --- VARIABLES DE INVENTARIO INTERNO ---
    [HideInInspector] public bool tieneObjetoKey = false; 

    private Rigidbody rb;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    private Animator animator;

    // Sensores de proximidad actuales
    private DoorController currentDoor;
    private Interactable currentInteractable;
    private bool estaEnPuertaFinal = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.notificationBehavior = PlayerNotifications.BroadcastMessages;
        }

        animator = GetComponentInChildren<Animator>();
        Application.targetFrameRate = 60;

        // Jugador busca el componente InventarioUI en la escena por sí mismo
        inventarioUI = Object.FindFirstObjectByType<InventarioUI>();
        if(inventarioUI == null)
        {
            Debug.LogError("No se encontró ningún script 'InventarioUI en la escena. Asegurate de tenerlo pegado en tu CANVAS.");
        }
    }

    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    // --- BOTÓN ÚNICO DE INTERACTUAR ---
    // Este método maneja TODO: recoger llaves, abrir la puerta final o abrir puertas normales
    public void OnInteract()
    {
        // Prioridad 1: Si hay un objeto interactuable (como la llave) en nuestro sensor, lo recogemos de inmediato
        if (currentInteractable != null)
        {
            OnPickUp(); // Esto ejecuta la recolección, guarda la llave y destruye la esfera
            return; // Cortamos el código aquí para que no intente hacer nada más en este clic
        }

        // Prioridad 2: Si no hay objetos pero estamos en el área de la PUERTA FINAL
        if (estaEnPuertaFinal)
        {
            IntentarAbrirPuertaFinal();
            return;
        }

        // Prioridad 3: Si no hay nada de lo anterior pero estamos en una PUERTA NORMAL
        if (currentDoor != null)
        {
            currentDoor.InteractWithDoor();
            Debug.Log("Puerta normal accionada a través del sistema de triggers.");
            return;
        }
    }

    // --- BOTÓN DE RECOGER OBJETOS EN LA UI (Ahora por SENSOR) ---
    public void OnPickUp()
    {
        // Si estamos pisando el sensor de un objeto interactuable
        if (currentInteractable != null)
        {
            if (currentInteractable.canBePickedUp)
            {
                // 1. intentar añadirlo primero visualmente al inventario
                if(inventarioUI != null && currentInteractable.icon != null)
                {
                    bool seGuardo = inventarioUI.AgregarItemAlInventario(currentInteractable.icon);

                    // si inventario lleno, se frena la recolección
                    if(!seGuardo) return;
                }

                // 2. lógica 
                string nombreObjeto = currentInteractable.itemName.ToLower();

                // Comprobamos si es la llave
                if (nombreObjeto.Contains("llave") || nombreObjeto.Contains("key") || nombreObjeto.Contains("judas"))
                {
                    tieneObjetoKey = true;
                    Debug.Log("¡Llave guardada en el inventario a través del sensor!");
                }

                Debug.Log($"Recogiste: {currentInteractable.itemName}");
                
                // Guardamos una referencia temporal para destruirlo y limpiamos el sensor antes
                GameObject objetoAEliminar = currentInteractable.gameObject;
                currentInteractable = null; 
                
                Destroy(objetoAEliminar);
            }
        }
        else
        {
            Debug.Log("No hay ningún objeto cerca para recoger.");
        }
    }

    // --- BOTÓN DE OBSERVAR / EXAMINAR EN LA UI (Ahora por SENSOR) ---
    public void OnObserve()
    {
        if (currentInteractable != null)
        {
            Debug.Log($"Examinando por sensor: {currentInteractable.itemName} - {currentInteractable.description}");
        }
        else
        {
            Debug.Log("No hay ningún objeto cerca para examinar.");
        }
    }

    private void IntentarAbrirPuertaFinal()
    {
        if (tieneObjetoKey)
        {
            Debug.Log("¡Puerta final abierta con éxito! Cargando créditos...");
            SceneManager.LoadScene(escenaCreditos);
        }
        else
        {
            Debug.Log("La puerta final está cerrada con candado. Necesitas encontrar el objeto clave.");
        }
    }

    // --- DETECCIÓN DE PROXIMIDAD POR TRIGGERS (Puertas y Objetos) ---
    private void OnTriggerEnter(Collider other)
    {
        // 1. Detectar Puerta Final
        if (other.CompareTag("PuertaFinal"))
        {
            estaEnPuertaFinal = true;
            Debug.Log("Estás frente a la Puerta Final.");
            return;
        }

        // 2. Detectar Puerta Normal
        DoorController door = other.GetComponent<DoorController>();
        if (door != null)
        {
            currentDoor = door;
            return;
        }

        // 3. Detectar Objeto Interactuable (Llaves, Notas, etc.)
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log($"Cerca de objeto: {interactable.itemName}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 1. Salir de Puerta Final
        if (other.CompareTag("PuertaFinal"))
        {
            estaEnPuertaFinal = false;
            return;
        }

        // 2. Salir de Puerta Normal
        DoorController door = other.GetComponent<DoorController>();
        if (door != null && currentDoor == door)
        {
            currentDoor = null;
            return;
        }

        // 3. Salir de Objeto Interactuable
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null && currentInteractable == interactable)
        {
            currentInteractable = null;
        }
    }

    void FixedUpdate()
    {
        Vector2 finalInput = movementInput;
        Vector3 moveDirection = new Vector3(finalInput.x, 0f, finalInput.y).normalized;
        
        rb.linearVelocity = moveDirection * speed;

        if (animator != null)
        {
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
    }

    private int GetDirection(Vector2 input)
    {
        if (animator == null) return 0;
        if (input.x > 0.1f && input.y < -0.1f) return 0; // SE
        if (input.x < -0.1f && input.y < -0.1f) return 1; // SW
        if (input.x < -0.1f && input.y > 0.1f) return 2; // NW
        if (input.x > 0.1f && input.y > 0.1f) return 3; // NE
        return animator.GetInteger("Direction");
    }
}