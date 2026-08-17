using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    public Joystick joystick; // Joystick'i Inspector'dan buraya atayacağız

    void Start()
    {
        // Rigidbody2D bileşenini alıyoruz
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Joystick'ten gelen veriyi alıyoruz
        if (joystick != null)
        {
            Vector2 moveInput = joystick.GetInput();
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }
}