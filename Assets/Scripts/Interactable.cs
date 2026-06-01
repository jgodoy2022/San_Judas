using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string itemName = "Objeto";
    public string description = "Esto es un objeto interesante.";
    public bool canBePickedUp = true;
    public Sprite icon; // Para inventario

    public void Interact(PlayerInteraction player)
    {
        // Aquí puedes decidir qué hacer según el tipo
        Debug.Log($"Interactuando con: {itemName}");
    }
}