using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestTeacherUI : MonoBehaviour
{
    public static QuestTeacherUI Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Message")]
    public TMP_Text messageText;

    [Header("Teacher Images")]
    public Image mathTeacher;
    public Image geographyTeacher;

    [Header("Teacher Sprites")]
    public Sprite mathTeacherSprite;
    public Sprite geographyTeacherSprite;

    [Header("Crafting Images")]
    public Image cauldron;

    [Header("Crafting Sprites")]
    public Sprite cauldronSprite;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        cauldron.gameObject.SetActive(false);
        mathTeacher.gameObject.SetActive(false);
        geographyTeacher.gameObject.SetActive(false);
    }

    public void ShowMessage(string text)
    {
        panel.SetActive(true);
        messageText.text = text;
    }

    public void ShowTeachers(bool mathDone, bool geoDone)
    {
        Time.timeScale = 0f;
        panel.SetActive(true);


        cauldron.gameObject.SetActive(false);

        // Only show teachers that are NOT yet done
        if (!mathDone && mathTeacherSprite != null)
        {
            mathTeacher.sprite = mathTeacherSprite;
            mathTeacher.gameObject.SetActive(true);
        }
        else
        {
            mathTeacher.gameObject.SetActive(false);
        }

        if (!geoDone && geographyTeacherSprite != null)
        {
            geographyTeacher.sprite = geographyTeacherSprite;
            geographyTeacher.gameObject.SetActive(true);
        }
        else
        {
            geographyTeacher.gameObject.SetActive(false);
        }
    }

    public void ShowCrafting()
    {
        Time.timeScale = 0f;
        panel.SetActive(true);

        mathTeacher.gameObject.SetActive(false);
        geographyTeacher.gameObject.SetActive(false);

        if (cauldronSprite != null)
        {
            cauldron.sprite = cauldronSprite;
            cauldron.gameObject.SetActive(true);
        }
        else
        {
            cauldron.gameObject.SetActive(false);
        }

    }

    public void ShowFinished()
    {
        panel.SetActive(true);
        messageText.text = "EXCELLENT WORK!";

        mathTeacher.gameObject.SetActive(false);
        geographyTeacher.gameObject.SetActive(false);
        cauldron.gameObject.SetActive(false);

    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
