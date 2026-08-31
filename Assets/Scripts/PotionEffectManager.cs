using UnityEngine;
using System.Collections;

//üretim/malzeme kontrolü yok, sadece 4 buton, her birine basınca ilgili özellik karaktere direkt yükleniyor

public class PotionEffectManager : MonoBehaviour
{
    [Header("Bağlantı")]
    [SerializeField] private PlayerHealth playerHealth;
    public static float CurrentDamageMultiplier = 1f; // 1 = normal hasar, 2 = iki katı
    public static float CurrentSpeedMultiplier = 1f; // 1 = normal hız, 1.5 = %50 daha hızlı

    [Header("İksir Etki Ayarları")]
    public int healAmount = 30;
    public float speedMultiplier = 1.5f;
    public float speedDuration = 5f;
    public float damageMultiplier = 2f;
    public float damageDuration = 5f;
    public float shieldReduction = 0.5f;
    public float shieldDuration = 4f;

    private bool speedBoostActive = false;
    private bool damageBoostActive = false;
    private bool shieldActive = false;

    private void Awake()
    {
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
    }

    // ---- BU 4 METOT, 4 BUTONA TEK TEK BAĞLANACAK ----

    public void UseRootBurnPotion() // Kök Yakıcı İksir -> Ekstra Hasar
    {
        StartCoroutine(DamageBoostRoutine());
    }

    public void UseAquaCrystalPotion() // Su Kristali İksiri -> Can Yenileme
    {
        if (playerHealth != null) playerHealth.Heal(healAmount);
    }

    public void UseFiberKineticPotion() // Kinetik Lif İksiri -> Kalkan
    {
        StartCoroutine(ShieldRoutine());
    }

    public void UseGreenEnergyPotion() // Yeşil Enerji İksiri -> Hız Artışı
    {
        StartCoroutine(SpeedBoostRoutine());
    }



    private IEnumerator DamageBoostRoutine()
    {
        if (damageBoostActive) yield break;
        damageBoostActive = true;

        CurrentDamageMultiplier = damageMultiplier; // örn. 2f -> hasar iki katına çıkar
        Debug.Log("Kök Yakıcı İksir aktif! Hasar " + damageMultiplier + "x arttı.");

        yield return new WaitForSeconds(damageDuration);

        CurrentDamageMultiplier = 1f; // süre bitince normale dön
        damageBoostActive = false;
        Debug.Log("Hasar iksiri süresi doldu.");
    }
    private IEnumerator SpeedBoostRoutine()
    {
        if (speedBoostActive) yield break;
        speedBoostActive = true;

        CurrentSpeedMultiplier = speedMultiplier; // örn. 1.5f
        Debug.Log("Yeşil Enerji İksiri aktif! Hız " + speedMultiplier + "x arttı.");

        yield return new WaitForSeconds(speedDuration);

        CurrentSpeedMultiplier = 1f;
        speedBoostActive = false;
        Debug.Log("Hız iksiri süresi doldu.");
    }

    private IEnumerator ShieldRoutine()
    {
        if (shieldActive || playerHealth == null) yield break;
        shieldActive = true;
        playerHealth.damageReduction = shieldReduction;
        Debug.Log("Kinetik Lif İksiri aktif! Gelen hasar azaldı.");

        yield return new WaitForSeconds(shieldDuration);

        playerHealth.damageReduction = 0f;
        shieldActive = false;
        Debug.Log("Kalkan iksiri süresi doldu.");
    }
}