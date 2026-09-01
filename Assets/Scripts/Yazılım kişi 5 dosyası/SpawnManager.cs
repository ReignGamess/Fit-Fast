using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    [Tooltip("Haritada canavarların doğabileceği noktalar (F3 entegrasyonu için transform listesi).")]
    public Transform[] spawnPoints;

    [Tooltip("Oluşturulacak canavar prefab'i (Geçici renkli kutu veya gerçek D1/D2 karakteri).")]
    public GameObject enemyPrefab;

    [Header("Zamanlama ve Limitler")]
    [Tooltip("Canavarların kaç saniyede bir doğacağı.")]
    public float spawnInterval = 5f;

    [Tooltip("Haritada aynı anda bulunabilecek maksimum canavar sayısı.")]
    public int maxEnemiesOnMap = 10;

    private int currentEnemyCount = 0;
    private bool canSpawn = true;

    void Start()
    {
        // F3 göreviyle belirlenen spawn noktalarının doğruluğunu kontrol et
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("SpawnManager: Hiçbir spawn noktası tanımlanmamış! Lütfen F3 harita spawn noktalarını ekleyin.");
        }

        // Belirlenen aralıklarla spawn döngüsünü başlat
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Oyun devam ettiği sürece çalışır
        while (canSpawn)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Maksimum sınır kontrolü
            if (currentEnemyCount < maxEnemiesOnMap && spawnPoints.Length > 0)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        // Rastgele bir spawn noktası seç
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // Canavarı seçilen noktada ve rotasyonda oluştur
        GameObject newEnemy = Instantiate(enemyPrefab, selectedPoint.position, Quaternion.identity);

        // Canavar sayısını artır ve ölüm durumunda sayaçtan düşmek için dinleyici ekle
        currentEnemyCount++;

        // Canavar yok olduğunda (ölünce) sayacın güncellenmesi için bir script entegrasyonu
        // Örn: newEnemy.GetComponent<EnemyHealth>().OnEnemyDeath += HandleEnemyDeath;
    }

    // Canavar öldüğünde dışarıdan tetiklenebilecek veya GameManager üzerinden yönetilebilecek metod
    public void HandleEnemyDeath()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
    }
}