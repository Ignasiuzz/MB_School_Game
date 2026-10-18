using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [HideInInspector] public bool mathCompleted;
    [HideInInspector] public bool geographyCompleted;

    [HideInInspector] public bool craftingQuestStarted;

    [HideInInspector] public bool towerDefenseStarted;
    [HideInInspector] public bool towerDefenseCompleted;

    private void Awake()
    {
        Instance = this;
    }

    public bool TeachersCompleted()
    {
        return mathCompleted && geographyCompleted;
    }
}