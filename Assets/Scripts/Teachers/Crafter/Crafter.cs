using UnityEngine;
using System.Collections.Generic;

public class Crafter : MonoBehaviour, IIteractable
{
    [Header("Ingredient 1")]
    public int firstItemID = 1;
    public int firstItemAmount = 1;

    [Header("Ingredient 2")]
    public int secondItemID = 2;
    public int secondItemAmount = 1;

    [Header("Result")]
    public int craftedItemID = 3;

    private InventoryController inventory;
    private ItemDictionary itemDictionary;

    void Start()
    {
        inventory = FindFirstObjectByType<InventoryController>();
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
    }

    public bool CanInteract() => true;

    public void Interact()
    {
        OpenCraftingWindow();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void OpenCraftingWindow()
    {
        var ids = inventory.GetItemIDs();

        int firstItemCount = CountItem(ids, firstItemID);
        int secondItemCount = CountItem(ids, secondItemID);

        Sprite firstIcon = itemDictionary.GetItemPrefab(firstItemID).GetComponent<Item>().icon;
        Sprite secondIcon = itemDictionary.GetItemPrefab(secondItemID).GetComponent<Item>().icon;

        CraftingUI.Instance.Open(
            firstIcon, firstItemCount, firstItemAmount,
            secondIcon, secondItemCount, secondItemAmount
        );
    }

    private int CountItem(List<int> ids, int id)
    {
        int count = 0;
        foreach (var x in ids)
            if (x == id) count++;
        return count;
    }

    public void Craft()
    {
        var ids = inventory.GetItemIDs();

        int countA = CountItem(ids, firstItemID);
        int countB = CountItem(ids, secondItemID);

        bool hasEnoughA = countA >= firstItemAmount;
        bool hasEnoughB = countB >= secondItemAmount;

        if (hasEnoughA && hasEnoughB)
        {
            // Remove ingredients
            for (int i = 0; i < firstItemAmount; i++)
                inventory.RemoveItemByID(firstItemID);

            for (int i = 0; i < secondItemAmount; i++)
                inventory.RemoveItemByID(secondItemID);

            // Get prefab
            GameObject craftedPrefab = itemDictionary.GetItemPrefab(craftedItemID);

            // Drop the item 
            Instantiate(craftedPrefab, transform.position + Vector3.down, Quaternion.identity);

            Sprite craftedIcon = craftedPrefab.GetComponent<Item>().icon;

            CraftingUI.Instance.ShowCraftResult(
                "SUCCESS !   ITEM   CRAFTED !",
                true,
                craftedIcon,
                ""
            );
        }
        else
        {
            CraftingUI.Instance.ShowCraftResult("YOU   DON'T   HAVE   ENOUGH    INGREDIENTS !");
        }
    }
}
