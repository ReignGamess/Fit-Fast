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
    [SerializeField] private bool[] isSlotUnlocked = { true, true, true, true };
    private bool[] isSlotOnCooldown = new bool[4]; // Bekleme süresinde olan iksirler

    public static int ActivePotionIndex = 0;

    private int currentSelectedIndex = -1;
    private bool isMenuOpen = false;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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
            // Sadece kilitli OLMAYAN ve BEKLEMEDE OLMAYAN iksirler seçilebilir
            if (isSlotUnlocked[currentSelectedIndex] && !isSlotOnCooldown[currentSelectedIndex])
            {
                ActivePotionIndex = currentSelectedIndex;
                TriggerSelectedPotion(ActivePotionIndex);
            }
            else if (isSlotOnCooldown[currentSelectedIndex])
            {
                Debug.Log("Bu iksir şu an bekleme süresinde!");
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
                duration = 3f; // Can iksiri için 3 sn bekleme
                break;
            case 3:
                manager.UseGreenEnergyPotion();
                duration = manager.speedDuration; 
                break;
        }

        // İksiri belirtilen süre kadar kilitle
        StartCoroutine(StartCooldownRoutine(index, duration));
    }

    private IEnumerator StartCooldownRoutine(int slotIndex, float duration)
    {
        isSlotOnCooldown[slotIndex] = true;

        // İksirin etkisinin sürmesi kadar bekle
        yield return new WaitForSeconds(duration);

        // Süre dolunca tekrar erişime aç
        isSlotOnCooldown[slotIndex] = false;
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
        float startAngle = 100f;
        float endAngle = 260f;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].anchorMin = new Vector2(0.5f, 0.5f);
            slots[i].anchorMax = new Vector2(0.5f, 0.5f);
            slots[i].pivot = new Vector2(0.5f, 0.5f);

            float angle = Mathf.Lerp(startAngle, endAngle, (float)i / Mathf.Max(1, slots.Length - 1));
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
                // Beklemedeyse gri gözüksün ve seçilemesin
                if (isSlotOnCooldown[i])
                {
                    slotImage.color = Color.gray; 
                }
                else if (i == index)
                {
                    slotImage.color = isSlotUnlocked[i] ? new Color(1f, 0.9f, 0.3f, 1f) : Color.red;
                    slots[i].transform.localScale = Vector3.one * 1.25f;
                }
                else
                {
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
        foreach (var slot in slots)
        {
            if (slot != null) slot.gameObject.SetActive(isActive);
        }
    }
}