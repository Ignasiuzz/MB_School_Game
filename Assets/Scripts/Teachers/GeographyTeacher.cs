using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeographyTeacher : MonoBehaviour, IIteractable
{
    public GameObject itemPrefab;

    public float giveCooldown = 1f;
    private bool canGive = true;

    public Sprite normalSprite;
    public Sprite talkingSprite;
    private SpriteRenderer spriteRenderer;

    public GameObject GeographyDialogPanel; // assign GeographyDialogPanel in Inspector
    private bool isResetting = false;

    [System.Serializable]
    public struct CountryData
    {
        public string country;
        public string capital;
    }

    // The data of the countries
    private CountryData[] data = new CountryData[] {
    new CountryData { country = "Albania", capital = "Tirana" },
    new CountryData { country = "Andorra", capital = "Andorra    la    Vella" },
    new CountryData { country = "Armenia", capital = "Yerevan" },
    new CountryData { country = "Austria", capital = "Vienna" },
    new CountryData { country = "Azerbaijan", capital = "Baku" },
    new CountryData { country = "Belarus", capital = "Minsk" },
    new CountryData { country = "Belgium", capital = "Brussels" },
    new CountryData { country = "Bosnia    and    Herzegovina", capital = "Sarajevo" },
    new CountryData { country = "Bulgaria", capital = "Sofia" },
    new CountryData { country = "Croatia", capital = "Zagreb" },
    new CountryData { country = "Cyprus", capital = "Nicosia" },
    new CountryData { country = "Czechia", capital = "Prague" },
    new CountryData { country = "Denmark", capital = "Copenhagen" },
    new CountryData { country = "Estonia", capital = "Tallinn" },
    new CountryData { country = "Finland", capital = "Helsinki" },
    new CountryData { country = "France", capital = "Paris" },
    new CountryData { country = "Georgia", capital = "Tbilisi" },
    new CountryData { country = "Germany", capital = "Berlin" },
    new CountryData { country = "Greece", capital = "Athens" },
    new CountryData { country = "Hungary", capital = "Budapest" },
    new CountryData { country = "Iceland", capital = "Reykjavik" },
    new CountryData { country = "Ireland", capital = "Dublin" },
    new CountryData { country = "Italy", capital = "Rome" },
    new CountryData { country = "Kazakhstan", capital = "Astana" },
    new CountryData { country = "Kosovo", capital = "Pristina" },
    new CountryData { country = "Latvia", capital = "Riga" },
    new CountryData { country = "Liechtenstein", capital = "Vaduz" },
    new CountryData { country = "Lithuania", capital = "Vilnius" },
    new CountryData { country = "Luxembourg", capital = "Luxembourg" },
    new CountryData { country = "Malta", capital = "Valletta" },
    new CountryData { country = "Moldova", capital = "Chisinau" },
    new CountryData { country = "Monaco", capital = "Monaco" },
    new CountryData { country = "Montenegro", capital = "Podgorica" },
    new CountryData { country = "Netherlands", capital = "Amsterdam" },
    new CountryData { country = "North Macedonia", capital = "Skopje" },
    new CountryData { country = "Norway", capital = "Oslo" },
    new CountryData { country = "Poland", capital = "Warsaw" },
    new CountryData { country = "Portugal", capital = "Lisbon" },
    new CountryData { country = "Romania", capital = "Bucharest" },
    new CountryData { country = "Russia", capital = "Moscow" },
    new CountryData { country = "San Marino", capital = "San Marino" },
    new CountryData { country = "Serbia", capital = "Belgrade" },
    new CountryData { country = "Slovakia", capital = "Bratislava" },
    new CountryData { country = "Slovenia", capital = "Ljubljana" },
    new CountryData { country = "Spain", capital = "Madrid" },
    new CountryData { country = "Sweden", capital = "Stockholm" },
    new CountryData { country = "Switzerland", capital = "Bern" },
    new CountryData { country = "Turkey", capital = "Ankara" },
    new CountryData { country = "Ukraine", capital = "Kyiv" },
    new CountryData { country = "United Kingdom", capital = "London" },
    new CountryData { country = "Vatican City", capital = "Vatican    City" }
    };

    private List<string> allCapitals = new List<string>();

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        foreach (var entry in data)
            allCapitals.Add(entry.capital);
    }

    public bool CanInteract() => canGive;

    public void Interact()
    {
        if (!canGive) return;
        StartQuestion();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void StartQuestion()
    {   
        canGive = false;

        if (talkingSprite) spriteRenderer.sprite = talkingSprite;

        // Pick random country
        int index = Random.Range(0, data.Length);
        var question = data[index];

        // Generate 3 randomized choices
        var options = GenerateOptions(question.capital);
        string[] optionTexts = options.options;
        int correctIndex = options.correctIndex;

        GeographyDialogUI.Instance.Show(
            $"WHAT   IS   THE   CAPITAL   OF   {question.country} ?",
            optionTexts[0],
            optionTexts[1],
            optionTexts[2],
            (chosen) =>
            {
                if (chosen == correctIndex)
                {
                    StartCoroutine(GiveRewardRoutine());
                    TowerDefenseManager.Instance?.RegisterQuizAnswer(true);
                }
                else
                {
                    TowerDefenseManager.Instance?.RegisterQuizAnswer(false);
                    GeographyDialogUI.Instance.ShowResult("WRONG !   TRY   AGAIN   NEXT   TIME   :(");
                    StartCoroutine(ResetSpriteWithDelay());
                }
            });
    }

    private (string[] options, int correctIndex) GenerateOptions(string correctCapital)
    {
        List<string> pool = new List<string>(allCapitals);

        pool.Remove(correctCapital);

        string wrong1 = pool[Random.Range(0, pool.Count)];
        pool.Remove(wrong1);
        string wrong2 = pool[Random.Range(0, pool.Count)];

        List<string> options = new List<string> { correctCapital, wrong1, wrong2 };

        for (int i = 0; i < options.Count; i++)
        {
            int r = Random.Range(i, options.Count);
            (options[i], options[r]) = (options[r], options[i]);
        }

        int correctIndex = options.IndexOf(correctCapital);

        return (options.ToArray(), correctIndex);
    }

    private IEnumerator GiveRewardRoutine()
    {
        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            droppedItem.GetComponent<BounceEffect>()?.StartBounce();
        }

        QuestManager.Instance.geographyCompleted = true;
        
        GeographyDialogUI.Instance.ShowResult("CORRECT !   HERE'S   OUR   REWARD !");

        // Wait before resetting sprite and allowing next interaction
        yield return StartCoroutine(ResetSpriteWithDelay());
    }

    void Update()
    {
        if (!isResetting && spriteRenderer.sprite == talkingSprite && GeographyDialogPanel != null && !GeographyDialogPanel.activeSelf)
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
