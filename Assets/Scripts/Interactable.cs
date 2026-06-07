using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string itemName = "Objeto";
    public string description = "Esto es un objeto interesante.";
    public bool canBePickedUp = true;
    public Sprite icon; // Para inventario

    // Cambiamos PlayerInteraction por tu script unificado: PlayerMovements
    public void Interact(PlayerMovements player)
    {
        Debug.Log($"Interactuando con: {itemName}");
        
        // Aquí puedes agregar lógica en el futuro si el objeto 
        // hace algo especial con el jugador al ser tocado.
    }
}