using System.Collections;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Üretilecek Besinler")]
    public GameObject[] foodPrefabs;

    [Header("Zamanlama Ayarları (Saniye)")]
    public float minSpawnZamani = 2f;
    public float maxSpawnZamani = 5f;

    [Header("Harita Sınırları (X ve Y)")]
    public Vector2 minKoordinatlar;
    public Vector2 maxKoordinatlar;

    // --- YENİ EKLENEN KISIM BURASI ---
    [Header("Başlangıç Ayarları")]
    public int baslangicBesinSayisi = 8; // Oyun başlar başlamaz haritada kaç besin olsun?
    // ---------------------------------

    void Start()
    {
        // --- YENİ EKLENEN KISIM BURASI ---
        // 1. Oyun başladığı an, beklemeksizin haritaya rastgele besinleri saç
        for (int i = 0; i < baslangicBesinSayisi; i++)
        {
            BesinUret();
        }
        // ---------------------------------

        // 2. Zamanla yeni besinler üretmeye devam edecek olan sonsuz döngüyü başlat
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float beklemeSuresi = Random.Range(minSpawnZamani, maxSpawnZamani);
            yield return new WaitForSeconds(beklemeSuresi);
            BesinUret();
        }
    }

    void BesinUret()
    {
        if (foodPrefabs.Length == 0) return;

        int rastgeleIndex = Random.Range(0, foodPrefabs.Length);
        GameObject secilenBesin = foodPrefabs[rastgeleIndex];

        float rastgeleX = Random.Range(minKoordinatlar.x, maxKoordinatlar.x);
        float rastgeleY = Random.Range(minKoordinatlar.y, maxKoordinatlar.y);
        Vector2 rastgeleKonum = new Vector2(rastgeleX, rastgeleY);

        Instantiate(secilenBesin, rastgeleKonum, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 merkez = new Vector3((minKoordinatlar.x + maxKoordinatlar.x) / 2, (minKoordinatlar.y + maxKoordinatlar.y) / 2, 0);
        Vector3 boyut = new Vector3(maxKoordinatlar.x - minKoordinatlar.x, maxKoordinatlar.y - minKoordinatlar.y, 0);
        Gizmos.DrawWireCube(merkez, boyut);
    }
}