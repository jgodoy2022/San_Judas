using UnityEngine;
using UnityEngine.SceneManagement; // <-- LIBRERÍA CRUCIAL PARA CAMBIAR DE ESCENA

public class MainMenuController : MonoBehaviour
{
    // Este método lo llamará el botón JUGAR al hacerle clic
    public void IniciarJuego()
    {
        // Carga la escena del orfanato usando su nombre exacto
        SceneManager.LoadScene("01_Orfanato"); 
    }

    // Este método lo podemos usar más adelante para el botón de salir
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}