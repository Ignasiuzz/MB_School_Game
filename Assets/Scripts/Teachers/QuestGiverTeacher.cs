using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuestGiverTeacher : MonoBehaviour, IIteractable
{
    [Header("Final Item")]
    public int finalCraftedItemID;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite talkingSprite;

    private SpriteRenderer spriteRenderer;
    private InventoryController inventory;

    private bool introDone = false;
    private bool toldToCraft = false;
    private bool isResetting = false;

    void Start()
    {
        inventory = FindFirstObjectByType<InventoryController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool CanInteract() => true;

    public Transform GetTransform() => transform;

    public void Interact()
    {
        if (spriteRenderer)
            spriteRenderer.flipX = false;

        SetTalking();

        if (!introDone)
        {
            introDone = true;

            QuestTeacherUI.Instance.ShowMessage(
                "GO   HELP   THE   MATH   AND   GEOGRAPHY   TEACHERS!"
            );

            UpdateUI();
            StartCoroutine(ResetSpriteWithDelay());
            return;
        }

        if (!QuestManager.Instance.TeachersCompleted())
        {
            QuestTeacherUI.Instance.ShowMessage(
                GetRemainingTeachersMessage()
            );

            UpdateUI();
            StartCoroutine(ResetSpriteWithDelay());
            return;
        }

        if (!toldToCraft)
        {
            toldToCraft = true;
            QuestManager.Instance.craftingQuestStarted = true;

            QuestTeacherUI.Instance.ShowMessage(
                "GREAT   WORK!   NOW   FIND   THE   CRAFTER!   IT   IS   LOCATED   OUTSIDE!"
            );

            UpdateUI();
            StartCoroutine(ResetSpriteWithDelay());
            return;
        }

        if (inventory.HasItem(finalCraftedItemID))
        {
            inventory.RemoveItemByID(finalCraftedItemID);

            QuestTeacherUI.Instance.ShowFinished();

            StartCoroutine(ResetSpriteWithDelay());
            Invoke(nameof(LoadEnd), 2f);
        }
        else
        {
            QuestTeacherUI.Instance.ShowMessage(
                "CRAFTER   IS   LOCATED   OUTSIDE!   CRAFT   THE   FINAL   ITEM!"
            );

            StartCoroutine(ResetSpriteWithDelay());
        }
    }

    string GetRemainingTeachersMessage()
    {
        bool mathDone = QuestManager.Instance.mathCompleted;
        bool geoDone = QuestManager.Instance.geographyCompleted;

        if (!mathDone && !geoDone)
            return "YOU   STILL   NEED   TO   HELP   BOTH   TEACHERS!";

        if (!mathDone)
            return "GOOD   PROGRESS!   NOW   GO   HELP  THE   MATH   TEACHER!";

        if (!geoDone)
            return "GOOD   PROGRESS!   NOW   GO   HELP   THE   GEOGRAPHY   TEACHER!";
  
        return "";
    }

    void SetTalking()
    {
        if (talkingSprite && spriteRenderer)
            spriteRenderer.sprite = talkingSprite;
    }

    void UpdateUI()
    {
        if (!QuestManager.Instance.TeachersCompleted())
        {
            QuestTeacherUI.Instance.ShowTeachers(
                QuestManager.Instance.mathCompleted,
                QuestManager.Instance.geographyCompleted
            );
            return;
        }

        if (!QuestManager.Instance.craftingQuestStarted)
            return;

        QuestTeacherUI.Instance.ShowCrafting();
    }

    private IEnumerator ResetSpriteWithDelay()
    {
        if (isResetting) yield break;

        isResetting = true;
        yield return new WaitForSeconds(0.5f);

        if (normalSprite && spriteRenderer)
            spriteRenderer.sprite = normalSprite;

        isResetting = false;
    }

    void LoadEnd()
    {
        Time.timeScale = 1f;

        SaveController saveController = FindFirstObjectByType<SaveController>();
        if (saveController != null)
            saveController.SaveGame();

        SceneManager.LoadScene("EndScene");
    }

}
