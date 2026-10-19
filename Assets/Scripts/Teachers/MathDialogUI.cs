using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MathDialogUI : MonoBehaviour
{
    public static MathDialogUI Instance;

    public GameObject panel;
    public TMP_Text questionText;
    public TMP_InputField inputField;
    public Button submitButton;

    private System.Action<string> onSubmit;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string question, System.Action<string> callback)
    {
        Time.timeScale = 0f;

        panel.SetActive(true);
        inputField.text = "";
        questionText.text = question;
        onSubmit = callback;

        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() =>
        {
            StartCoroutine(HideAfterDelay(1f));
            onSubmit?.Invoke(inputField.text);
            Time.timeScale = 1f;
        });
    }

    public void ShowMessage(string message)
    {
        Time.timeScale = 0f;

        panel.SetActive(true);
        questionText.text = message;

        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() =>
        {
            StartCoroutine(HideAfterDelay(1f));
            Time.timeScale = 1f;
        });
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        panel.SetActive(false);
    }
}
