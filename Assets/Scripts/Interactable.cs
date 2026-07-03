using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Configuración del Objeto")]
    public string itemName = "Objeto";
    [TextArea] public string description = "Esto es un objeto interesante.";
    public bool canBePickedUp = true;
    public Sprite icon; // El sprite/foto que usaremos en el inventario

    // Este es el método que detectará cuando el jugador toque la llave
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto se puede recoger y lo que lo tocó es el Jugador
        if (canBePickedUp && collision.CompareTag("Player"))
        {
            // Buscamos el controlador de tu nuevo inventario deslizable
            InventoryController inventario = FindAnyObjectByType<InventoryController>();

            if (inventario != null)
            {
                // Le mandamos el nombre y su icono al inventario de abajo
                inventario.AgregarItemAlInventario(itemName, icon);

                Debug.Log($"¡{itemName} recogido con éxito!");
                
                // Destruimos la llave física del suelo del mapa
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("No se encontró el InventoryController en la escena. Asegúrate de que esté en el Button_Inventario_Deslizable.");
            }
        }
    }

    // Dejamos la función por si la necesitan para otra cosa en el futuro
    public void Interact(PlayerMovements player)
    {
        Debug.Log($"Interactuando con: {itemName}");
    }
}