using NUnit.Framework;
using UnityEngine;

public class InventoryControllerTests
{
    private GameObject inventoryObject;
    private GameObject inventoryPanel;
    private GameObject itemPrefab;

    private InventoryController inventory;
    private Slot slot;

    [SetUp]
    public void SetUp()
    {
        // Sukuriamas inventoriaus valdiklis
        inventoryObject = new GameObject("Inventory");
        inventory = inventoryObject.AddComponent<InventoryController>();

        // Sukuriama inventoriaus panelė
        inventoryPanel = new GameObject(
            "InventoryPanel",
            typeof(RectTransform)
        );

        inventory.inventoryPanel = inventoryPanel;

        // Sukuriamas vienas tuščias inventoriaus langelis
        GameObject slotObject = new GameObject(
            "Slot",
            typeof(RectTransform),
            typeof(Slot)
        );

        slotObject.transform.SetParent(
            inventoryPanel.transform, false
        );

        slot = slotObject.GetComponent<Slot>();

        // Sukuriamas testinis daiktas
        itemPrefab = new GameObject(
            "Pencil",
            typeof(RectTransform),
            typeof(Item)
        );

        itemPrefab.GetComponent<Item>().ID = 1;
    }

    [Test] // 1
    public void AddItem_EmptySlot_ReturnsTrue()
    {
        // Act – bandoma pridėti daiktą
        bool result = inventory.AddItem(itemPrefab);

        // Assert – patikrinamas rezultatas
        Assert.IsTrue(result);
        Assert.IsNotNull(slot.currentItem);
        Assert.AreEqual(
            1,
            slot.currentItem.GetComponent<Item>().ID
        );
    }

    [Test] // 2
    public void AddItem_FullInventory_ReturnsFalse()
    {
        // Arrange – užpildome vienintelį inventoriaus langelį
        bool firstResult = inventory.AddItem(itemPrefab);

        GameObject originalItem = slot.currentItem;

        // Act – bandome pridėti antrą daiktą
        bool secondResult = inventory.AddItem(itemPrefab);

        // Assert – tikriname rezultatus
        Assert.IsTrue(firstResult);
        Assert.IsFalse(secondResult);

        // Patikriname, ar pirmasis daiktas liko vietoje
        Assert.AreSame(originalItem, slot.currentItem);

        // Patikriname, ar inventoriuje liko tik vienas daiktas
        Assert.AreEqual(1, inventoryPanel.transform.childCount);
        Assert.AreEqual(1, slot.transform.childCount);
    }

    [Test]
    public void AddItem_PartiallyFilledInventory_ReturnsTrue()
    {
        // Arrange – sukuriame antrą tuščią inventoriaus langelį
        GameObject secondSlotObject = new GameObject(
            "Slot2",
            typeof(RectTransform),
            typeof(Slot)
        );

        secondSlotObject.transform.SetParent(
            inventoryPanel.transform, false
        );

        Slot secondSlot = secondSlotObject.GetComponent<Slot>();

        // Užpildome pirmą inventoriaus langelį
        bool firstResult = inventory.AddItem(itemPrefab);
        GameObject originalItem = slot.currentItem;

        // Act – pridedame dar vieną daiktą
        bool secondResult = inventory.AddItem(itemPrefab);

        // Assert – patikriname rezultatus
        Assert.IsTrue(firstResult);
        Assert.IsTrue(secondResult);

        // Pirmasis daiktas turi likti savo vietoje
        Assert.AreSame(originalItem, slot.currentItem);

        // Antrasis daiktas turi atsirasti laisvame langelyje
        Assert.IsNotNull(secondSlot.currentItem);

        Assert.AreEqual(
            1,
            secondSlot.currentItem.GetComponent<Item>().ID
        );

        // Abu langeliai turi turėti po vieną daiktą
        Assert.AreEqual(1, slot.transform.childCount);
        Assert.AreEqual(1, secondSlot.transform.childCount);
    }

    [Test]
    public void AddItem_SlotComponentMissing_SkipsInvalidSlot()
    {
        // Arrange – sukuriame objektą be Slot komponento
        GameObject invalidSlot = new GameObject(
            "InvalidSlot",
            typeof(RectTransform)
        );

        invalidSlot.transform.SetParent(
            inventoryPanel.transform, false
        );

        // Netinkamą objektą perkeliame į pirmą poziciją
        invalidSlot.transform.SetSiblingIndex(0);

        // Act – bandome pridėti daiktą
        bool result = inventory.AddItem(itemPrefab);

        // Assert – metodas turi sėkmingai pridėti daiktą
        Assert.IsTrue(result);

        // Netinkamame objekte daikto neturi būti
        Assert.AreEqual(0, invalidSlot.transform.childCount);

        // Daiktas turi būti pridėtas į tinkamą langelį
        Assert.IsNotNull(slot.currentItem);

        Assert.AreEqual(
            1,
            slot.currentItem.GetComponent<Item>().ID
        );
    }

    [Test]
    public void AddItem_NoInventorySlots_ReturnsFalse()
    {
        // Arrange – pašaliname vienintelį inventoriaus langelį
        Object.DestroyImmediate(slot.gameObject);

        // Patikriname, kad panelėje neliko langelių
        Assert.AreEqual(0, inventoryPanel.transform.childCount);

        // Act – bandome pridėti daiktą
        bool result = inventory.AddItem(itemPrefab);

        // Assert – daiktas neturi būti pridėtas
        Assert.IsFalse(result);

        // Inventoriaus panelė vis dar turi būti tuščia
        Assert.AreEqual(0, inventoryPanel.transform.childCount);
    }
    
    [TearDown]
    public void TearDown()
    {
        // Pašalinami testavimo metu sukurti objektai
        Object.DestroyImmediate(inventoryObject);
        Object.DestroyImmediate(inventoryPanel);
        Object.DestroyImmediate(itemPrefab);
    }
}