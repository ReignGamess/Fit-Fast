using System.Collections.Generic;
using UnityEngine;

// 1. Arayüz ve envanter ekibinin toplanan eşyaları takip edeceği Besin Listesi
public enum HealthyFood
{
    YabaniNaneKoku,      // Wild Mint Root
    Pancar,              // Crimson Beet
    BrokoliFilizi,       // Broccoli Sprouts
    AloeVeraDamlalari,   // Aloe Vera Drops
    DagSalataligi,       // Mountain Cucumber
    YabanMersini,        // Wild Blueberries
    TamTahil,            // Whole Grains
    AltinKetenTohumu,    // Golden Flaxseeds
    CitirIspanak,        // Crispy Spinach Leaves
    KraliyetAvokadosu,   // Royal Avocado
    AromatikKereviz,     // Aromatic Celery
    KopukluYesilKivi     // Sparkling Green Kiwi
}

// 2. İksir tarifinin yapısını belirleyen Sınıf
[System.Serializable]
public class PotionRecipe
{
    public string PotionName;
    public string CombatEffect;
    public string Description; // Çalışma mantığı
    public List<HealthyFood> RequiredFoods;
}

// 3. Oyun içi veritabanı ve Üretim (Crafting) Sistemi
public class PotionDatabase : MonoBehaviour
{
    [Header("İksir Veritabanı")]
    public List<PotionRecipe> AllPotions;

    [Header("Oyuncu Envanteri (E3 Görevi)")]
    // Hangi besinden kaç tane olduğunu tutan yapı
    public Dictionary<HealthyFood, int> PlayerInventory = new Dictionary<HealthyFood, int>();

    void Awake()
    {
        // --- E5 GÖREVİ: İksir tariflerini başlatıyoruz ---
        AllPotions = new List<PotionRecipe>
        {
            new PotionRecipe
            {
                PotionName = "Kök Yakıcı İksir (Root Burn)",
                CombatEffect = "Yavaş Düşmanlara Karşı",
                Description = "Yağları eriten bitkisel enerji dalgası",
                RequiredFoods = new List<HealthyFood> { HealthyFood.YabaniNaneKoku, HealthyFood.Pancar, HealthyFood.BrokoliFilizi }
            },
            new PotionRecipe
            {
                PotionName = "Su Kristali İksiri (Aqua Crystal)",
                CombatEffect = "Şekerli Düşmanlara Karşı",
                Description = "Şekeri emen ozmotik reaksiyon",
                RequiredFoods = new List<HealthyFood> { HealthyFood.AloeVeraDamlalari, HealthyFood.DagSalataligi, HealthyFood.YabanMersini }
            },
            new PotionRecipe
            {
                PotionName = "Kinetik Lif İksiri (Fiber Kinetic)",
                CombatEffect = "Uzak Menzilli Düşmanlara Karşı",
                Description = "Mermileri emen lif kalkanı",
                RequiredFoods = new List<HealthyFood> { HealthyFood.TamTahil, HealthyFood.AltinKetenTohumu, HealthyFood.CitirIspanak }
            },
            new PotionRecipe
            {
                PotionName = "Yeşil Enerji İksiri (Green Energy)",
                CombatEffect = "Dengeli İksir",
                Description = "Süper hız ve çeviklik artışı",
                RequiredFoods = new List<HealthyFood> { HealthyFood.KraliyetAvokadosu, HealthyFood.AromatikKereviz, HealthyFood.KopukluYesilKivi }
            }
        };

        // Oyun başında envanteri hazırlıyoruz (Bütün besinlerden 0 tane var)
        foreach (HealthyFood food in System.Enum.GetValues(typeof(HealthyFood)))
        {
            PlayerInventory[food] = 0;
        }
    }

    // --- E3 GÖREVİ İÇİN EKSTRA: Envantere Besin Ekleme ---
    public void AddFoodToInventory(HealthyFood foodToAdd)
    {
        PlayerInventory[foodToAdd]++;
        Debug.Log(foodToAdd + " toplandı! Mevcut miktar: " + PlayerInventory[foodToAdd]);
    }

    // --- E6 GÖREVİ: Üretim ve Envanter Kontrolü ---
    public bool TryCraftPotion(PotionRecipe recipe)
    {
        // 1. Adım: Gerekli besinler envanterde var mı kontrol et
        foreach (HealthyFood requiredFood in recipe.RequiredFoods)
        {
            if (PlayerInventory[requiredFood] <= 0)
            {
                // Yetersiz malzeme uyarısı gösteriliyor
                Debug.LogWarning("Yetersiz malzeme! " + recipe.PotionName + " üretilemedi. Eksik olan: " + requiredFood);
                return false;
            }
        }

        // 2. Adım: Tüm malzemeler tamsa, bunları envanterden düş
        foreach (HealthyFood requiredFood in recipe.RequiredFoods)
        {
            PlayerInventory[requiredFood]--;
        }

        // Üretim başarılı
        Debug.Log("Başarıyla üretildi: " + recipe.PotionName);
        return true;
    }
}