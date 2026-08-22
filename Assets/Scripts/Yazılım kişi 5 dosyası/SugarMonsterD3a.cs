using UnityEngine;

/// <summary>
/// D3a Görevi: İlk canavar türü (Şeker Canavarı) için özel hızlı ve zayıf vuruş mekanizması.
/// SugarMonsterD1 ile uyumlu çalışacak şekilde tasarlanmıştır.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SugarMonsterD3a : MonoBehaviour
{
    [Header("D3a: Saldırı Konfigürasyonu")]
    [Tooltip("Canavarın oyuncuya hızlı/zayıf vuruş yapabileceği maksimum saldırı menzili.")]
    [SerializeField] private float attackRange = 1.2f;

    [Tooltip("Verilecek zayıf saldırı hasar miktarı.")]
    [SerializeField] private int attackDamage = 5;

    [Tooltip("Saldırıların arasındaki minimum bekleme süresi (Cooldown - saniye cinsinden).")]
    [SerializeField] private float attackCooldown = 1.0f;

    [Header("Hedef Referansı")]
    [Tooltip("Hedef alttaki oyuncu transform bileşeni. Boş bırakılırsa 'Player' etiketli obje otomatik seçilir.")]
    [SerializeField] private Transform playerTransform;

    // Dahili Durum Değişkenleri
    private float lastAttackTime;
    private bool isPlayerInRange = false;

    private void Start()
    {
        // Eğer Inspector üzerinden oyuncu atanmadıysa sahnedeki "Player" etiketini arayıp bul
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: Sahnede 'Player' etiketine sahip bir nesne bulunamadı! Lütfen oyuncu objesini etiketleyin.");
            }
        }
    }

    private void Update()
    {
        // Hedef yoksa işlem yapma
        if (playerTransform == null) return;

        // Oyuncu ile canavar arasındaki mesafeyi hesapla
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Mesafe kontrolü ve D3a saldırı tetikleyicisi
        if (distanceToPlayer <= attackRange)
        {
            isPlayerInRange = true;
            TryExecuteAttack();
        }
        else
        {
            isPlayerInRange = false;
        }
    }

    /// <summary>
    /// Cooldown süresini hesaba katarak hızlı ve zayıf vuruş gerçekleştirme girişimi.
    /// </summary>
    private void TryExecuteAttack()
    {
        // Mevcut zaman, son saldırı zamanı artı cooldown süresinden büyük veya eşitse saldır
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformQuickAttack();
        }
    }

    /// <summary>
    /// Gerçekleşen zayıf vuruş eylemi ve hasar aktarım mantığı.
    /// </label>
    private void PerformQuickAttack()
    {
        // Örnek Entegrasyon: Oyuncunun can betiğine (PlayerHealth) buradan erişerek hasar verebilirsin.
        /*
        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        */

        Debug.Log($"<color=magenta>[D3a Görevi]</color> {gameObject.name}, oyuncuya hızlı ve zayıf vuruş yaptı! Verilen Hasar: {attackDamage}");
    }

    /// <summary>
    /// Unity Editör penceresinde saldırı menzilini mavi renkle görselleştirmek için kullanılır.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Editörde seçildiğinde veya oyun çalışırken saldırı alanını göster
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}