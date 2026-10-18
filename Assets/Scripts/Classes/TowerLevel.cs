using UnityEngine;

[System.Serializable]
public struct TowerLevel
{
    public float damage;
    public float range;
    public float fireRate;      // shots per second

    [Header("Special")]
    public float splashRadius;  // 0 = single target
    public float slowFactor;    // 1 = no slow, 0.5 = half speed
    public float slowDuration;

    [Header("Cost")]
    public int upgradeCost;     // cost to REACH this level (ignored on level 1)
}
