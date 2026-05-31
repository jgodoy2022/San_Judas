using UnityEngine;
using UnityEngine.SceneManagement; // <-- LIBRERÍA CRUCIAL PARA CAMBIAR DE ESCENA

public class MainMenuController : MonoBehaviour
{
    // Tu método original (sigue funcionando igual para el botón JUGAR si ya lo tenías conectado)
    public void IniciarJuego()
    {
        // Carga la escena del orfanato usando su nombre exacto
        SceneManager.LoadScene("01_Orfanato"); 
    }

    // ¡NUEVA FUNCIÓN GENERAL! Recibe cualquier escena desde el Inspector de Unity
    public void CambiarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // Este método lo podemos usar más adelante para el botón de salir
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}