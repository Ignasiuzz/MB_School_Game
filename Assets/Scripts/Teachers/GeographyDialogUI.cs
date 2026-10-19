using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class GeographyDialogUI : MonoBehaviour
{
    public static GeographyDialogUI Instance;

    public GameObject panel;
    public TMP_Text questionText;

    public Button buttonA;
    public Button buttonB;
    public Button buttonC;

    public TMP_Text textA;
    public TMP_Text textB;
    public TMP_Text textC;

    private Action<int> onAnswer;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string question, string optionA, string optionB, string optionC, Action<int> callback)
    {
        Time.timeScale = 0f;

        panel.SetActive(true);
        questionText.text = question;

        buttonA.interactable = true;
        buttonB.interactable = true;
        buttonC.interactable = true;

        textA.text = optionA;
        textB.text = optionB;
        textC.text = optionC;

        onAnswer = callback;

        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
        buttonC.onClick.RemoveAllListeners();

        buttonA.onClick.AddListener(() => Choose(0));
        buttonB.onClick.AddListener(() => Choose(1));
        buttonC.onClick.AddListener(() => Choose(2));
    }

    public void ShowResult(string resultText)
    {
        questionText.text = resultText;

        // Disable buttons so player cannot click more
        buttonA.interactable = false;
        buttonB.interactable = false;
        buttonC.interactable = false;

        StartCoroutine(HideAfterDelay(1f));
        Time.timeScale = 1f;
    }

    private void Choose(int index)
    {
        onAnswer?.Invoke(index);
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        panel.SetActive(false);
    }
}
