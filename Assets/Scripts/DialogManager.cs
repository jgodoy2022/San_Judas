using UnityEngine;
using TMPro;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    void Awake() 
    { 
        instance = this; 
        dialogPanel.SetActive(false); 
        
        // Lanzamos el mensaje inicial después de un pequeñito retraso
        Invoke("MensajeInicial", 2.0f); 
    }

    void MensajeInicial()
    {
        ShowMessage("¿Qué pasó...?");
    }

    public void ShowMessage(string message)
    {
        // Esto es lo que mata cualquier proceso anterior y evita el bug
        StopAllCoroutines(); 
        dialogPanel.SetActive(true);
        StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        dialogText.text = ""; // Limpiar antes de escribir
        foreach (char letter in message.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.03f); // Velocidad de escritura
        }
        yield return new WaitForSeconds(2f); // Espera antes de cerrar
        dialogPanel.SetActive(false);
    }
}