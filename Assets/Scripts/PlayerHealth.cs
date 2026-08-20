using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false; // Oyuncu öldü mü kontrolü

    [Header("Game Over Bağlantısı")]
    [SerializeField] private GameOverManager gameOverManager;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // Zaten ölmüşse hiçbir şey yapma, düşman saldırmaya devam etse bile can azalmasın

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