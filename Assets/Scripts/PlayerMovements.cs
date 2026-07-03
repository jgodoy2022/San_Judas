using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.SceneManagement; 

public class PlayerMovements : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 2f; 

    [Header("Configuración de Visión")]
    public Transform lightObject; // Arrastra aquí tu Spot Light 2D desde el Inspector
    public float rotSpeed = 15f;  // Velocidad de giro del cono

    [Header("Configuración Final del Juego")]
    public string escenaCreditos = "04_Credits"; 

    [Header("Referencias de UI")]
    public InventoryController inventarioController;

    private Rigidbody rb;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    private Animator animator;

    private DoorController currentDoor;
    private Interactable currentInteractable;
    private bool estaEnPuertaFinal = false;
    private PlayerHealth healthManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationY | 
                         RigidbodyConstraints.FreezeRotationZ;

        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.notificationBehavior = PlayerNotifications.BroadcastMessages;

        animator = GetComponentInChildren<Animator>();
        Application.targetFrameRate = 60;

        inventarioController = Object.FindFirstObjectByType<InventoryController>();

        healthManager = GetComponent<PlayerHealth>();
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

    public void CurarJugador()
    {
        if (healthManager != null)
        {
            // Restauramos al máximo
            healthManager.currentHearts = healthManager.maxHearts;
        
            Debug.Log("¡Salud restaurada al máximo! Corazones actuales: " + healthManager.currentHearts);
        
            // --- AQUÍ ESTÁ EL TRUCO PARA LA UI ---
            // Si tu script UIHealthManager tiene una función de refresco, llámala aquí.
            // Si no, al cambiar el valor de currentHearts, si tu UIHealthManager 
            // está en el Update(), se debería actualizar solo.
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
        // 1. Movimiento del jugador
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        rb.linearVelocity = moveDirection * speed;

        // 2. Rotación de la Linterna
        if (movementInput.magnitude > 0.1f && lightObject != null)
        {
            float targetAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
            // El -90 ajusta el cono para que mire hacia adelante según la dirección
            Quaternion targetRotation = Quaternion.Euler(90f, 0, targetAngle - 90f);
            lightObject.rotation = Quaternion.Slerp(lightObject.rotation, targetRotation, Time.fixedDeltaTime * rotSpeed);
        }

        // 3. Animación
        if (animator != null)
        {
            if (movementInput.magnitude > 0.1f)
            {
                animator.SetBool("IsWalking", true);
                animator.SetFloat("MoveX", movementInput.x);
                animator.SetFloat("MoveY", movementInput.y);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
    }
}