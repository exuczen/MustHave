using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerInBoundsHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event UnityAction<PointerEventData> PointerEnter = delegate { };
    public event UnityAction<PointerEventData> PointerExit = delegate { };

    public bool IsInBounds { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsInBounds = true;
        PointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsInBounds = false;
        PointerExit(eventData);
    }
}
