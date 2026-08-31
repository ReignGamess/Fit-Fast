using System.Collections;
using UnityEngine;

public class Enemy3AI : MonoBehaviour
{
    [Header("D7 - Hareket ve Algılama")]
    [Tooltip("Canavarın belirlenen alanda dolaşma hızı")]
    public float moveSpeed = 1.5f;
    [Tooltip("Oyuncunun fark edileceği maksimum mesafe (menzil)")]
    public float detectionRange = 5f;
    [Tooltip("Canavarın devriye gezeceği alanın yarıçapı")]
    public float wanderRadius = 3f;

    [Header("D9 - Saldırı ve Fırlatma Sistemi")]
    [Tooltip("Fırlatılacak nesnenin (Bomba/Mermi) Prefab'ı")]
    public GameObject projectilePrefab;
    [Tooltip("Nesnenin fırlatılma hızı")]
    public float launchForce = 7f;
    [Tooltip("Saldırılar arasındaki bekleme süresi (saniye)")]
    public float attackCooldown = 2f;

    [Header("Alan Hasarı Ayarları")]
    [Tooltip("Patlama / Hasar yarıçapı")]
    public float explosionRadius = 2f;
    [Tooltip("Karaktere verilecek hasar miktarı")]
    public float damageAmount = 25f;

    [Header("Efekt Ayarları")]
    [Tooltip("Hasar alan oyuncunun yanacağı renk")]
    public Color playerDamageColor = Color.red;
    [Tooltip("Oyuncunun kırmızı kalacağı süre (saniye)")]
    public float flashDuration = 0.2f;

    private Vector2 startPosition;
    private Transform player;
    private Vector2 targetPosition;
    private float wanderTimer = 0f;
    private float nextAttackTime = 0f;
    private Rigidbody2D rb;
    private Collider2D enemyCollider;

    void Start()
    {
        //Oyuncu referansını bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        SetNewWanderTarget();
    }

    void Update()
    {
        if (player == null) return;

        wanderTimer += Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            //Takip
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);

            //Saldırı kontrolü
            if (Time.time >= nextAttackTime)
            {
                ThrowProjectile();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            //Devriye
            if (wanderTimer >= 2f)
            {
                SetNewWanderTarget();
                wanderTimer = 0f;
            }
            rb.MovePosition(Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime));
        }
    }

    void SetNewWanderTarget()
    {
        //Devriye hedefini belirle
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + randomDirection;
    }

    void ThrowProjectile()
    {
        if (projectilePrefab == null) return;

        //Oyuncunun yönünü hesapla
        Vector2 throwDirection = (player.position - transform.position).normalized;

        //Alt kısmın (bottom) karaktere bakması için +90 derece ofset eklendi
        float angle = (Mathf.Atan2(throwDirection.y, throwDirection.x) * Mathf.Rad2Deg) + 90f;

        //Düşmanın biraz önünde ve doğru açıda oluştur
        Vector3 spawnPos = transform.position + (Vector3)throwDirection * 0.8f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject proj = Instantiate(projectilePrefab, spawnPos, rotation);

        //Düşman ile merminin çarpışmasını engelle
        Collider2D projCollider = proj.GetComponent<Collider2D>();
        if (projCollider != null && enemyCollider != null)
        {
            Physics2D.IgnoreCollision(projCollider, enemyCollider);
        }

        //Alan hasarı bileşenini ekle ve renk parametrelerini aktar
        AreaDamage damageScript = proj.AddComponent<AreaDamage>();
        damageScript.Setup(explosionRadius, damageAmount, playerDamageColor, flashDuration);

        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            projRb.AddForce(throwDirection * launchForce, ForceMode2D.Impulse);
        }
    }
}

//Fırlatılan nesnenin alan hasarı vermesini ve silinmesini sağlayan sınıf
public class AreaDamage : MonoBehaviour
{
    private float radius;
    private float damage;
    private Color damageColor;
    private float flashDuration;

    public void Setup(float radius, float damage, Color damageColor, float flashDuration)
    {
        this.radius = radius;
        this.damage = damage;
        this.damageColor = damageColor;
        this.flashDuration = flashDuration;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleImpact(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleImpact(collider.gameObject);
    }

    private void HandleImpact(GameObject hitObject)
    {
        //Çarpışma anında yarıçap içindekilere hasar ver
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

                //Oyuncunun SpriteRenderer bileşenini bulup kırmızı yakma işlemini başlat
                SpriteRenderer playerSr = hit.GetComponent<SpriteRenderer>();
                if (playerSr != null)
                {
                    //Mermi yok olacağı için Coroutine'i oyuncu nesnesi üzerinden çalıştırıyoruz
                    hit.GetComponent<MonoBehaviour>().StartCoroutine(FlashPlayerRed(playerSr));
                }
            }
        }

        //Mermiyi yok et
        Destroy(gameObject);
    }

    private IEnumerator FlashPlayerRed(SpriteRenderer playerSr)
    {
        Color originalColor = playerSr.color;
        playerSr.color = damageColor;

        yield return new WaitForSeconds(flashDuration);

        if (playerSr != null)
        {
            playerSr.color = originalColor;
        }
    }
}