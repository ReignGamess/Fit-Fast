using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Saldırı Ayarları")]
    [SerializeField] private int damageAmount = 25;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float cooldownTime = 0.8f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Yön Bilgisi")]
    [SerializeField] private Transform attackPoint;

    // YENİ: Ekrandaki Joystick'i bu değişkene bağlayacağız
    [SerializeField] private Joystick movementJoystick;

    private float lastAttackTime = -999f;
    private Vector2 facingDirection = Vector2.right; // Varsayılan: sağa bakıyor

    private void Update()
    {
        UpdateFacingDirectionFromInput();

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void UpdateFacingDirectionFromInput()
    {
        // Eski Input.GetAxisRaw yerine doğrudan Joystick scriptindeki GetInput() metodunu çağırıyoruz.
        float h = movementJoystick.GetInput().x;
        float v = movementJoystick.GetInput().y;

        if (h != 0 || v != 0)
        {
            facingDirection = new Vector2(h, v).normalized;
        }
        // h ve v sıfırsa (oyuncu parmağını joystickten çektiyse) son yön korunur, sıfırlanmaz.
    }

    public void TryAttack()
    {
        if (Time.time < lastAttackTime + cooldownTime)
        {
            return;
        }

        lastAttackTime = Time.time;
        PerformAttack();
    }

    private void PerformAttack()
    {
        // Saldırı merkezini karakterin baktığı yöne göre hesapla
        Vector2 attackCenter = (Vector2)transform.position + facingDirection * attackRange;

        // O noktanın etrafında, enemyLayer'daki tüm collider'ları bul
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackCenter, attackRadius, enemyLayer);

        foreach (Collider2D hit in hitColliders)
        {
            // Eğer Damageable scripti düşmanda varsa hasar ver
            Damageable damageable = hit.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
            }
        }

        Debug.Log($"Saldırı yapıldı, yön: {facingDirection}, {hitColliders.Length} hedef bulundu.");
    }

    // Sahnede saldırı alanını Scene view'da görmek için
    private void OnDrawGizmosSelected()
    {
        Vector2 attackCenter = (Vector2)transform.position + facingDirection * attackRange;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCenter, attackRadius);
    }
}