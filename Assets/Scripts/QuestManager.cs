using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [HideInInspector] public bool mathCompleted;
    [HideInInspector] public bool geographyCompleted;

    [HideInInspector] public bool craftingQuestStarted;

    private void Awake()
    {
        Instance = this;
    }

    public bool TeachersCompleted()
    {
        return mathCompleted && geographyCompleted;
    }
}