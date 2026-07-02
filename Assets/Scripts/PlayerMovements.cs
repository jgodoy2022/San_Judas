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
    public InventoryController inventarioController;

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
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                     RigidbodyConstraints.FreezeRotationY | 
                     RigidbodyConstraints.FreezeRotationZ;

        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.notificationBehavior = PlayerNotifications.BroadcastMessages;
        }

        animator = GetComponentInChildren<Animator>();
        Application.targetFrameRate = 60;

        inventarioController = Object.FindFirstObjectByType<InventoryController>();
        if(inventarioController == null)
        {
            Debug.LogError("No se encontró ningún script 'InventoryController' en la escena.");
        }
    }

    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    // --- BOTÓN ÚNICO DE INTERACTUAR ---
    public void OnInteract()
    {
        // Prioridad 1: Recoger objetos interactuables en el sensor
        if (currentInteractable != null)
        {
            OnPickUp(); 
            return; 
        }

        // Prioridad 2: Área de la PUERTA FINAL
        if (estaEnPuertaFinal)
        {
            // Intentamos buscar si el jugador tiene la llave guardada en la barra visual
            if (ConsumirLlaveDesdeBotonFisico())
            {
                AbrirPuertaFinalConExito();
            }
            else
            {
                Debug.Log("La puerta final está cerrada con candado. No tienes la llave en tu inventario.");
            }
            return;
        }

        // Prioridad 3: PUERTA NORMAL
        if (currentDoor != null)
        {
            currentDoor.InteractWithDoor();
            Debug.Log("Puerta normal accionada a través del sistema de triggers.");
            return;
        }
    }

    // Intenta buscar en el contenedor 'Content' si existe la celda de la llave
    private bool ConsumirLlaveDesdeBotonFisico()
    {
        if (inventarioController != null && inventarioController.panelContenedor != null)
        {
            // Recorremos los clones hijos dentro del Content
            foreach (Transform hijo in inventarioController.panelContenedor)
            {
                if (hijo.name.ToLower().Contains("llave"))
                {
                    // Encontró la llave: la borramos del inventario y confirmamos el consumo
                    inventarioController.EliminarCeldaPorNombre(hijo.gameObject);
                    return true;
                }
            }
        }
        return false;
    }

    public void AbrirPuertaFinalConExito()
    {
        Debug.Log("¡Puerta final abierta con éxito! Cargando créditos...");
        SceneManager.LoadScene(escenaCreditos);
    }

    // Función pública para que el InventoryController sepa si estamos tocando el trigger de la puerta
    public bool EstaEnPuertaFinal()
    {
        return estaEnPuertaFinal;
    }

    // --- RECOGER OBJETOS ---
    public void OnPickUp()
    {
        if (currentInteractable != null)
        {
            if (currentInteractable.canBePickedUp)
            {
                if(inventarioController != null && currentInteractable.icon != null)
                {
                    inventarioController.AgregarItemAlInventario(currentInteractable.itemName, currentInteractable.icon);
                }

                Debug.Log($"Recogiste: {currentInteractable.itemName} y se envió a la barra.");
                
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

    // --- BOTÓN DE OBSERVAR / EXAMINAR ---
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

    // --- DETECCIÓN DE PROXIMIDAD POR TRIGGERS ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PuertaFinal"))
        {
            estaEnPuertaFinal = true;
            Debug.Log("Estás frente a la Puerta Final.");
            return;
        }

        DoorController door = other.GetComponent<DoorController>();
        if (door != null)
        {
            currentDoor = door;
            return;
        }

        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log($"Cerca de objeto: {interactable.itemName}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PuertaFinal"))
        {
            estaEnPuertaFinal = false;
            return;
        }

        DoorController door = other.GetComponent<DoorController>();
        if (door != null && currentDoor == door)
        {
            currentDoor = null;
            return;
        }

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
            // Verificamos si hay movimiento significativo
            if (finalInput.magnitude > 0.1f)
            {
                animator.SetBool("IsWalking", true);
                
                // ENVIAMOS LOS DATOS AL BLEND TREE
                animator.SetFloat("MoveX", finalInput.x);
                animator.SetFloat("MoveY", finalInput.y);
            }
            else
            {
                animator.SetBool("IsWalking", false);
                // Si quieres que el personaje mantenga su última dirección al pararse,
                // NO actualices MoveX/MoveY aquí.
            }
        }
    }
}