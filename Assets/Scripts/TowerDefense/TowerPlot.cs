using UnityEngine;
using System.Collections.Generic;

public class TowerPlot : MonoBehaviour, IIteractable
{
    public static List<TowerPlot> AllPlots = new List<TowerPlot>();

    [Header("Setup")]
    public Transform towerAnchor;   // where the tower spawns, defaults to this object

    public Tower CurrentTower { get; private set; }
    public bool HasTower => CurrentTower != null;

    private void OnEnable()
    {
        AllPlots.Add(this);
    }

    private void OnDisable()
    {
        AllPlots.Remove(this);
    }

    // Plots only work while the defense is running
    public bool CanInteract()
    {
        return TowerDefenseManager.Instance != null && TowerDefenseManager.Instance.IsActive;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        TowerBuildUI.Instance.Open(this);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool Build(TowerOption option)
    {
        if (HasTower || option.towerPrefab == null) return false;
        if (!TowerDefenseManager.Instance.TrySpend(option.cost)) return false;

        Transform anchor = towerAnchor != null ? towerAnchor : transform;
        GameObject obj = Instantiate(option.towerPrefab, anchor.position, Quaternion.identity, transform);

        CurrentTower = obj.GetComponent<Tower>();
        CurrentTower.Init(option.cost);

        obj.GetComponent<BounceEffect>()?.StartBounce();

        return true;
    }

    public void Sell()
    {
        if (!HasTower) return;

        TowerDefenseManager.Instance.AddGold(CurrentTower.SellValue);
        ClearTower();
    }

    public void ClearTower()
    {
        if (CurrentTower == null) return;

        Destroy(CurrentTower.gameObject);
        CurrentTower = null;
    }
}
