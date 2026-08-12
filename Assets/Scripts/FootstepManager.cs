using UnityEngine;
using UnityEngine.InputSystem; // Importante añadir esto

public class FootstepManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip footstepsClip;
    
    // Referencia al componente que mueve a tu personaje
    public Rigidbody rb; 

    void Start()
    {
        audioSource.clip = footstepsClip;
        audioSource.loop = true;
    }

    void Update()
    {
        // Detectamos si el personaje se está moviendo a través de su velocidad
        // Esto funciona independientemente de si usas el nuevo o viejo Input System
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        if (isMoving)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}