
using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class QuestGiverTeacherTests
{
    private GameObject managerObject;
    private GameObject inventoryObject;
    private GameObject uiObject;
    private GameObject teacherObject;
    private GameObject finalItemPrefab;

    private QuestManager questManager;
    private InventoryController inventory;
    private QuestGiverTeacher teacher;
    private QuestTeacherUI questUI;

    private Texture2D spriteTexture;
    private Sprite teacherSprite;

    private const string IntroMessage =
        "GO   HELP   THE   MATH   AND   GEOGRAPHY   TEACHERS!";

    private const string BothMissingMessage =
        "YOU   STILL   NEED   TO   HELP   BOTH   TEACHERS!";

    private const string MathMissingMessage =
        "GOOD   PROGRESS!   NOW   GO   HELP  THE   MATH   TEACHER!";

    private const string GeographyMissingMessage =
        "GOOD   PROGRESS!   NOW   GO   HELP   THE   GEOGRAPHY   TEACHER!";

    private const string CraftingMessage =
        "GREAT   WORK!   NOW   FIND   THE   CRAFTER!   IT   IS   LOCATED   OUTSIDE!";

    private const string FinalItemMissingMessage =
        "CRAFTER   IS   LOCATED   OUTSIDE!   CRAFT   THE   FINAL   ITEM!";

    // =========================================
    // TESTINĖS APLINKOS PARUOŠIMAS
    // =========================================

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        Time.timeScale = 1f;

        // 1. Sukuriamas QuestManager
        managerObject = new GameObject("TestQuestManager");
        questManager =
            managerObject.AddComponent<QuestManager>();

        questManager.mathCompleted = false;
        questManager.geographyCompleted = false;
        questManager.craftingQuestStarted = false;

        // 2. Sukuriamas inventorius
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

        GameObject slot = new GameObject(
            "InventorySlot",
            typeof(RectTransform),
            typeof(Slot)
        );

        slot.transform.SetParent(
            panel.transform, false
        );

        // 3. Sukuriamas galutinio daikto prefab
        finalItemPrefab = new GameObject(
            "FinalItem",
            typeof(RectTransform),
            typeof(Item)
        );

        finalItemPrefab.GetComponent<Item>().ID = 3;

        // 4. Sukuriamas QuestTeacherUI
        uiObject = new GameObject(
            "TestQuestUI",
            typeof(RectTransform)
        );

        uiObject.SetActive(false);

        questUI = uiObject.AddComponent<QuestTeacherUI>();

        questUI.panel = new GameObject(
            "QuestPanel",
            typeof(RectTransform)
        );

        questUI.panel.transform.SetParent(
            uiObject.transform, false
        );

        questUI.messageText = CreateText("MessageText");

        questUI.mathTeacher = CreateImage("MathTeacherImage");
        questUI.geographyTeacher =
            CreateImage("GeographyTeacherImage");
        questUI.cauldron = CreateImage("CauldronImage");

        // Aktyvuojame UI, kad įvyktų Awake()
        uiObject.SetActive(true);

        // 5. Sukuriamas direktorius
        teacherObject = new GameObject(
            "TestQuestGiverTeacher",
            typeof(SpriteRenderer),
            typeof(QuestGiverTeacher)
        );

        teacher = teacherObject.GetComponent<QuestGiverTeacher>();
        teacher.finalCraftedItemID = 3;

        // Testinis kalbėjimo sprite
        spriteTexture = new Texture2D(2, 2);

        teacherSprite = Sprite.Create(
            spriteTexture,
            new Rect(0, 0, 2, 2),
            new Vector2(0.5f, 0.5f)
        );

        teacher.normalSprite = teacherSprite;
        teacher.talkingSprite = teacherSprite;

        teacherObject.GetComponent<SpriteRenderer>().flipX = true;

        // Palaukiame Start()
        yield return null;
    }

    // =========================================
    // PAGALBINIAI METODAI
    // =========================================

    private TMP_Text CreateText(string name)
    {
        GameObject obj = new GameObject(
            name,
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

        obj.transform.SetParent(uiObject.transform, false);

        return obj.GetComponent<TextMeshProUGUI>();
    }

    private Image CreateImage(string name)
    {
        GameObject obj = new GameObject(
            name,
            typeof(RectTransform),
            typeof(Image)
        );

        obj.transform.SetParent(uiObject.transform, false);

        return obj.GetComponent<Image>();
    }

    private void CompleteBothTeachers()
    {
        questManager.mathCompleted = true;
        questManager.geographyCompleted = true;
    }

    private void StartCraftingQuest()
    {
        // Pirmasis pokalbis – įžanga
        teacher.Interact();

        // Abu mokytojai jau išgelbėti
        CompleteBothTeachers();

        // Antrasis pokalbis – konstravimo užduotis
        teacher.Interact();
    }

    // =========================================
    // TC-QUEST-001
    // Pirmasis pokalbis
    // =========================================

    [UnityTest]
    public IEnumerator Interact_FirstVisit_ShowsIntroduction()
    {
        // Act
        teacher.Interact();

        // Assert
        Assert.AreEqual(
            IntroMessage,
            questUI.messageText.text
        );

        Assert.IsTrue(questUI.panel.activeSelf);

        Assert.IsFalse(
            questManager.craftingQuestStarted
        );

        // Direktorius turi atsisukti į žaidėją
        Assert.IsFalse(
            teacherObject.GetComponent<SpriteRenderer>().flipX
        );

        yield return null;
    }

    // =========================================
    // TC-QUEST-002
    // Nepadėta nė vienam mokytojui
    // =========================================

    [UnityTest]
    public IEnumerator Interact_NoTeachersCompleted_ShowsReminder()
    {
        // Arrange – pirmą kartą jau kalbėta
        teacher.Interact();

        // Act – kalbamės antrą kartą
        teacher.Interact();

        // Assert
        Assert.AreEqual(
            BothMissingMessage,
            questUI.messageText.text
        );

        Assert.IsFalse(questManager.mathCompleted);
        Assert.IsFalse(questManager.geographyCompleted);

        Assert.IsFalse(
            questManager.craftingQuestStarted
        );

        yield return null;
    }

    // =========================================
    // TC-QUEST-003
    // Nepadėta matematikos mokytojui
    // =========================================

    [UnityTest]
    public IEnumerator Interact_MathIncomplete_ShowsMathReminder()
    {
        // Arrange
        teacher.Interact();

        questManager.geographyCompleted = true;
        questManager.mathCompleted = false;

        // Act
        teacher.Interact();

        // Assert
        Assert.AreEqual(
            MathMissingMessage,
            questUI.messageText.text
        );

        Assert.IsFalse(
            questManager.craftingQuestStarted
        );

        yield return null;
    }

    // =========================================
    // TC-QUEST-004
    // Nepadėta geografijos mokytojui
    // =========================================

    [UnityTest]
    public IEnumerator Interact_GeographyIncomplete_ShowsGeoReminder()
    {
        // Arrange
        teacher.Interact();

        questManager.mathCompleted = true;
        questManager.geographyCompleted = false;

        // Act
        teacher.Interact();

        // Assert
        Assert.AreEqual(
            GeographyMissingMessage,
            questUI.messageText.text
        );

        Assert.IsFalse(
            questManager.craftingQuestStarted
        );

        yield return null;
    }

    // =========================================
    // TC-QUEST-005
    // Abu mokytojai išgelbėti
    // =========================================

    [UnityTest]
    public IEnumerator Interact_BothCompleted_StartsCraftingQuest()
    {
        // Arrange
        teacher.Interact();
        CompleteBothTeachers();

        // Act
        teacher.Interact();

        // Assert
        Assert.IsTrue(
            questManager.craftingQuestStarted
        );

        Assert.AreEqual(
            CraftingMessage,
            questUI.messageText.text
        );

        Assert.IsTrue(questUI.panel.activeSelf);

        yield return null;
    }

    // =========================================
    // TC-QUEST-006
    // Galutinio daikto dar nėra
    // =========================================

    [UnityTest]
    public IEnumerator Interact_FinalItemMissing_ShowsCraftReminder()
    {
        // Arrange – pradedama konstravimo užduotis
        StartCraftingQuest();

        Assert.IsFalse(inventory.HasItem(3));

        // Act – dar kartą kalbamės su direktoriumi
        teacher.Interact();

        // Assert
        Assert.AreEqual(
            FinalItemMissingMessage,
            questUI.messageText.text
        );

        Assert.IsFalse(inventory.HasItem(3));

        Assert.IsTrue(
            questManager.craftingQuestStarted
        );

        yield return null;
    }

    // =========================================
    // TC-QUEST-007
    // Galutinis daiktas yra inventoriuje
    // =========================================

    [UnityTest]
    public IEnumerator Interact_FinalItemPresent_CompletesQuest()
    {
        // Arrange
        StartCraftingQuest();

        // Į inventorių pridedame galutinį daiktą
        Assert.IsTrue(inventory.AddItem(finalItemPrefab));

        Assert.IsTrue(inventory.HasItem(3));

        // Act
        teacher.Interact();

        // Assert – galutinis daiktas sunaudotas
        Assert.IsFalse(inventory.HasItem(3));

        // Parodomas pabaigos pranešimas
        Assert.AreEqual(
            "EXCELLENT WORK!",
            questUI.messageText.text
        );

        Assert.IsTrue(questUI.panel.activeSelf);

        // Originalus kodas suplanuoja EndScene įkėlimą.
        // Teste atšaukiame perėjimą į kitą sceną.
        teacher.CancelInvoke();

        yield return null;
    }

    // =========================================
    // TESTINĖS APLINKOS IŠVALYMAS
    // =========================================

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (teacher != null)
        {
            // Neleidžiame testui pakeisti Unity scenos
            teacher.CancelInvoke();
            teacher.StopAllCoroutines();
        }

        Time.timeScale = 1f;

        if (teacherObject != null)
            Object.Destroy(teacherObject);

        if (uiObject != null)
            Object.Destroy(uiObject);

        if (inventoryObject != null)
            Object.Destroy(inventoryObject);

        if (managerObject != null)
            Object.Destroy(managerObject);

        if (finalItemPrefab != null)
            Object.Destroy(finalItemPrefab);

        if (teacherSprite != null)
            Object.Destroy(teacherSprite);

        if (spriteTexture != null)
            Object.Destroy(spriteTexture);

        QuestManager.Instance = null;
        QuestTeacherUI.Instance = null;

        yield return null;
    }
}
