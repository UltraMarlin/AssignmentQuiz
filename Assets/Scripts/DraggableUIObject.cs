using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUIObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentAfterDrag;
    private Transform originalParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        parentAfterDrag = originalParent;
        transform.SetParent(transform.root);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}