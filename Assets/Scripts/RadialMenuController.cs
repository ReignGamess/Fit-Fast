using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] PlayerControls controls;
    [SerializeField] GameObject radialMenuRoot;
    [SerializeField] RectTransform[] slots;
    [SerializeField] float radius=150f;

    void OnEnable()
    {
        controls= new PlayerControls();
        controls.Enable();
        controls.GamePlay.HoldToOpenMenu.started += OnHoldStarted;
        controls.GamePlay.HoldToOpenMenu.canceled += OnHoldEnded;
    }

    void OnHoldStarted(InputAction.CallbackContext context)
    {
        radialMenuRoot.SetActive(true);
        radialMenuRoot.GetComponent<RectTransform>().position=Mouse.current.position.ReadValue();
        PositionSlots();
    }

    void OnHoldEnded(InputAction.CallbackContext context)
    {
        radialMenuRoot.SetActive(false);
    }

void PositionSlots()
    {
        for(int i = 0; i < 4; i++)
        {
            
            float angle = i * 90f;
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;
            slots[i].anchoredPosition = new Vector2(x, y);

        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
