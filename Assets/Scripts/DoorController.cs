using UnityEngine;
using UnityEngine.InputSystem; 

public class DoorController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform doorMesh;      // Aquí se arrastra la hoja 'Door'

    [Header("Configuración de Animación")]
    public float openAngle = 90f;   // Ángulo de apertura en horizontal
    public float speed = 3f;        

    private bool isOpen = false;
    private bool isPlayerNearby = false; 
    
    private Quaternion defaultRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Guarda la rotación inicial tal cual como pusiste la puerta en la escena
        if (doorMesh != null)
        {
            defaultRotation = doorMesh.localRotation;
            targetRotation = defaultRotation * Quaternion.Euler(0, openAngle, 0);
        }
    }

    void Update()
    {
        if (doorMesh == null) return;

        // Detecta la interacción con la tecla E
        if (isPlayerNearby && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            isOpen = !isOpen; 
        }

        // Aplica la rotación suavizada en el eje Y local
        if (isOpen)
        {
            doorMesh.localRotation = Quaternion.Slerp(doorMesh.localRotation, targetRotation, Time.deltaTime * speed);
        }
        else
        {
            doorMesh.localRotation = Quaternion.Slerp(doorMesh.localRotation, defaultRotation, Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BasicMovement>() != null)
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<BasicMovement>() != null)
        {
            isPlayerNearby = false;
        }
    }
}