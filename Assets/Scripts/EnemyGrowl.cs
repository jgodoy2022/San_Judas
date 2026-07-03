using UnityEngine;

public class EnemyGrowl : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip growlClip;
    public float minTime = 3f; // Tiempo mínimo entre gruñidos
    public float maxTime = 8f; // Tiempo máximo entre gruñidos
    private float timer;

    void Start()
    {
        // Iniciamos el timer con un valor aleatorio
        timer = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            audioSource.PlayOneShot(growlClip);
            // Reiniciamos el timer con un nuevo valor aleatorio
            timer = Random.Range(minTime, maxTime);
        }
    }
}