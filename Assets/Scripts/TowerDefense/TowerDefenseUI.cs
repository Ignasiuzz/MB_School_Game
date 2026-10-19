using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TowerDefenseUI : MonoBehaviour
{
    public static TowerDefenseUI Instance;

    [Header("HUD")]
    public GameObject hudPanel;
    public TMP_Text goldText;
    public TMP_Text waveText;

    [Header("Base Health")]
    public TMP_Text baseHealthText;
    public Image baseHealthFill;    // Image Type = Filled

    [Header("Countdown")]
    public GameObject countdownGroup;
    public TMP_Text countdownText;
    public Button skipButton;

    [Header("Message")]
    public TMP_Text messageText;

    private Coroutine messageRoutine;

    private void Awake()
    {
        Instance = this;

        hudPanel.SetActive(false);
        countdownGroup.SetActive(false);
        messageText.gameObject.SetActive(false);
    }

    void Start()
    {
        skipButton.onClick.AddListener(() =>
        {
            TowerDefenseManager.Instance.waveManager.SkipCountdown();
        });
    }

    public void ShowHUD(bool show)
    {
        hudPanel.SetActive(show);

        if (!show)
        {
            countdownGroup.SetActive(false);
            messageText.gameObject.SetActive(false);
        }
    }

    public void UpdateGold(int gold)
    {
        goldText.text = $"GOLD :   {gold}";
    }

    public void UpdateBaseHealth(int current, int max)
    {
        baseHealthText.text = $"BASE :   {current}  /  {max}";

        if (baseHealthFill != null)
            baseHealthFill.fillAmount = max > 0 ? (float)current / max : 0f;
    }

    public void UpdateWave(int current, int total)
    {
        waveText.text = $"WAVE   {current}  /  {total}";
    }

    public void UpdateCountdown(int seconds, int nextWave)
    {
        countdownGroup.SetActive(true);
        countdownText.text = $"WAVE   {nextWave}   IN   {seconds}";
    }

    public void HideCountdown()
    {
        countdownGroup.SetActive(false);
    }

    public void ShowMessage(string text, float duration)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(MessageRoutine(text, duration));
    }

    private IEnumerator MessageRoutine(string text, float duration)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);

        // Realtime so messages still fade out while a dialog has the game paused
        yield return new WaitForSecondsRealtime(duration);

        messageText.gameObject.SetActive(false);
        messageRoutine = null;
    }
}
