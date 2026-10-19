using UnityEngine;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    [Header("UI Setup")]
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    [Header("Item Prefabs (optional)")]
    public GameObject[] itemPrefabs;

    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
    }

    private void InitializeSlots()
    {
        // Clear existing slots (if any)
        foreach (Transform child in inventoryPanel.transform)
            Destroy(child.gameObject);

        // Create fresh empty slots
        for (int i = 0; i < slotCount; i++)
            Instantiate(slotPrefab, inventoryPanel.transform);
    }

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                slot.currentItem = newItem;
                return true;
            }
        }

        Debug.Log("Inventory full!");
        return false;
    }

    public bool AddItemByPrefab(GameObject prefab)
    {
        return AddItem(prefab);
    }

    public bool RemoveItemByID(int itemID)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null && item.ID == itemID)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                    return true;
                }
            }
        }

        return false;
    }

    public List<int> GetItemIDs()
    {
        List<int> ids = new List<int>();

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                    ids.Add(item.ID);
            }
        }
        return ids;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex()
                });
            }
        }

        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        InitializeSlots(); // Reset slots

        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Transform slotTransform = inventoryPanel.transform.GetChild(data.slotIndex);
                Slot slot = slotTransform.GetComponent<Slot>();

                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slotTransform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    slot.currentItem = item;
                }
            }
            else
            {
                Debug.LogWarning("Inventory load error: slot index out of range.");
            }
        }
    }

    public bool HasItem(int id)
    {
        return GetItemIDs().Contains(id);
    }
}
