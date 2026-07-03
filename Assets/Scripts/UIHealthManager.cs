using UnityEngine;
using UnityEngine.UI; // Necesario para manejar Imágenes

public class UIHealthManager : MonoBehaviour
{
    public PlayerHealth player; // Arrastra a tu jugador aquí
    public Image[] corazonesUI; // Arrastra tus 4 imágenes de corazones aquí en orden
    
    public Sprite corazonLleno;
    public Sprite corazonMitad;
    public Sprite corazonVacio;

    void Update()
    {
        // Calculamos cuántos corazones enteros y medios tenemos
        float vidaActual = player.currentHearts;

        for (int i = 0; i < corazonesUI.Length; i++)
        {
            // El índice i va de 0 a 3. Un corazón en posición i es como decir:
            // "Si mi vida es mayor a (i+1), está lleno. Si es i+0.5, está a la mitad".
            
            if (vidaActual >= i + 1)
            {
                corazonesUI[i].sprite = corazonLleno;
            }
            else if (vidaActual >= i + 0.5f)
            {
                corazonesUI[i].sprite = corazonMitad;
            }
            else
            {
                corazonesUI[i].sprite = corazonVacio;
            }
        }
    }
}