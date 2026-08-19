using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] PlayerControls controls;
    [SerializeField] GameObject radialMenuRoot;
    [SerializeField] RectTransform[] slots;
    [SerializeField] float radius = 150f;
    [SerializeField] float selectThreshold = 40f;

    [Header("Potion Status")]
    [SerializeField] bool[] isSlotUnlocked = { true, true, false, false }; // Hangi iksirlerin üretildiği (açık olduğu)

    // Oyuncunun o an seçtiği ve oyun boyunca aktif olan iksirin indeksi
    public static int ActivePotionIndex = 0; 

    private int currentSelectedIndex = -1;
    private bool isMenuOpen = false;

    void OnEnable()
    {
        controls = new PlayerControls();
        controls.Enable();
        controls.GamePlay.HoldToOpenMenu.started += OnHoldStarted;
        controls.GamePlay.HoldToOpenMenu.canceled += OnHoldEnded;
    }

    void OnDisable()
    {
        controls.Disable();
        controls.GamePlay.HoldToOpenMenu.started -= OnHoldStarted;
        controls.GamePlay.HoldToOpenMenu.canceled -= OnHoldEnded;
    }

    void OnHoldStarted(InputAction.CallbackContext context)
    {
        radialMenuRoot.SetActive(true);
        radialMenuRoot.GetComponent<RectTransform>().position = Mouse.current.position.ReadValue();
        PositionSlots();
        isMenuOpen = true;
        currentSelectedIndex = -1;
    }

    void OnHoldEnded(InputAction.CallbackContext context)
    {
        if (!isMenuOpen) return;

        // Bırakıldığı anda geçerli ve kilidi açık bir slot üzerindeysek aktif iksiri değiştir
        if (currentSelectedIndex != -1 && currentSelectedIndex < slots.Length)
        {
            if (isSlotUnlocked[currentSelectedIndex])
            {
                ActivePotionIndex = currentSelectedIndex;
                Debug.Log($"Aktif İksir Değiştirildi! Yeni Aktif İksir: Slot {ActivePotionIndex}");
                
                // BURAYA: UI'daki aktif iksir ikonunu güncelleyen bir fonksiyon çağırabilirsin.
            }
            else
            {
                Debug.Log("Bu iksir henüz üretilmemiş (Kilitli), değiştirilemedi!");
            }
        }
        else
        {
            Debug.Log("Seçim yapılmadı, mevcut aktif iksir korundu.");
        }

        radialMenuRoot.SetActive(false);
        isMenuOpen = false;
        ResetHighlights();
    }

    void PositionSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            float angle = i * (360f / slots.Length);
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;
            slots[i].anchoredPosition = new Vector2(x, y);
        }
    }

    void Update()
    {
        if (!isMenuOpen) return;

        CheckHoveredSlot();
    }

    void CheckHoveredSlot()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        int closestSlot = -1;
        float minDistance = float.MaxValue;

        // Hangi slotun fareye en yakın olduğunu bul
        for (int i = 0; i < slots.Length; i++)
        {
            Vector2 slotScreenPos = RectTransformUtility.WorldToScreenPoint(null, slots[i].position);
            float distToSlot = Vector2.Distance(mousePos, slotScreenPos);

            if (distToSlot < minDistance)
            {
                minDistance = distToSlot;
                closestSlot = i;
            }
        }

        // Eğer yeterince yakınsa ve eşik içindeyse seç
        if (minDistance <= selectThreshold)
        {
            currentSelectedIndex = closestSlot;
            HighlightSlot(currentSelectedIndex);
        }
        else
        {
            currentSelectedIndex = -1;
            ResetHighlights();
        }
    }

    void HighlightSlot(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();
            if (slotImage != null)
            {
                if (i == index)
                {
                    // Üzerine gelinen slot açıksa yeşil, kilitliyse kırmızı renk olsun
                    slotImage.color = isSlotUnlocked[i] ? Color.green : Color.red;
                }
                else
                {
                    // Diğer slotlar normal renklerine dönsün (Eğer aktif iksirse sarı yapabilirsin)
                    if (i == ActivePotionIndex)
                        slotImage.color = Color.yellow; // Zaten halihazırda seçili olan aktif iksir
                    else
                        slotImage.color = isSlotUnlocked[i] ? Color.white : Color.gray;
                }
            }
        }
    }

    void ResetHighlights()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();
            if (slotImage != null)
            {
                // Menü kapandığında veya boştayken mevcut aktif iksir sarı, diğerleri normal/gri görunsün
                if (i == ActivePotionIndex)
                    slotImage.color = Color.yellow;
                else
                    slotImage.color = isSlotUnlocked[i] ? Color.white : Color.gray;
            }
        }
    }
}