using UnityEngine;

/// <summary>
/// Merminin hareketini, ömrünü ve düşmana çarpışma anındaki hasar mantığını yönetir.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Mermi Ayarları")]
    [Tooltip("Merminin saniyedeki uçuş hızı")]
    [SerializeField] private float speed = 10f;

    [Tooltip("Merminin hiçbir şeye çarpmazsa sahnede kalacağı maksimum süre (saniye)")]
    [SerializeField] private float lifeTime = 2f;

    [Tooltip("Merminin düşmana vereceği hasar miktarı")]
    [SerializeField] private int damage = 10;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Belirlenen süre sonunda mermiyi yok et
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 direction)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Çarptığımız objenin etiketi "Enemy" mi?
        if (collision.CompareTag("Enemy"))
        {
            // Düşmanın üzerindeki EnemyHealth scriptine ulaşıyoruz
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                // Hasarı iletiyoruz
                enemy.TakeDamage(damage);
            }

            // Mermiyi yok ediyoruz
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}