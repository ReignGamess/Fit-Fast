using UnityEngine;

/// <summary>
/// Düşmanın can puanını (HP) yönetir, hasar almasını ve ölmesini sağlar.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    [Tooltip("Düşmanın maksimum ve başlangıç canı")]
    [SerializeField] private int maxHealth = 30;

    // Mevcut anlık can miktarı
    private int currentHealth;

    private void Start()
    {
        // Oyun başladığında mevcut canı maksimum cana eşitliyoruz
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Mermiden veya başka bir kaynaktan hasar alındığında çağrılır.
    /// </summary>
    /// <param name="damageAmount">Alınacak hasar miktarı</param>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " hasar aldı! Kalan Can: " + currentHealth);

        // Can sıfıra veya altına düşerse canavarı yok et
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Düşman öldüğünde çalışacak işlemler.
    /// </summary>
    private void Die()
    {
        Debug.Log(gameObject.name + " yok edildi!");
        // Düşman objesini sahneden kaldırıyoruz
        Destroy(gameObject);
    }
}