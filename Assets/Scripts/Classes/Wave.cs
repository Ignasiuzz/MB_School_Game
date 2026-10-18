using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public string waveName = "WAVE";
    public List<EnemyGroup> groups = new List<EnemyGroup>();
    public int clearBonusGold = 25;
}
