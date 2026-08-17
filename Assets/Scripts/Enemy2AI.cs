using UnityEngine;

public class Enemy2AI : MonoBehaviour
{
    [Header("D2 - Hareket ve Algılama")]
    public float moveSpeed = 1.5f;          // YAVAŞ (2. canavar özelliği)
    public float detectionRange = 5f;       // Uzaktan algılar
    public float wanderRadius = 3f;         // Dolaşma alanı

    [Header("D3b - Saldırı (Yavaş ama Güçlü)")]
    public int attackDamage = 30;           // GÜÇLÜ vuruş
    public float attackCooldown = 3f;       // YAVAŞ vuruş (3 saniyede bir)
    public float attackRange = 1.5f;        // Yakın mesafe saldırı

    [Header("D5 - Zamanla Zorlaşma")]
    public float difficultyIncreaseInterval = 30f; // Her 30 saniyede bir güçlenir

    // PRIVATE DEĞİŞKENLER (Arka planda çalışır)
    private Transform player;
    private Vector2 targetPosition;
    private float wanderTimer = 0f;
    private float attackTimer = 0f;
    private float gameTimer = 0f;
    private int level = 1;
    private Rigidbody2D rb;

    void Start()
    {
        // Oyuncuyu bul ve Rigidbody'yi al
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        SetNewWanderTarget();
    }

    void Update()
    {
        // --- ZAMANLAYICILAR (Süre sayacı) ---
        wanderTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        gameTimer += Time.deltaTime;

        // --- D5: Zorluk Artışı (Her 30 saniyede bir) ---
        if (gameTimer >= difficultyIncreaseInterval)
        {
            gameTimer = 0f;
            level++;
            // Canavarı güçlendir (Hasar artar, hız biraz artabilir)
            attackDamage += 5;  
            moveSpeed += 0.1f;
            Debug.Log("Enemy2 Seviye Atladı! Seviye: " + level + ", Hasar: " + attackDamage);
        }

        // --- D2: Devriye (Wander) ve Takip (Chase) Mantığı ---
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            // OYUNCUYU TAKİP ET
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            // DEVRIYE (Rastgele dolaş)
            if (wanderTimer >= 2f) // 2 saniyede bir yeni hedef
            {
                SetNewWanderTarget();
                wanderTimer = 0f;
            }
            rb.MovePosition(Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime));
        }

        // --- D3b: Saldırı (Yavaş ama güçlü) ---
        // Eğer oyuncu saldırı menzilindeyse ve bekleme süresi (cooldown) dolduysa
        if (distanceToPlayer < attackRange && attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f; // Cooldown'u sıfırla
        }
    }

    void Attack()
    {
        // Burada gerçek hasar verme işlemi var.
        // Oyuncunun canı olduğunu varsayıyoruz (Kişi 3'ün sistemiyle bağlanacak).
        Debug.Log("Enemy2 SALDIRDI! Hasar: " + attackDamage + " (Seviye: " + level + ")");
        
        // İLERDE KİŞİ 3 İLE BİRLEŞTİRMEK İÇİN:
         player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        
        // Geçici görsel efekt için rengini kırmızı yap
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.2f);
    }

    void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void SetNewWanderTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)transform.position + randomDirection;
    }

    // --- D6b İÇİN HAZIRLIK: Bu script'i farklı canavar türlerinde kullanmak için 
    // buradaki değerleri (speed, damage, cooldown) dışarıdan değiştirebilirsin.
}