using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 0.5f; // Lo que pediste: medio corazón
    public float attackRate = 1.0f; 
    private float nextAttackTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        // Solo ataca si lo que toca es el jugador
        if (other.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    nextAttackTime = Time.time + attackRate;
                    Debug.Log("¡Ataque recibido! Vida restante en jugador.");
                }
            }
        }
    }
}