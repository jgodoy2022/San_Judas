using UnityEngine;
using UnityEngine.InputSystem; 

public class DoorController : MonoBehaviour
{
    public enum DoorSide { Right, Left }

    [Header("Referencias")]
    public Transform doorMesh;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioChirrido; 

    [Header("Configuración de Animación")]
    public DoorSide doorSide = DoorSide.Right;
    public float openAngle = 90f;
    public float speed = 3f;        

    private bool isOpen = false;
    private bool isPlayerNearby = false; 
    public bool estaBloqueada = false;
    public bool esLaPrimeraPuerta = false;
    
    
    private Quaternion defaultRotation;
    private Quaternion targetRotation;

    void Start()
    {
        Application.targetFrameRate = 60;

        if (doorMesh != null)
        {
            defaultRotation = doorMesh.localRotation;
            float finalAngle = (doorSide == DoorSide.Left) ? -openAngle : openAngle;
            targetRotation = defaultRotation * Quaternion.Euler(0, finalAngle, 0);
        }
    }

    void Update()
    {
        if (doorMesh == null) return;

        bool interactionPressed = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame);

        if (isPlayerNearby && interactionPressed)
        {
            InteractWithDoor();
        }

        if (isOpen)
        {
            doorMesh.localRotation = Quaternion.Slerp(doorMesh.localRotation, targetRotation, Time.deltaTime * speed);
        }
        else
        {
            doorMesh.localRotation = Quaternion.Slerp(doorMesh.localRotation, defaultRotation, Time.deltaTime * speed);
        }
    }

    public void InteractWithDoor()
    {
        if (estaBloqueada)
        {
            DialogManager.instance.ShowMessage("Esta puerta está cerrada con llave.");
            return;
        }

        if (esLaPrimeraPuerta) 
        {
            // Llamamos a un método que no tiene parámetros
            Invoke("MostrarDialogoPuerta", 3.0f);
            Invoke("MostrarDialogoDuda1", 10.0f);
            Invoke("MostrarDialogoDuda2", 12.0f);
            Invoke("MostrarDialogoObjetivo", 15.0f);
        }
    
        isOpen = !isOpen;

        // Reproducimos el audio cada vez que se cambia el estado de isOpen
        if (audioSource != null && audioChirrido != null)
        {
            audioSource.PlayOneShot(audioChirrido);
        }
    }

    void MostrarDialogoPuerta()
    {
        DialogManager.instance.ShowMessage("¿Dónde están todos?");
    }

    void MostrarDialogoDuda1()
    {
        DialogManager.instance.ShowMessage("Esto...");
    }

    void MostrarDialogoDuda2()
    {
        DialogManager.instance.ShowMessage("No recuerdo qué sucedió...");
    }

    void MostrarDialogoObjetivo()
    {
        DialogManager.instance.ShowMessage("!Tengo que salir de aquí!!!");
    }

    public void DesbloquearPuerta()
    {
        estaBloqueada = false;
        Debug.Log("¡Has desbloqueado la puerta!");
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