using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Karakterin hareket hızı
    public float moveSpeed = 5f;

    // Unity'deki Animator bileşeni
    public Animator animator;

    // Kendi yazdığın Joystick scriptinin referansı
    public Joystick mobileJoystick;

    private Rigidbody2D rb;
    private Vector2 movementDirection;

    void Start()
    {
        // Rigidbody2D bileşenini alıyoruz
        rb = GetComponent<Rigidbody2D>();

        // Animator'ı otomatik olarak al (Manuel sürüklemeye gerek kalmaz)
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Kendi Joystick scriptindeki GetInput fonksiyonundan değerleri alıyoruz
        if (mobileJoystick != null)
        {
            movementDirection = mobileJoystick.GetInput();
        }

        // Animator parametrelerini güncelliyoruz
        UpdateAnimatorParameters();
    }

    void FixedUpdate()
    {
        // Karakteri hareket ettiriyoruz
        rb.linearVelocity = movementDirection * moveSpeed;
    }

    // Animasyon geçişlerini ve yön mantığını yöneten fonksiyon
    void UpdateAnimatorParameters()
    {
        // Eğer hareket varsa
        if (movementDirection.sqrMagnitude > 0.01f)
        {
            // Karakterin yürüdüğünü belirtmek için 'IsWalking' true yapılıyor
            animator.SetBool("IsWalking", true);

            // Hangi yöne baktığını Animator'a bildiriyoruz
            animator.SetFloat("InputX", movementDirection.x);
            animator.SetFloat("InputY", movementDirection.y);
        }
        else
        {
            // Hareket yoksa durma (Idle) durumuna geçiliyor
            animator.SetBool("IsWalking", false);
        }
    }
}