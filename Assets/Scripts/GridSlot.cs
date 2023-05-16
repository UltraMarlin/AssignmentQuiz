using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridSlot : MonoBehaviour, IDropHandler
{
    private readonly PointerEventData.InputButton interactionMouseButton = PointerEventData.InputButton.Left;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != interactionMouseButton)
            return;

        GameObject dropped = eventData.pointerDrag;
        DraggableUIObject draggableUIObject = dropped.GetComponent<DraggableUIObject>();

        // If the slot is occupied
        if (transform.childCount > 0)
        {
            // Get the current child of the slot
            Transform currentChild = transform.GetChild(0);
            DraggableUIObject currentChildDraggable = currentChild.GetComponent<DraggableUIObject>();

            // If the current child is draggable
            if (currentChildDraggable != null)
            {
                // Move it to the original parent of the dropped object
                currentChild.SetParent(draggableUIObject.originalParent);
                currentChildDraggable.parentAfterDrag = draggableUIObject.originalParent;
            }
        }

        // Move the dropped object to the current slot
        draggableUIObject.parentAfterDrag = transform;
    }
}
