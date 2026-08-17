using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform handle; // Joystick içindeki hareket eden kısım
    private Vector2 joystickVec;

    public Vector2 GetInput()
    {
        return joystickVec;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / GetComponent<RectTransform>().sizeDelta.x);
            pos.y = (pos.y / GetComponent<RectTransform>().sizeDelta.y);

            joystickVec = new Vector2(pos.x * 2, pos.y * 2);
            joystickVec = (joystickVec.magnitude > 1.0f) ? joystickVec.normalized : joystickVec;

            if (handle != null)
            {
                handle.anchoredPosition = new Vector2(joystickVec.x * (GetComponent<RectTransform>().sizeDelta.x / 2), joystickVec.y * (GetComponent<RectTransform>().sizeDelta.y / 2));
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickVec = Vector2.zero;
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
}