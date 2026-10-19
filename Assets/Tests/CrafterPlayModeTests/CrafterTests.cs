
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class CrafterTests
{
    private GameObject inventoryObject;
    private GameObject dictionaryObject;
    private GameObject craftingUIObject;
    private GameObject crafterObject;

    private List<GameObject> itemPrefabs;

    private InventoryController inventory;
    private Crafter crafter;
    private TMP_Text resultText;

    private const string SuccessMessage =
        "SUCCESS !   ITEM   CRAFTED !";

    private const string FailureMessage =
        "YOU   DON'T   HAVE   ENOUGH    INGREDIENTS !";


    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // 1. Sukuriami trys testiniai daiktai
        itemPrefabs = new List<GameObject>
        {
            new GameObject(
                "Pencil",
                typeof(RectTransform),
                typeof(Item)
            ),
            new GameObject(
                "Eraser",
                typeof(RectTransform),
                typeof(Item)
            ),
            new GameObject(
                "CraftedItem",
                typeof(RectTransform),
                typeof(Item)
            )
        };

        // 2. Sukuriamas daiktų žodynas
        dictionaryObject = new GameObject("TestDictionary");
        dictionaryObject.SetActive(false);

        ItemDictionary dictionary =
            dictionaryObject.AddComponent<ItemDictionary>();

        dictionary.itemPrefabs = new List<Item>();

        foreach (GameObject prefab in itemPrefabs)
        {
            dictionary.itemPrefabs.Add(
                prefab.GetComponent<Item>()
            );
        }

        dictionaryObject.SetActive(true);

        // 3. Sukuriamas inventorius
        inventoryObject = new GameObject("TestInventory");

        inventory =
            inventoryObject.AddComponent<InventoryController>();

        GameObject panel = new GameObject(
            "InventoryPanel",
            typeof(RectTransform)
        );

        panel.transform.SetParent(
            inventoryObject.transform, false
        );

        inventory.inventoryPanel = panel;

        // Du pradiniai inventoriaus langeliai
        CreateSlot("Slot1");
        CreateSlot("Slot2");

        // Pridedami ingredientai
        Assert.IsTrue(inventory.AddItem(itemPrefabs[0]));
        Assert.IsTrue(inventory.AddItem(itemPrefabs[1]));

        // 4. Sukuriama CraftingUI
        craftingUIObject = new GameObject("TestCraftingUI");
        craftingUIObject.SetActive(false);

        CraftingUI ui =
            craftingUIObject.AddComponent<CraftingUI>();

        ui.panel = new GameObject("CraftingPanel");

        ui.panel.transform.SetParent(
            craftingUIObject.transform, false
        );

        ui.redPotionIcon = CreateImage("FirstIcon");
        ui.bluePotionIcon = CreateImage("SecondIcon");

        ui.redPotionCountText = CreateText("FirstCount");
        ui.bluePotionCountText = CreateText("SecondCount");

        ui.resultText = CreateText("ResultText");
        ui.craftedItemIcon = CreateImage("CraftedItemIcon");

        resultText = ui.resultText;

        craftingUIObject.SetActive(true);

        // 5. Sukuriamas Crafter
        crafterObject = new GameObject("TestCrafter");

        crafter = crafterObject.AddComponent<Crafter>();

        crafter.firstItemID = 1;
        crafter.firstItemAmount = 1;

        crafter.secondItemID = 2;
        crafter.secondItemAmount = 1;

        crafter.craftedItemID = 3;

        // Leidžiame Unity iškviesti Start()
        yield return null;
    }

    private void CreateSlot(string name)
    {
        GameObject slot = new GameObject(
            name,
            typeof(RectTransform),
            typeof(Slot)
        );

        slot.transform.SetParent(
            inventory.inventoryPanel.transform,
            false
        );
    }

    private TMP_Text CreateText(string name)
    {
        GameObject obj = new GameObject(
            name,
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

        obj.transform.SetParent(
            craftingUIObject.transform, false
        );

        return obj.GetComponent<TextMeshProUGUI>();
    }

    private Image CreateImage(string name)
    {
        GameObject obj = new GameObject(
            name,
            typeof(RectTransform),
            typeof(Image)
        );

        obj.transform.SetParent(
            craftingUIObject.transform, false
        );

        return obj.GetComponent<Image>();
    }

    private GameObject GetCraftedItem()
    {
        return GameObject.Find("CraftedItem(Clone)");
    }

    [UnityTest]
    public IEnumerator Craft_WithEnoughIngredients_Succeeds()
    {
        // Arrange
        Assert.AreEqual(2, inventory.GetItemIDs().Count);
        Assert.Contains(1, inventory.GetItemIDs());
        Assert.Contains(2, inventory.GetItemIDs());

        // Act
        crafter.Craft();

        // Assert – ingredientai pašalinti
        Assert.AreEqual(0, inventory.GetItemIDs().Count);

        // Parodytas sėkmės pranešimas
        Assert.AreEqual(SuccessMessage, resultText.text);

        // Sukurtas naujas daiktas
        GameObject craftedItem = GetCraftedItem();

        Assert.IsNotNull(craftedItem);
        Assert.AreEqual(
            3,
            craftedItem.GetComponent<Item>().ID
        );

        yield return null;
    }

    [UnityTest]
    public IEnumerator Craft_MissingFirstIngredient_DoesNotCraft()
    {
        // Arrange – pašalinamas pirmasis ingredientas
        Assert.IsTrue(inventory.RemoveItemByID(1));

        Assert.AreEqual(1, inventory.GetItemIDs().Count);
        Assert.Contains(2, inventory.GetItemIDs());

        // Act
        crafter.Craft();

        // Assert – antras ingredientas turi likti
        Assert.AreEqual(1, inventory.GetItemIDs().Count);
        Assert.Contains(2, inventory.GetItemIDs());

        Assert.AreEqual(FailureMessage, resultText.text);
        Assert.IsNull(GetCraftedItem());

        yield return null;
    }

    [UnityTest]
    public IEnumerator Craft_MissingSecondIngredient_DoesNotCraft()
    {
        // Arrange – pašalinamas antrasis ingredientas
        Assert.IsTrue(inventory.RemoveItemByID(2));

        Assert.AreEqual(1, inventory.GetItemIDs().Count);
        Assert.Contains(1, inventory.GetItemIDs());

        // Act
        crafter.Craft();

        // Assert – pirmas ingredientas turi likti
        Assert.AreEqual(1, inventory.GetItemIDs().Count);
        Assert.Contains(1, inventory.GetItemIDs());

        Assert.AreEqual(FailureMessage, resultText.text);
        Assert.IsNull(GetCraftedItem());

        yield return null;
    }

    [UnityTest]
    public IEnumerator Craft_NoIngredients_DoesNotCraft()
    {
        // Arrange – pašalinami abu ingredientai
        Assert.IsTrue(inventory.RemoveItemByID(1));
        Assert.IsTrue(inventory.RemoveItemByID(2));

        Assert.AreEqual(0, inventory.GetItemIDs().Count);

        // Act
        crafter.Craft();

        // Assert – inventorius lieka tuščias
        Assert.AreEqual(0, inventory.GetItemIDs().Count);

        Assert.AreEqual(FailureMessage, resultText.text);
        Assert.IsNull(GetCraftedItem());

        yield return null;
    }

    [UnityTest]
    public IEnumerator Craft_ExtraIngredients_ConsumesOnlyRequired()
    {
        // Arrange – papildomi du inventoriaus langeliai
        CreateSlot("Slot3");
        CreateSlot("Slot4");

        // Pridedamas papildomas pieštukas ir trintukas
        Assert.IsTrue(inventory.AddItem(itemPrefabs[0]));
        Assert.IsTrue(inventory.AddItem(itemPrefabs[1]));

        // Inventoriuje yra keturi ingredientai
        Assert.AreEqual(4, inventory.GetItemIDs().Count);

        // Konstravimui reikia tik po vieną
        crafter.firstItemAmount = 1;
        crafter.secondItemAmount = 1;

        // Act
        crafter.Craft();

        // Assert – turi likti po vieną ingredientą
        List<int> remaining = inventory.GetItemIDs();

        Assert.AreEqual(2, remaining.Count);
        Assert.Contains(1, remaining);
        Assert.Contains(2, remaining);

        Assert.AreEqual(SuccessMessage, resultText.text);

        GameObject craftedItem = GetCraftedItem();

        Assert.IsNotNull(craftedItem);
        Assert.AreEqual(
            3,
            craftedItem.GetComponent<Item>().ID
        );

        yield return null;
    }

    [UnityTest]
    public IEnumerator Craft_TwoOfEachIngredient_Succeeds()
    {
        // Arrange – pridedami papildomi langeliai
        CreateSlot("Slot3");
        CreateSlot("Slot4");

        Assert.IsTrue(inventory.AddItem(itemPrefabs[0]));
        Assert.IsTrue(inventory.AddItem(itemPrefabs[1]));

        // Reikalaujama po du kiekvieno ingrediento
        crafter.firstItemAmount = 2;
        crafter.secondItemAmount = 2;

        Assert.AreEqual(4, inventory.GetItemIDs().Count);

        // Act
        crafter.Craft();

        // Assert – visi keturi ingredientai sunaudoti
        Assert.AreEqual(0, inventory.GetItemIDs().Count);

        Assert.AreEqual(SuccessMessage, resultText.text);

        GameObject craftedItem = GetCraftedItem();

        Assert.IsNotNull(craftedItem);
        Assert.AreEqual(
            3,
            craftedItem.GetComponent<Item>().ID
        );

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        GameObject craftedItem = GetCraftedItem();

        if (craftedItem != null)
            Object.Destroy(craftedItem);

        if (crafterObject != null)
            Object.Destroy(crafterObject);

        if (craftingUIObject != null)
            Object.Destroy(craftingUIObject);

        if (inventoryObject != null)
            Object.Destroy(inventoryObject);

        if (dictionaryObject != null)
            Object.Destroy(dictionaryObject);

        if (itemPrefabs != null)
        {
            foreach (GameObject prefab in itemPrefabs)
            {
                if (prefab != null)
                    Object.Destroy(prefab);
            }
        }

        CraftingUI.Instance = null;
        Time.timeScale = 1f;

        yield return null;
    }
}
