using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Ingredient UI")]
    public Image redPotionIcon;
    public TMP_Text redPotionCountText;

    public Image bluePotionIcon;
    public TMP_Text bluePotionCountText;

    [Header("Result UI")]
    public TMP_Text resultText;

    [Header("Crafted Item UI")]
    public Image craftedItemIcon;

    private string defaultResultText = "";
    private Coroutine revertCoroutine;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        defaultResultText = resultText.text;

        craftedItemIcon.gameObject.SetActive(false);
    }

    public void Open(Sprite redIcon, int redCount, int redNeeded, Sprite blueIcon, int blueCount, int blueNeeded)
    {
        Time.timeScale = 0f;
        panel.SetActive(true);

        redPotionIcon.gameObject.SetActive(true);
        redPotionCountText.gameObject.SetActive(true);
        bluePotionIcon.gameObject.SetActive(true);
        bluePotionCountText.gameObject.SetActive(true);

        craftedItemIcon.gameObject.SetActive(false);

        redPotionIcon.sprite = redIcon;
        bluePotionIcon.sprite = blueIcon;

        redPotionCountText.text = $"{redCount} / {redNeeded}";
        bluePotionCountText.text = $"{blueCount} / {blueNeeded}";

        resultText.text = defaultResultText;
    }

    public void ShowCraftResult(string message, bool success = false, Sprite craftedSprite = null, string craftedName = "")
    {
        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);

        if (success)
        {
            redPotionIcon.gameObject.SetActive(false);
            redPotionCountText.gameObject.SetActive(false);
            bluePotionIcon.gameObject.SetActive(false);
            bluePotionCountText.gameObject.SetActive(false);

            // Show crafted item icon
            if (craftedSprite != null)
            {
                craftedItemIcon.sprite = craftedSprite;
                craftedItemIcon.gameObject.SetActive(true);
            }

            resultText.text = message;

            revertCoroutine = StartCoroutine(AutoCloseAfterSuccess());
        }
        else
        {
            resultText.text = message;
            revertCoroutine = StartCoroutine(RevertResultText());
        }
    }

    private IEnumerator RevertResultText()
    {
        yield return new WaitForSecondsRealtime(2f);

        resultText.text = defaultResultText;
    }

    private IEnumerator AutoCloseAfterSuccess()
    {
        yield return new WaitForSecondsRealtime(1f);
        Close();
    }

    public void Close()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
    }
}
