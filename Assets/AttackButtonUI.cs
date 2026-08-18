using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Mobil ekrandaki saldırı düğmesine yapılan dokunma ve basılı tutma olaylarını dinler.
/// </summary>
public class AttackButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Referanslar")]
    [Tooltip("Karakterin üzerindeki PlayerShooting bileşeni")]
    [SerializeField] private PlayerShooting playerShooting;

    private void Start()
    {
        // Eğer editörden atanmadıysa sahnedeki PlayerShooting bileşenini otomatik bul
        if (playerShooting == null)
        {
            playerShooting = FindFirstObjectByType<PlayerShooting>();
        }
    }

    /// <summary>
    /// Ekranda butona parmakla dokunulduğu (veya fare ile tıklandığı) an çalışır.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerShooting != null)
        {
            playerShooting.OnAttackButtonDown();
        }
    }

    /// <summary>
    /// Parmak butondan kaldırıldığı (veya fare tuşu bırakıldığı) an çalışır.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (playerShooting != null)
        {
            playerShooting.OnAttackButtonUp();
        }
    }
}