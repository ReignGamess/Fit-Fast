using UnityEngine;

/// <summary>
/// Karakterin saldırı girişlerini (Klavye, Fare ve Mobil Dokunmatik UI Butonu),
/// atış bekleme süresini (cooldown) ve yön yönetimini sağlar[cite: 1].
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    [Header("Menzilli Saldırı Ayarları")]
    [Tooltip("Fırlatılacak mermi prefabı[cite: 1]")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("Merminin çıkış noktası (Transform)")]
    [SerializeField] private Transform firePoint;

    [Tooltip("İki menzilli atış arasındaki minimum bekleme süresi (saniye)[cite: 1]")]
    [SerializeField] private float rangedCooldown = 0.6f;

    [Header("Giriş / Tuş Algılama Ayarları")]
    [Tooltip("Basılı tutma sayılması için gereken minimum basma süresi (saniye)")]
    [SerializeField] private float holdThreshold = 0.25f;

    // Zamanlayıcılar ve durum takibi
    private float nextRangedFireTime = 0f;
    private float pressStartTime = 0f;
    private bool isHolding = false;

    // Karakterin son baktığı yön (varsayılan: sağa doğru)
    private Vector2 lastLookDirection = Vector2.right;

    private void Update()
    {
        // 1. Hareket yönünü oku ve son bakış yönünü güncelle
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            lastLookDirection = new Vector2(moveX, moveY).normalized;
            UpdateFirePointPosition();
        }

        // 2. Klavye/Boşluk Tuşu Test Kontrolleri
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnAttackButtonDown();
        }

        if (Input.GetKey(KeyCode.Space) && isHolding)
        {
            CheckHoldAttack();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnAttackButtonUp();
        }

        // 3. UI Butonuna basılı tutuluyorsa sürekli kontrol et
        if (isHolding)
        {
            CheckHoldAttack();
        }
    }

    /// <summary>
    /// Saldırı butonuna basıldığı an çağrılır.
    /// </summary>
    public void OnAttackButtonDown()
    {
        pressStartTime = Time.time;
        isHolding = true;
    }

    /// <summary>
    /// Butona basılı tutulma süresini kontrol eder ve menzilli atışı tetikler[cite: 1].
    /// </summary>
    private void CheckHoldAttack()
    {
        if (Time.time - pressStartTime >= holdThreshold)
        {
            if (Time.time >= nextRangedFireTime)
            {
                PerformRangedAttack();
                nextRangedFireTime = Time.time + rangedCooldown;
            }
        }
    }

    /// <summary>
    /// Saldırı butonu bırakıldığı an çağrılır.
    /// </summary>
    public void OnAttackButtonUp()
    {
        if (!isHolding) return;

        float pressDuration = Time.time - pressStartTime;

        // Kısa dokunuşsa yakın dövüşü tetikle[cite: 1]
        if (pressDuration < holdThreshold)
        {
            PerformMeleeAttack();
        }

        isHolding = false;
    }

    private void UpdateFirePointPosition()
    {
        if (firePoint != null)
        {
            firePoint.localPosition = lastLookDirection * 0.6f;
        }
    }

    private void PerformRangedAttack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("PlayerShooting: Projectile Prefab atanmamış!");
            return;
        }

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        float angle = Mathf.Atan2(lastLookDirection.y, lastLookDirection.x) * Mathf.Rad2Deg;
        Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

        GameObject newProjectileObj = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

        Projectile projectileScript = newProjectileObj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Launch(lastLookDirection);
            Debug.Log("Menzilli Saldırı Yapıldı![cite: 1]");
        }
    }

    private void PerformMeleeAttack()
    {
        Debug.Log("Kısa basıldı: Yakın Dövüş Saldırısı Tetiklendi![cite: 1]");
    }
}