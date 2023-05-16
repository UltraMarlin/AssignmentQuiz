using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUIObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentAfterDrag;
    public Transform originalParent;
    private Vector2 dragSize = new Vector2(120, 120);
    private readonly PointerEventData.InputButton interactionMouseButton = PointerEventData.InputButton.Left;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != interactionMouseButton)
            return;

        originalParent = transform.parent;
        parentAfterDrag = originalParent;
        transform.SetParent(transform.root);
        (transform as RectTransform).sizeDelta = dragSize;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != interactionMouseButton)
            return;

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != interactionMouseButton)
            return;

        transform.SetParent(parentAfterDrag);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

}