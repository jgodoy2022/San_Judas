using UnityEngine;
using UnityEngine.InputSystem; 

public class DoorController : MonoBehaviour
{
    public enum DoorSide { Right, Left }

    [Header("Referencias")]
    public Transform doorMesh;      // Aquí se arrastra la hoja 'Door'

    [Header("Configuración de Animación")]
    public DoorSide doorSide = DoorSide.Right; // Elegimos el lado en el Inspector
    public float openAngle = 90f;   // Ángulo de apertura deseado
    public float speed = 3f;        

    private bool isOpen = false;
    private bool isPlayerNearby = false; 
    
    private Quaternion defaultRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Forzamos 60 FPS por si acaso
        Application.targetFrameRate = 60;

        if (doorMesh != null)
        {
            // Guarda la rotación inicial tal cual está en la escena
            defaultRotation = doorMesh.localRotation;
            
            // Si es la puerta izquierda, invertimos el ángulo multiplicando por -1
            float finalAngle = (doorSide == DoorSide.Left) ? -openAngle : openAngle;
            
            // Calcula el destino exacto en base a su dirección
            targetRotation = defaultRotation * Quaternion.Euler(0, finalAngle, 0);
        }
    }

    void Update()
    {
        if (doorMesh == null) return;

        // --- SOLUCIÓN INPUT MÓVIL ---
        // Al usar el New Input System, esta línea detecta TANTO la tecla 'E' en PC 
        // como cualquier botón virtual táctil que configuremos en Android.
        bool interactionPressed = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame);

        if (isPlayerNearby && interactionPressed)
        {
            isOpen = !isOpen; 
        }

        // Aplica la rotación suavizada
        if (isOpen)
        {
            doorMesh.localRotation = Quaternion.Slerp(doorMesh.localRotation, targetRotation, Time.deltaTime * speed);
        }
        else
        {
            doorMesh.localRotation = Quaternion.Slerp(doorMesh.localRotation, defaultRotation, Time.deltaTime * speed);
        }
    }

    // Método público para que el botón de la UI de Android pueda abrir la puerta al tocarlo
    public void InteractWithDoor()
    {
        if (isPlayerNearby)
        {
            isOpen = !isOpen;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovements>() != null)
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovements>() != null)
        {
            isPlayerNearby = false;
        }
    }
}