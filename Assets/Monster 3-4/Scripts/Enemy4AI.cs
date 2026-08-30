using System.Collections;
using UnityEngine;

public class Enemy4AI : MonoBehaviour
{
    [Header("D8 - Hareket ve Algılama")]
    [Tooltip("Canavarın devriye dolaşma hızı")]
    public float moveSpeed = 1.5f;
    [Tooltip("Oyuncunun fark edileceği maksimum mesafe (menzil)")]
    public float detectionRange = 5f;
    [Tooltip("Canavarın devriye gezeceği alanın yarıçapı")]
    public float wanderRadius = 3f;

    [Header("Sıçrama Saldırısı Ayarları")]
    [Tooltip("Sıçrama anındaki hareket hızı")]
    public float leapSpeed = 8f;
    [Tooltip("Sıçrama saldırıları arasındaki bekleme süresi (saniye)")]
    public float attackCooldown = 2f;
    [Tooltip("Sıçramadan önceki hazırlık süresi (saniye)")]
    public float chargeTime = 0.5f;
    [Tooltip("Sıçramanın ne kadar süreceği (saniye)")]
    public float leapDuration = 0.4f;

    [Header("Hasar ve Efekt Ayarları")]
    [Tooltip("Karaktere çarptığında verilecek hasar miktarı")]
    public float damageAmount = 25f;
    [Tooltip("Hasarın etki edeceği alan yarıçapı")]
    public float explosionRadius = 1.5f;
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

    private enum EnemyState { Idle, Wandering, Charging, Leaping, Cooldown }
    private EnemyState currentState = EnemyState.Wandering;

    private Vector2 leapDirection;
    private float stateTimer = 0f;

    void Start()
    {
        //Oyuncuyu sahnede bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        SetNewWanderTarget();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Wandering:
                HandleWandering(distanceToPlayer);
                break;

            case EnemyState.Charging:
                //Hazırlık aşaması
                stateTimer += Time.deltaTime;
                if (stateTimer >= chargeTime)
                {
                    currentState = EnemyState.Leaping;
                    stateTimer = 0f;
                }
                break;

            case EnemyState.Leaping:
                //Sıçrama hareketi
                stateTimer += Time.deltaTime;
                rb.MovePosition((Vector2)transform.position + leapDirection * leapSpeed * Time.deltaTime);

                if (stateTimer >= leapDuration)
                {
                    currentState = EnemyState.Cooldown;
                    nextAttackTime = Time.time + attackCooldown;
                }
                break;

            case EnemyState.Cooldown:
                if (Time.time >= nextAttackTime)
                {
                    currentState = EnemyState.Wandering;
                }
                else
                {
                    //Cooldown esnasında yavaşça takip et
                    Vector2 direction = (player.position - transform.position).normalized;
                    rb.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);
                }
                break;
        }
    }

    void HandleWandering(float distanceToPlayer)
    {
        if (distanceToPlayer < detectionRange && Time.time >= nextAttackTime)
        {
            currentState = EnemyState.Charging;
            stateTimer = 0f;
            leapDirection = (player.position - transform.position).normalized;
            return;
        }

        wanderTimer += Time.deltaTime;
        if (wanderTimer >= 2f)
        {
            SetNewWanderTarget();
            wanderTimer = 0f;
        }
        rb.MovePosition(Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime));
    }

    void SetNewWanderTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + randomDirection;
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                //Hasar mesajını gönder
                hit.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);

                //Oyuncunun SpriteRenderer bileşenini al ve kırmızı efekt coroutine'ini çalıştır
                SpriteRenderer playerSr = hit.GetComponent<SpriteRenderer>();
                if (playerSr != null)
                {
                    StartCoroutine(FlashPlayerRed(playerSr));
                }
            }
        }
    }

    //Oyuncuyu kısa süreliğine kırmızı yapıp eski rengine döndüren coroutine
    private IEnumerator FlashPlayerRed(SpriteRenderer playerSr)
    {
        Color originalColor = playerSr.color;
        playerSr.color = playerDamageColor;

        yield return new WaitForSeconds(flashDuration);

        //Eğer oyuncu obje silinmediyse rengi eski haline getir
        if (playerSr != null)
        {
            playerSr.color = originalColor;
        }
    }
}