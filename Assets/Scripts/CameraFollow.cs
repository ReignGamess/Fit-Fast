using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Takip edilecek hedef (Oyuncu)
    public Transform target;

    // Kameranın takip yumuşaklığı ve hızı
    [Range(1f, 15f)]
    public float smoothSpeed = 5f;

    // Kamera ile oyuncu arasındaki mesafe farkı
    private Vector3 offset;

    void Start()
    {
        // Hedef atanmışsa başlangıçtaki mesafe farkını otomatik kaydet
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        // Hedef yoksa hata vermemesi için işlemi durdur
        if (target == null) return;

        // Kameranın gitmesi gereken hedef pozisyon
        Vector3 desiredPosition = target.position + offset;

        // Ani geçişleri engelleyip yumuşak (smooth) bir takip sağlamak için Lerp kullanımı
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Yeni pozisyonu kameraya uygula
        transform.position = smoothedPosition;
    }
}