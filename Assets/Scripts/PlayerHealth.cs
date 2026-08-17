using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Oyuncu {amount} hasar aldı. Kalan can: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Oyuncu öldü.");
        // İleride: Game Over ekranı, yeniden başlatma vs.
    }
}