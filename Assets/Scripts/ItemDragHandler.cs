using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // Save OG parent
        transform.SetParent(transform.root); // Above other canvas
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; // Semi transparent drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Enables raycats
        canvasGroup.alpha = 1f; // No longer transparent

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); // Slot where item is dropped
        if(dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if(dropSlot == null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if(dropSlot != null)
        {
            if(dropSlot.currentItem != null)
            {
                // Slot has an item - swap items
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;
            }

            // Move intem into drop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // No slot under drop point
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
