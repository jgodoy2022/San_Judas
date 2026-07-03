using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class PlayerHealth : MonoBehaviour
{
    public float maxHearts = 4f;
    public float currentHearts;

    void Start()
    {
        currentHearts = maxHearts;
    }

    public void TakeDamage(float damage)
    {
        currentHearts -= damage;
        Debug.Log("¡Ay! Vida actual: " + currentHearts);
        
        if (currentHearts <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over - Cargando escena...");
        
        // Carga la escena específica que definiste
        SceneManager.LoadScene("03_GameOver");
    }
}