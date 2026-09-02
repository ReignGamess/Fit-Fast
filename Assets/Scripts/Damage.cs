using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Damage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform[] slots;
    [SerializeField] private float radius = 90f;
    [SerializeField] private float selectThreshold = 70f;

    [Header("Potion Durumları")]
    [Tooltip("Inspector'dan hangi slotun açık hangisinin kilitli olduğunu seçebilirsin.")]
    [SerializeField] private bool[] isSlotUnlocked = { true, true, false, false }; // Son ikisi kilitli!
    
    private bool[] isSlotOnCooldown;

    public static int ActivePotionIndex = 0;

    private int currentSelectedIndex = -1;
    private bool isMenuOpen = false;
    private RectTransform rectTransform;

    void Awake()
{
    rectTransform = GetComponent<RectTransform>();

    // Sadece ismi "button" içeren veya üzerinde Button bileşeni olan çocukları slot olarak al
    System.Collections.Generic.List<RectTransform> validSlots = new System.Collections.Generic.List<RectTransform>();

    for (int i = 0; i < transform.childCount; i++)
    {
        Transform child = transform.GetChild(i);
        // Text (TMP) gibi sadece görsel olan elemanları atla, sadece buton olanları al
        if (child.GetComponent<UnityEngine.UI.Button>() != null)
        {
            validSlots.Add(child.GetComponent<RectTransform>());
        }
    }

    int childCount = validSlots.Count;
    slots = validSlots.ToArray();
    isSlotOnCooldown = new bool[childCount];

    // Inspector'daki kilit durumlarını koru ve dizi boyutunu iksir sayısına eşitle
    if (isSlotUnlocked == null || isSlotUnlocked.Length < childCount)
    {
        System.Array.Resize(ref isSlotUnlocked, childCount);
    }
}

    void Start()
    {
        SetSlotsActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetSlotsActive(true);
        PositionSlots();
        isMenuOpen = true;
        currentSelectedIndex = -1;
        UpdateSelection(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isMenuOpen) return;
        UpdateSelection(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isMenuOpen) return;

        if (currentSelectedIndex != -1 && currentSelectedIndex < slots.Length)
        {
            // Kilitli veya Beklemede olan iksir seçilemez
            if (isSlotUnlocked[currentSelectedIndex] && !isSlotOnCooldown[currentSelectedIndex])
            {
                ActivePotionIndex = currentSelectedIndex;
                TriggerSelectedPotion(ActivePotionIndex);
            }
            else if (!isSlotUnlocked[currentSelectedIndex])
            {
                Debug.Log("Bu iksir henüz kilitli!");
            }
            else if (isSlotOnCooldown[currentSelectedIndex])
            {
                Debug.Log("Bu iksir bekleme süresinde!");
            }
        }

        SetSlotsActive(false);
        isMenuOpen = false;
        ResetHighlights();
    }

    private void TriggerSelectedPotion(int index)
    {
        PotionEffectManager manager = FindFirstObjectByType<PotionEffectManager>();
        if (manager == null) return;

        float duration = 0f;

        switch (index)
        {
            case 0:
                manager.UseRootBurnPotion();
                duration = manager.damageDuration; 
                break;
            case 1:
                manager.UseFiberKineticPotion();
                duration = manager.shieldDuration; 
                break;
            case 2:
                manager.UseAquaCrystalPotion();
                duration = 3f; 
                break;
            case 3:
                manager.UseGreenEnergyPotion();
                duration = manager.speedDuration; 
                break;
        }

        StartCoroutine(StartCooldownRoutine(index, duration));
    }

    private IEnumerator StartCooldownRoutine(int slotIndex, float duration)
    {
        if (slotIndex >= 0 && slotIndex < isSlotOnCooldown.Length)
        {
            isSlotOnCooldown[slotIndex] = true;
            yield return new WaitForSeconds(duration);
            isSlotOnCooldown[slotIndex] = false;
        }
    }

    private void UpdateSelection(Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out Vector2 localPoint);

        int closestSlot = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < slots.Length; i++)
        {
            float distToSlot = Vector2.Distance(localPoint, slots[i].anchoredPosition);
            if (distToSlot < minDistance)
            {
                minDistance = distToSlot;
                closestSlot = i;
            }
        }

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

    void PositionSlots()
{
    int count = slots.Length;
    if (count == 0) return;

    // 360 dereceyi slot sayısına eşit böl
    float angleStep = 360f / count;

    for (int i = 0; i < count; i++)
    {
        slots[i].anchorMin = new Vector2(0.5f, 0.5f);
        slots[i].anchorMax = new Vector2(0.5f, 0.5f);
        slots[i].pivot = new Vector2(0.5f, 0.5f);

        // Açı hesaplama (İsteğe göre başlangıç açısı eklenebilir)
        float angle = i * angleStep;
        float rad = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad) * radius;
        float y = Mathf.Sin(rad) * radius;

        slots[i].anchoredPosition = new Vector2(x, y);
        slots[i].SetAsLastSibling();
    }
}

    void HighlightSlot(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();
            if (slotImage != null)
            {
                if (isSlotOnCooldown[i])
                {
                    slotImage.color = Color.gray; 
                }
                else if (i == index)
                {
                    // Açık ise sarımsı vurgu, kilitli ise kırmızı vurgu
                    slotImage.color = isSlotUnlocked[i] ? new Color(1f, 0.9f, 0.3f, 1f) : Color.red;
                    slots[i].transform.localScale = Vector3.one * 1.25f;
                }
                else
                {
                    // Normal duruş: Açık ise beyaz, Kilitli ise gri
                    slotImage.color = isSlotUnlocked[i] ? Color.white : Color.gray;
                    slots[i].transform.localScale = Vector3.one;
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
                slotImage.color = isSlotOnCooldown[i] ? Color.gray : (isSlotUnlocked[i] ? Color.white : Color.gray);
                slots[i].transform.localScale = Vector3.one;
            }
        }
    }

    private void SetSlotsActive(bool isActive)
    {
        if (slots == null) return;

        foreach (var slot in slots)
        {
            if (slot != null) slot.gameObject.SetActive(isActive);
        }
    }
}