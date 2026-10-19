using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TowerBuildUI : MonoBehaviour
{
    public static TowerBuildUI Instance;

    [Header("Panel")]
    public GameObject panel;
    public TMP_Text titleText;

    [Header("Build Options")]
    public GameObject buildGroup;
    public RectTransform optionListParent;
    public Button optionButtonPrefab;   // needs a TMP_Text child
    public List<TowerOption> options;

    [Header("Manage Tower")]
    public GameObject manageGroup;
    public TMP_Text towerInfoText;
    public Button upgradeButton;
    public TMP_Text upgradeButtonText;
    public Button sellButton;
    public TMP_Text sellButtonText;
    public Button targetModeButton;
    public TMP_Text targetModeText;

    private TowerPlot currentPlot;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        sellButton.onClick.AddListener(OnSellClicked);
        targetModeButton.onClick.AddListener(OnTargetModeClicked);
    }

    public void Open(TowerPlot plot)
    {
        currentPlot = plot;

        Time.timeScale = 0f;
        panel.SetActive(true);

        if (plot.HasTower)
            ShowManage();
        else
            ShowBuild();
    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        currentPlot = null;
    }

    private void ShowBuild()
    {
        buildGroup.SetActive(true);
        manageGroup.SetActive(false);
        titleText.text = "BUILD   A   TOWER";

        // Clear old buttons
        foreach (GameObject old in spawnedButtons)
            Destroy(old);
        spawnedButtons.Clear();

        int gold = TowerDefenseManager.Instance.Gold;

        for (int i = 0; i < options.Count; i++)
        {
            TowerOption option = options[i];

            Button button = Instantiate(optionButtonPrefab, optionListParent);
            button.gameObject.SetActive(true);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            label.text = $"{option.towerName}   -   {option.cost}G";

            button.interactable = gold >= option.cost;
            button.onClick.AddListener(() => OnBuildClicked(option));

            spawnedButtons.Add(button.gameObject);
        }
    }

    private void ShowManage()
    {
        buildGroup.SetActive(false);
        manageGroup.SetActive(true);

        Tower tower = currentPlot.CurrentTower;
        TowerLevel stats = tower.Stats;
        int gold = TowerDefenseManager.Instance.Gold;

        titleText.text = tower.towerName.ToUpper();

        towerInfoText.text =
            $"LEVEL   {tower.Level + 1}  /  {tower.levels.Length}\n" +
            $"DAMAGE   {stats.damage}\n" +
            $"RANGE   {stats.range}\n" +
            $"RATE   {stats.fireRate}  /  SEC";

        upgradeButton.gameObject.SetActive(tower.CanUpgrade);
        upgradeButtonText.text = $"UPGRADE   -   {tower.UpgradeCost}G";
        upgradeButton.interactable = gold >= tower.UpgradeCost;

        sellButtonText.text = $"SELL   +{tower.SellValue}G";
        targetModeText.text = $"TARGET :   {tower.targetMode.ToString().ToUpper()}";
    }

    private void OnBuildClicked(TowerOption option)
    {
        if (currentPlot != null && currentPlot.Build(option))
            Close();
    }

    private void OnUpgradeClicked()
    {
        if (currentPlot != null && currentPlot.CurrentTower.Upgrade())
            ShowManage();
    }

    private void OnSellClicked()
    {
        if (currentPlot == null) return;

        currentPlot.Sell();
        Close();
    }

    private void OnTargetModeClicked()
    {
        if (currentPlot == null) return;

        currentPlot.CurrentTower.CycleTargetMode();
        ShowManage();
    }
}
