using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventarioUI : MonoBehaviour
{
    private List<Image> celdasIconos = new List<Image>();

    void Awake()
    {
        // Busca automáticamente TODOS los componentes Image que estén dentro de este objeto (tus celdas)
        // Ignora el fondo principal y guarda los iconos hijos
        Image[] todasLasImagenes = GetComponentsInChildren<Image>(true);
        
        foreach (Image img in todasLasImagenes)
        {
            // Filtramos para guardar solo los objetos que se llamen "Icono_Item" o similar
            // Y nos aseguramos de no añadir el fondo del panel principal
            if (img.gameObject != this.gameObject && img.name.ToLower().Contains("icono"))
            {
                celdasIconos.Add(img);
                img.gameObject.SetActive(false); // Los ocultamos al empezar (vacíos)
            }
        }
        
        Debug.Log($"Inventario inicializado automáticamente con {celdasIconos.Count} celdas.");
    }

    public bool AgregarItemAlInventario(Sprite spriteItem)
    {
        for (int i = 0; i < celdasIconos.Count; i++)
        {
            // Si la celda está oculta, significa que está libre
            if (!celdasIconos[i].gameObject.activeSelf)
            {
                celdasIconos[i].sprite = spriteItem;
                celdasIconos[i].gameObject.SetActive(true); // Mostramos el ítem visualmente
                Debug.Log($"Objeto añadido automáticamente a la celda visual número: {i}");
                return true; 
            }
        }

        Debug.LogWarning("¡El inventario está lleno!");
        return false; 
    }
}