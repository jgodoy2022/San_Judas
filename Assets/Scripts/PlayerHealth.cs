using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public float maxHearts = 4f;
    public float currentHearts;

    [Header("Audio")]
    public AudioSource audioSource; // Arrastra el AudioSource del Player aquí
    public AudioClip dañoClip;      // Arrastra tu audio de daño aquí

    void Start()
    {
        currentHearts = maxHearts;
    }

    public void TakeDamage(float damage)
    {
        currentHearts -= damage;
        Debug.Log("¡Ay! Vida actual: " + currentHearts);

        // --- REPRODUCIR SONIDO AL RECIBIR DAÑO ---
        if (audioSource != null && dañoClip != null)
        {
            audioSource.PlayOneShot(dañoClip);
        }
        
        if (currentHearts <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over - Cargando escena...");
        SceneManager.LoadScene("03_GameOver");
    }
}