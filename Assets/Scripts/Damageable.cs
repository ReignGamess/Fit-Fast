using UnityEngine;

public class Damageable : MonoBehaviour
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

        Debug.Log($"{gameObject.name} {amount} hasar aldı. Kalan can: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO (Kişi 9): Burada canavar ölüm efekti/sesi oynatılacak
        Debug.Log($"{gameObject.name} öldü.");
        Destroy(gameObject);
    }
}