using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 2f;
    public LayerMask interactableLayer;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    // Para botón "Observar"
    public void OnObserve()
    {
        TryInteract(false);
    }

    // Para botón "Recoger"
    public void OnPickUp()
    {
        TryInteract(true);
    }

    private void TryInteract(bool isPickingUp)
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (isPickingUp && interactable.canBePickedUp)
                {
                    PickUpItem(interactable);
                }
                else
                {
                    ExamineItem(interactable);
                }
            }
        }
    }

    private void PickUpItem(Interactable item)
    {
        Debug.Log($"Recogiste: {item.itemName}");
        // Aquí iría el inventario
        Destroy(item.gameObject);
    }

    private void ExamineItem(Interactable item)
    {
        Debug.Log($"Examinando: {item.itemName} - {item.description}");
        // Aquí mostrarías un panel UI con la descripción
    }
}