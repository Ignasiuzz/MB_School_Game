
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

public class ItemDragHandlerTests
{
    private GameObject canvasObject;
    private GameObject eventSystemObject;
    private GameObject itemObject;

    private Slot sourceSlot;
    private Slot destinationSlot;

    private ItemDragHandler dragHandler;
    private EventSystem eventSystem;

    // =========================================
    // TESTINĖS APLINKOS PARUOŠIMAS
    // =========================================

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Sukuriamas Canvas
        canvasObject = new GameObject(
            "TestCanvas",
            typeof(RectTransform),
            typeof(Canvas)
        );

        canvasObject.GetComponent<Canvas>().renderMode =
            RenderMode.ScreenSpaceOverlay;

        // Sukuriamas EventSystem, jei jo nėra
        eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            eventSystemObject = new GameObject(
                "TestEventSystem",
                typeof(EventSystem)
            );

            eventSystem =
                eventSystemObject.GetComponent<EventSystem>();
        }

        // Pirmasis inventoriaus langelis
        GameObject sourceObject = new GameObject(
            "SourceSlot",
            typeof(RectTransform),
            typeof(Slot)
        );

        sourceObject.transform.SetParent(
            canvasObject.transform, false
        );

        sourceSlot = sourceObject.GetComponent<Slot>();

        // Antrasis inventoriaus langelis
        GameObject destinationObject = new GameObject(
            "DestinationSlot",
            typeof(RectTransform),
            typeof(Slot)
        );

        destinationObject.transform.SetParent(
            canvasObject.transform, false
        );

        destinationSlot =
            destinationObject.GetComponent<Slot>();

        // Sukuriamas perkeliamas daiktas
        itemObject = new GameObject(
            "TestItem",
            typeof(RectTransform),
            typeof(CanvasGroup),
            typeof(ItemDragHandler)
        );

        itemObject.transform.SetParent(
            sourceSlot.transform, false
        );

        sourceSlot.currentItem = itemObject;

        dragHandler =
            itemObject.GetComponent<ItemDragHandler>();

        // Leidžiame Unity įvykdyti Start()
        yield return null;
    }

    // =========================================
    // PAGALBINIAI METODAI
    // =========================================

    private PointerEventData CreateEvent(GameObject target)
    {
        PointerEventData data =
            new PointerEventData(eventSystem);

        data.pointerEnter = target;

        return data;
    }

    private GameObject CreateDestinationItem()
    {
        GameObject targetItem = new GameObject(
            "TargetItem",
            typeof(RectTransform)
        );

        targetItem.transform.SetParent(
            destinationSlot.transform, false
        );

        destinationSlot.currentItem = targetItem;

        return targetItem;
    }

    private void AssertVisualStateRestored()
    {
        CanvasGroup group =
            itemObject.GetComponent<CanvasGroup>();

        Assert.IsTrue(group.blocksRaycasts);
        Assert.AreEqual(1f, group.alpha);
    }

    // =========================================
    // TC-DRAG-001
    // Perkėlimas į tuščią langelį
    // =========================================

    [UnityTest]
    public IEnumerator OnEndDrag_ToEmptySlot_MovesItem()
    {
        // Arrange
        Assert.IsNull(destinationSlot.currentItem);

        PointerEventData data =
            CreateEvent(destinationSlot.gameObject);

        // Act
        dragHandler.OnBeginDrag(data);
        dragHandler.OnEndDrag(data);

        // Assert
        Assert.IsNull(sourceSlot.currentItem);

        Assert.AreSame(
            itemObject,
            destinationSlot.currentItem
        );

        Assert.AreEqual(
            destinationSlot.transform,
            itemObject.transform.parent
        );

        AssertVisualStateRestored();

        Assert.AreEqual(
            Vector2.zero,
            itemObject.GetComponent<RectTransform>().anchoredPosition
        );

        yield return null;
    }

    // =========================================
    // TC-DRAG-002
    // Dviejų daiktų sukeitimas vietomis
    // =========================================

    [UnityTest]
    public IEnumerator OnEndDrag_ToOccupiedSlot_SwapsItems()
    {
        // Arrange
        GameObject targetItem = CreateDestinationItem();

        PointerEventData data =
            CreateEvent(destinationSlot.gameObject);

        // Act
        dragHandler.OnBeginDrag(data);
        dragHandler.OnEndDrag(data);

        // Assert – pirmame langelyje yra antras daiktas
        Assert.AreSame(
            targetItem,
            sourceSlot.currentItem
        );

        // Antrame langelyje yra perkeliamas daiktas
        Assert.AreSame(
            itemObject,
            destinationSlot.currentItem
        );

        // Tikrinami daiktų tėviniai objektai
        Assert.AreEqual(
            sourceSlot.transform,
            targetItem.transform.parent
        );

        Assert.AreEqual(
            destinationSlot.transform,
            itemObject.transform.parent
        );

        Assert.AreEqual(
            Vector2.zero,
            targetItem.GetComponent<RectTransform>().anchoredPosition
        );

        AssertVisualStateRestored();

        yield return null;
    }

    // =========================================
    // TC-DRAG-003
    // Numetimas ant kito daikto
    // =========================================

    [UnityTest]
    public IEnumerator OnEndDrag_OntoAnotherItem_SwapsItems()
    {
        // Arrange
        GameObject targetItem = CreateDestinationItem();

        // Pelytė yra virš daikto, o ne paties Slot
        PointerEventData data = CreateEvent(targetItem);

        // Act
        dragHandler.OnBeginDrag(data);
        dragHandler.OnEndDrag(data);

        // Assert
        Assert.AreSame(
            targetItem,
            sourceSlot.currentItem
        );

        Assert.AreSame(
            itemObject,
            destinationSlot.currentItem
        );

        Assert.AreEqual(
            sourceSlot.transform,
            targetItem.transform.parent
        );

        Assert.AreEqual(
            destinationSlot.transform,
            itemObject.transform.parent
        );

        AssertVisualStateRestored();

        yield return null;
    }

    // =========================================
    // TC-DRAG-004
    // Numetimas ant vidinio Slot objekto
    // =========================================

    [UnityTest]
    public IEnumerator OnEndDrag_OntoSlotChild_MovesItem()
    {
        // Arrange – sukuriamas objektas langelio viduje
        GameObject childObject = new GameObject(
            "SlotChild",
            typeof(RectTransform)
        );

        childObject.transform.SetParent(
            destinationSlot.transform, false
        );

        // Pelės pozicija yra virš vidinio objekto
        PointerEventData data = CreateEvent(childObject);

        // Act
        dragHandler.OnBeginDrag(data);
        dragHandler.OnEndDrag(data);

        // Assert
        Assert.IsNull(sourceSlot.currentItem);

        Assert.AreSame(
            itemObject,
            destinationSlot.currentItem
        );

        Assert.AreEqual(
            destinationSlot.transform,
            itemObject.transform.parent
        );

        AssertVisualStateRestored();

        yield return null;
    }

    // =========================================
    // TC-DRAG-005
    // Numetimas ne inventoriaus langelyje
    // =========================================

    [UnityTest]
    public IEnumerator OnEndDrag_OutsideSlot_ReturnsToOriginal()
    {
        // Arrange – sukuriama netinkama numetimo vieta
        GameObject outsideObject = new GameObject(
            "OutsideArea",
            typeof(RectTransform)
        );

        outsideObject.transform.SetParent(
            canvasObject.transform, false
        );

        PointerEventData data =
            CreateEvent(outsideObject);

        // Act
        dragHandler.OnBeginDrag(data);
        dragHandler.OnEndDrag(data);

        // Assert – daiktas lieka pradiniame langelyje
        Assert.AreSame(
            itemObject,
            sourceSlot.currentItem
        );

        Assert.IsNull(destinationSlot.currentItem);

        Assert.AreEqual(
            sourceSlot.transform,
            itemObject.transform.parent
        );

        AssertVisualStateRestored();

        yield return null;
    }

    // =========================================
    // TC-DRAG-006
    // Numetimas tame pačiame langelyje
    // =========================================

    [UnityTest]
    public IEnumerator OnEndDrag_SameSlot_KeepsItem()
    {
        // Arrange
        PointerEventData data =
            CreateEvent(sourceSlot.gameObject);

        // Act
        dragHandler.OnBeginDrag(data);
        dragHandler.OnEndDrag(data);

        // Assert – inventoriaus būsena nesikeičia
        Assert.AreSame(
            itemObject,
            sourceSlot.currentItem
        );

        Assert.IsNull(destinationSlot.currentItem);

        Assert.AreEqual(
            sourceSlot.transform,
            itemObject.transform.parent
        );

        AssertVisualStateRestored();

        yield return null;
    }

    // =========================================
    // TC-DRAG-007
    // ERROR-BASED: pointerEnter yra null
    // =========================================

    [UnityTest]
    public IEnumerator OnEndDrag_NullTarget_DocumentsExistingBug()
    {
        // Arrange – pelė nėra virš jokio objekto
        PointerEventData data = CreateEvent(null);

        dragHandler.OnBeginDrag(data);

        // Act + Assert
        // Originaliame kode tikimasi NullReferenceException.
        // Tai dokumentuoja esamą defektą.
        Assert.Throws<System.NullReferenceException>(
            () => dragHandler.OnEndDrag(data)
        );

        // Po klaidos vizualinė būsena jau atkurta,
        // tačiau daiktas negrąžintas į pradinį Slot.
        AssertVisualStateRestored();

        yield return null;
    }


[UnityTest]
public IEnumerator OnDrag_MouseMovement_UpdatesItemPosition()
{
    // Arrange – nustatome pelės koordinates
    PointerEventData data = CreateEvent(null);
    data.position = new Vector2(120f, 85f);

    // Act – imituojame pelės judėjimą
    dragHandler.OnDrag(data);

    // Assert – patikriname daikto poziciją
    Vector3 expectedPosition = new Vector3(120f, 85f, 0f);

    Assert.AreEqual(
        expectedPosition,
        itemObject.transform.position
    );

    yield return null;
}


    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (canvasObject != null)
            Object.Destroy(canvasObject);

        if (eventSystemObject != null)
            Object.Destroy(eventSystemObject);

        yield return null;
    }
}
