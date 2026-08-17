using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SugarMonsterD1 : MonoBehaviour
{
    [Header("Hareket Hızları")]
    [Tooltip("Canavarın belirlenen alanda dolaşma hızı")]
    public float patrolSpeed = 2f;
    [Tooltip("Canavarın oyuncuyu fark edip takip etme hızı")]
    public float chaseSpeed = 3.5f;

    [Header("Algılama ve Alan Ayarları")]
    [Tooltip("Oyuncunun fark edileceği maksimum mesafe (menzil)")]
    public float detectionRange = 5f;
    [Tooltip("Canavarın devriye gezeceği alanın yarıçapı")]
    public float patrolRadius = 4f;
    [Tooltip("Hedef noktaya varıldığında bekleme süresi (saniye)")]
    public float waitTimeAtWaypoint = 1.5f;
    [Tooltip("Hedefe ulaşıldı sayılma hassasiyeti")]
    public float waypointReachThreshold = 0.2f;

    [Header("Hedef (Oyuncu)")]
    public Transform playerTransform;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 currentPatrolTarget;
    private float waitTimer;
    private bool isWaiting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Canavarın ilk doğduğu konumu devriye merkezi olarak kaydet
        startPosition = transform.position;
        SetNewPatrolTarget();

        // Eğer Inspector'dan oyuncu atanmadıysa "Player" etiketli objeyi otomatik bul
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Oyuncu ile canavar arasındaki mesafeyi hesapla
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // D1 Görev Kuralı: Oyuncu yaklaşınca yönel, uzaktaysa alanda dolaş
        if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            PatrolArea();
        }
    }

    // Belirlenen alanda rastgele noktalara giderek devriye gezinme mantığı
    void PatrolArea()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SetNewPatrolTarget();
            }
            return;
        }

        // Hedef noktaya doğru hareket et
        Vector2 nextPosition = Vector2.MoveTowards(rb.position, currentPatrolTarget, patrolSpeed * Time.deltaTime);
        rb.MovePosition(nextPosition);

        // Hedef noktaya varıldı mı kontrol et
        if (Vector2.Distance(rb.position, currentPatrolTarget) <= waypointReachThreshold)
        {
            isWaiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }

    // Oyuncu algılama alanına girdiğinde çalışan takip mantığı
    void ChasePlayer()
    {
        // Takip halindeyken bekleme durumunu sıfırla
        isWaiting = false;

        Vector2 nextPosition = Vector2.MoveTowards(rb.position, playerTransform.position, chaseSpeed * Time.deltaTime);
        rb.MovePosition(nextPosition);
    }

    // Başlangıç noktası etrafında yeni bir rastgele hedef nokta belirler
    void SetNewPatrolTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        currentPatrolTarget = startPosition + randomOffset;
    }

    // Sahne penceresinde menzilleri görsel olarak takip etmek için (Gizmos)
    private void OnDrawGizmosSelected()
    {
        Vector2 center = Application.isPlaying ? startPosition : (Vector2)transform.position;

        // Devriye alanını sarı daire olarak göster
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, patrolRadius);

        // Oyuncu algılama menzilini kırmızı daire olarak göster
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}