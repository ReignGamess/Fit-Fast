using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false; // Oyuncu öldü mü kontrolü

    [Header("Game Over Bağlantısı")]
    [SerializeField] private GameOverManager gameOverManager;

    [HideInInspector] public float damageReduction = 0f; // 0 = normal, 0.5 = hasarın yarısı azalır   CAN KALKANI İKSİRİ İÇİN

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        int finalDamage = Mathf.RoundToInt(amount * (1f - damageReduction)); // Kalkan varsa hasarı azalt GELEN HASAR YARIYA İNER
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Oyuncu {finalDamage} hasar aldı. Kalan can: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //can yenileme, iyileşme
    public void Heal(int amount)
    {
        if (isDead) return; // Ölmüş karakter iyileşemez

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Can, maxHealth'i geçemesin

        Debug.Log($"Oyuncu {amount} can yeniledi. Güncel can: {currentHealth}");
    }

    private void Die()
    {
        if (isDead) return; // Die() bir kere çalışsın, tekrar tekrar tetiklenmesin
        isDead = true;

        Debug.Log("Oyuncu öldü.");

        // TODO (Kişi 9): Burada ölüm animasyonu/sesi/efekti oynatılacak

        // Oyuncunun hareket ve saldırı yeteneklerini kapat, ölü karakter artık kontrol edilemesin
        var attack = GetComponent<PlayerAttack>();
        if (attack != null) attack.enabled = false;

        // Game Over ekranını göster
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
    }
}