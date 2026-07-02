using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para manejar componentes de UI

public class InventoryController : MonoBehaviour
{
    [Header("Configuración del Inventario")]
    [SerializeField] public Transform panelContenedor; // Objeto "Content" (Puesto público para escanearlo)
    [SerializeField] private GameObject celdaPrefab;     // Tu prefab azul "Celda_Item"
    
    private List<GameObject> celdasActuales = new List<GameObject>();

    void Start()
    {
        LimpiarInventario();
    }

    // Ahora la función te pide el NOMBRE del ítem y su SPRITE (Imagen)
    public void AgregarItemAlInventario(string nombreItem, Sprite spriteItem)
    {
        if (panelContenedor == null || celdaPrefab == null) return;

        // 1. Clonamos la celda dentro del contenedor "Content"
        GameObject nuevaCelda = Instantiate(celdaPrefab, panelContenedor);
        celdasActuales.Add(nuevaCelda);

        // LE CAMBIAMOS EL NOMBRE AL GAMEOBJECT CLONADO: Esto nos servirá para saber qué ítem es al escanear
        nuevaCelda.name = nombreItem;

        // 2. ASIGNAR LA IMAGEN DEL ÍTEM
        Image icono = nuevaCelda.transform.Find("Icono_Item")?.GetComponent<Image>();
        if (icono == null) icono = nuevaCelda.GetComponent<Image>(); 

        if (icono != null && spriteItem != null)
        {
            icono.sprite = spriteItem;
            icono.gameObject.SetActive(true);
        }

        // 3. CONFIGURAR EL CLIC (Para usar el ítem al tocarlo en la pantalla)
        Button botonCelda = nuevaCelda.GetComponent<Button>();
        if (botonCelda != null)
        {
            botonCelda.onClick.RemoveAllListeners(); 
            botonCelda.onClick.AddListener(() => UsarItem(nombreItem, nuevaCelda));
        }

        Debug.Log($"Añadido: {nombreItem} con su imagen. Celdas en pantalla: {celdasActuales.Count}");
    }

    private void UsarItem(string nombreItem, GameObject celdaAsociada)
    {
        Debug.Log($"¡Tocaste el ítem: {nombreItem} desde la pantalla táctil!");

        // LÓGICA PARA LA LLAVE
        if (nombreItem.ToLower().Contains("llave"))
        {
            PlayerMovements jugador = FindObjectOfType<PlayerMovements>();
            if (jugador != null)
            {
                // Solo si está pisando el trigger de la puerta final, se consume y abre
                if (jugador.EstaEnPuertaFinal())
                {
                    Debug.Log("¡Llave usada con éxito desde el inventario!");
                    jugador.AbrirPuertaFinalConExito();
                    
                    // Se elimina de la lista y se destruye de la barra
                    celdasActuales.Remove(celdaAsociada);
                    Destroy(celdaAsociada);
                }
                else
                {
                    Debug.Log("No puedes usar la llave aquí. Debes estar frente a la puerta final.");
                }
            }
        }
        else if (nombreItem.ToLower().Contains("vida") || nombreItem.ToLower().Contains("pocion"))
        {
            Debug.Log("¡Curando al jugador!");
            // Aquí puedes meter la lógica de curación en el futuro
            celdasActuales.Remove(celdaAsociada);
            Destroy(celdaAsociada);
        }
    }

    // Método para que PlayerMovements pueda remover la llave si se interactúa desde el botón físico
    public void EliminarCeldaPorNombre(GameObject celda)
    {
        if (celdasActuales.Contains(celda))
        {
            celdasActuales.Remove(celda);
        }
        Destroy(celda);
    }

    public void LimpiarInventario()
    {
        foreach (GameObject celda in celdasActuales)
        {
            Destroy(celda);
        }
        celdasActuales.Clear();
    }
}