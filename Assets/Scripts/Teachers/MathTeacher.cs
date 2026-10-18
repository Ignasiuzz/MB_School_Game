using UnityEngine;
using System.Collections;

public class MathTeacher : MonoBehaviour, IIteractable
{
    public GameObject itemPrefab;
    public float giveCooldown = 1f;
    private bool canGive = true;

    public Sprite normalSprite;
    public Sprite talkingSprite;
    private SpriteRenderer spriteRenderer;

    public GameObject MathDialogPanel; // assign MathDialogPanel in Inspector

    private bool isResetting = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool CanInteract()
    {
        return canGive;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        StartMathChallenge();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void StartMathChallenge()
    {   
        canGive = false;

        if (talkingSprite && spriteRenderer)
            spriteRenderer.sprite = talkingSprite;

        int a = Random.Range(1, 100);
        int b = Random.Range(1, 50);

        int correctAnswer = (a == 6 && b == 7) ? 67 : a + b; // 67 meme easter egg

        string question = $"WHAT   IS   {a}  +  {b} ?";

        MathDialogUI.Instance.Show(question, (playerAnswer) =>
        {
            if (int.TryParse(playerAnswer, out int result) && result == correctAnswer)
            {
                StartCoroutine(GiveItemRoutine());
                TowerDefenseManager.Instance?.RegisterQuizAnswer(true);
            }
            else
            {
                MathDialogUI.Instance.ShowMessage("INCORRECT !   TRY   AGAIN   NEXT   TIME   :(");
                TowerDefenseManager.Instance?.RegisterQuizAnswer(false);
                StartCoroutine(ResetSpriteWithDelay());
            }
        });
    }

    private IEnumerator GiveItemRoutine()
    {
        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            droppedItem.GetComponent<BounceEffect>()?.StartBounce();
        }

        QuestManager.Instance.mathCompleted = true;

        MathDialogUI.Instance.ShowMessage("CORRECT !   HERE'S   YOUR   REWARD !");

        // Wait before resetting sprite and allowing next interaction
        yield return StartCoroutine(ResetSpriteWithDelay());
    }

    void Update()
    {
        if (!isResetting && spriteRenderer.sprite == talkingSprite && MathDialogPanel != null && !MathDialogPanel.activeSelf)
        {   
            StartCoroutine(ResetSpriteWithDelay());
        }
    }

    private IEnumerator ResetSpriteWithDelay()
    {
        if (isResetting) yield break;

        isResetting = true;
        yield return new WaitForSeconds(giveCooldown);

        if (normalSprite && spriteRenderer)
            spriteRenderer.sprite = normalSprite;

        canGive = true;
        isResetting = false;
    }
}