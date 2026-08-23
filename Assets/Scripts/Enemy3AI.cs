using UnityEngine;

public class Enemy3AI : MonoBehaviour
{
    [Header("D7 - Hareket ve Algılama")]
    [Tooltip("Canavarın belirlenen alanda dolaşma hızı")]
    public float moveSpeed = 1.5f;          //Hız
    [Tooltip("Oyuncunun fark edileceği maksimum mesafe (menzil)")]
    public float detectionRange = 5f;       //Algılama mesafesi
    [Tooltip("Canavarın devriye gezeceği alanın yarıçapı")]
    public float wanderRadius = 3f;         //Dolaşma yeri

    private Vector2 startPosition;
    private Transform player;
    private Vector2 targetPosition;
    private float wanderTimer = 0f;
    private Rigidbody2D rb;
    void Start()
    {
        // Oyuncuyu bul ve Rigidbody'yi al
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        SetNewWanderTarget();
    }
    void Update()
    {

        //D7 Devriye ve takip kısmı 
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            //Takip
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            //Devriye
            if (wanderTimer >= 2f) // 2 saniyede bir yeni hedef
            {
                SetNewWanderTarget();
                wanderTimer = 0f;
            }
            rb.MovePosition(Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime));
        }
    }
    void SetNewWanderTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)transform.position + randomDirection;
    }
}