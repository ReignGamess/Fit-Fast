using UnityEngine;

public class CollectibleFood : MonoBehaviour
{
    // Hangi besin olduğunu Inspector'dan seçeceğiz
    [Header("Besin Ayarları")]
    public HealthyFood foodType;
    public int amount = 1; // Toplandığında kaç tane verecek?

    // 2D fizikte bir obje bu nesnenin içine girdiğinde tetiklenir
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer çarpan obje "Player" etiketine sahipse (yani bizim karakterse)
        if (collision.CompareTag("Player"))
        {
            // TODO: Burada UI ve Envanter ekibinin yazdığı fonksiyona veriyi göndereceğiz.
            // Örnek: InventoryManager.Instance.AddFood(foodType, amount);

            Debug.Log(amount + " adet " + foodType.ToString() + " toplandı!");

            // Obje toplandığı için onu sahneden (haritadan) siliyoruz
            Destroy(gameObject);
        }
    }
}