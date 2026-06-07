using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <--- ¡Muy importante para poder controlar el texto!

public class CountdownTimer : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoRestante = 120f; // 2 minutos en segundos
    
    [Header("Componentes de Interfaz")]
    [SerializeField] private TMP_Text textoReloj; // Aquí arrastraremos el texto de la UI

    [Header("Escena de Derrota")]
    [SerializeField] private string escenaGameOver = "03_GameOver";

    private bool juegoTerminado = false;

    void Update()
    {
        if (juegoTerminado) return;

        if (tiempoRestante > 0)
        {
            // Resta el tiempo en cada fotograma
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoVisual();
        }
        else
        {
            // ¡Tiempo agotado!
            tiempoRestante = 0;
            juegoTerminado = true;
            ActualizarTextoVisual();
            CambiarAGameOver();
        }
    }

    void ActualizarTextoVisual()
    {
        if (textoReloj == null) return;

        // Calculamos minutos y segundos reales
        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);

        // Formatea el texto para que siempre use 2 dígitos (ej: 01:05 en vez de 1:5)
        textoReloj.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void CambiarAGameOver()
    {
        Debug.Log("¡Se agotó el tiempo en el orfanato! Cargando Game Over...");
        SceneManager.LoadScene(escenaGameOver);
    }
}