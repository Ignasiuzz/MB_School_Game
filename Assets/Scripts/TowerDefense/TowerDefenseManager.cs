using UnityEngine;
using System;
using System.Collections;

public class TowerDefenseManager : MonoBehaviour
{
    public static TowerDefenseManager Instance;

    [Header("Starting Values")]
    public int startingGold = 150;
    public int maxBaseHealth = 20;

    [Header("Quiz Bonus")]
    public int quizBonusBase = 20;
    public int quizBonusPerStreak = 5;
    public int maxQuizStreak = 5;

    [Header("References")]
    public WaveManager waveManager;

    // QuestGiverTeacher listens to this to finish the game
    public event Action OnDefenseWon;

    public DefenseState State { get; private set; } = DefenseState.Inactive;
    public int Gold { get; private set; }
    public int BaseHealth { get; private set; }

    public bool IsActive => State == DefenseState.Preparing || State == DefenseState.WaveInProgress;

    private int quizStreak = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void BeginDefense()
    {
        if (IsActive || State == DefenseState.Won) return;

        Gold = startingGold;
        BaseHealth = maxBaseHealth;
        quizStreak = 0;
        State = DefenseState.Preparing;

        QuestManager.Instance.towerDefenseStarted = true;

        TowerDefenseUI.Instance.ShowHUD(true);
        RefreshHUD();

        waveManager.StartWaves();
    }

    public void SetState(DefenseState newState)
    {
        // Don't let the wave manager overwrite a finished defense
        if (State == DefenseState.Won || State == DefenseState.Lost) return;
        State = newState;
    }

    // ---------- GOLD ----------

    public void AddGold(int amount)
    {
        Gold += amount;
        TowerDefenseUI.Instance.UpdateGold(Gold);
    }

    public bool TrySpend(int cost)
    {
        if (Gold < cost)
        {
            TowerDefenseUI.Instance.ShowMessage("NOT   ENOUGH   GOLD !", 1.5f);
            return false;
        }

        Gold -= cost;
        TowerDefenseUI.Instance.UpdateGold(Gold);
        return true;
    }

    // Called by the Math / Geography teachers after every answer
    public void RegisterQuizAnswer(bool correct)
    {
        if (!IsActive) return;

        if (!correct)
        {
            quizStreak = 0;
            return;
        }

        quizStreak = Mathf.Min(quizStreak + 1, maxQuizStreak);

        int bonus = quizBonusBase + quizBonusPerStreak * (quizStreak - 1);
        AddGold(bonus);

        TowerDefenseUI.Instance.ShowMessage($"QUIZ   BONUS !   +{bonus}   GOLD   (STREAK   x{quizStreak})", 2f);
    }

    // ---------- BASE ----------

    public void DamageBase(int damage)
    {
        if (!IsActive) return;

        BaseHealth = Mathf.Max(0, BaseHealth - damage);
        TowerDefenseUI.Instance.UpdateBaseHealth(BaseHealth, maxBaseHealth);

        if (BaseHealth <= 0)
            HandleLoss();
    }

    // ---------- WAVES ----------

    public void OnWaveCleared(int clearedIndex, int totalWaves, int bonusGold)
    {
        AddGold(bonusGold);

        if (clearedIndex < totalWaves - 1)
        {
            SetState(DefenseState.Preparing);
            TowerDefenseUI.Instance.ShowMessage($"WAVE   {clearedIndex + 1}   CLEARED !   +{bonusGold}   GOLD", 2f);
        }
    }

    public void HandleWin()
    {
        if (!IsActive) return;

        State = DefenseState.Won;
        QuestManager.Instance.towerDefenseCompleted = true;

        StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
        TowerDefenseUI.Instance.ShowMessage("BASE   DEFENDED !", 2f);

        yield return new WaitForSecondsRealtime(1.5f);

        TowerDefenseUI.Instance.ShowHUD(false);
        OnDefenseWon?.Invoke();
    }

    private void HandleLoss()
    {
        if (State == DefenseState.Lost || State == DefenseState.Won) return;

        State = DefenseState.Lost;

        waveManager.StopWaves();
        ClearBoard();

        StartCoroutine(LossRoutine());
    }

    private IEnumerator LossRoutine()
    {
        TowerDefenseUI.Instance.ShowMessage("THE   BASE   HAS   FALLEN !   TALK   TO   THE   TEACHER   TO   TRY   AGAIN", 3f);

        yield return new WaitForSecondsRealtime(3f);

        TowerDefenseUI.Instance.ShowHUD(false);
        State = DefenseState.Inactive;
    }

    private void ClearBoard()
    {
        Enemy.DestroyAll();

        foreach (TowerPlot plot in TowerPlot.AllPlots)
            plot.ClearTower();
    }

    private void RefreshHUD()
    {
        TowerDefenseUI.Instance.UpdateGold(Gold);
        TowerDefenseUI.Instance.UpdateBaseHealth(BaseHealth, maxBaseHealth);
    }
}
