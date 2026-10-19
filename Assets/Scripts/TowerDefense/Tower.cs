using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Info")]
    public string towerName = "Arrow Tower";
    public TowerLevel[] levels;
    public TargetMode targetMode = TargetMode.First;
    [Range(0f, 1f)] public float sellRefund = 0.6f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform rotatingPart;  // optional, turns toward target
    public float retargetInterval = 0.2f;

    [Header("Visuals")]
    public Sprite[] levelSprites;   // optional, one per level

    public int Level { get; private set; }
    public int InvestedGold { get; private set; }

    public TowerLevel Stats => levels[Level];
    public bool CanUpgrade => Level < levels.Length - 1;
    public int UpgradeCost => CanUpgrade ? levels[Level + 1].upgradeCost : 0;
    public int SellValue => Mathf.FloorToInt(InvestedGold * sellRefund);

    private Enemy currentTarget;
    private float fireTimer = 0f;
    private float retargetTimer = 0f;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int buildCost)
    {
        Level = 0;
        InvestedGold = buildCost;
        ApplyVisuals();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        retargetTimer -= Time.deltaTime;

        // Drop target if it died or walked out of range, then look for a new one
        if (!IsValidTarget(currentTarget) || retargetTimer <= 0f)
        {
            currentTarget = FindTarget();
            retargetTimer = retargetInterval;
        }

        if (currentTarget == null) return;

        AimAt(currentTarget.transform.position);

        if (fireTimer <= 0f)
        {
            Shoot(currentTarget);
            fireTimer = 1f / Stats.fireRate;
        }
    }

    private bool IsValidTarget(Enemy enemy)
    {
        if (enemy == null || enemy.IsDead) return false;

        float rangeSqr = Stats.range * Stats.range;
        return SqrDistanceTo(enemy) <= rangeSqr;
    }

    private float SqrDistanceTo(Enemy enemy)
    {
        return ((Vector2)(enemy.transform.position - transform.position)).sqrMagnitude;
    }

    private Enemy FindTarget()
    {
        Enemy best = null;
        float bestScore = float.MinValue;
        float rangeSqr = Stats.range * Stats.range;

        for (int i = 0; i < Enemy.ActiveEnemies.Count; i++)
        {
            Enemy enemy = Enemy.ActiveEnemies[i];
            if (enemy == null || enemy.IsDead) continue;

            float sqrDist = SqrDistanceTo(enemy);
            if (sqrDist > rangeSqr) continue;

            float score = 0f;

            switch (targetMode)
            {
                case TargetMode.First:
                    score = enemy.DistanceTravelled;
                    break;
                case TargetMode.Closest:
                    score = -sqrDist;
                    break;
                case TargetMode.Strongest:
                    score = enemy.Health;
                    break;
                case TargetMode.Weakest:
                    score = -enemy.Health;
                    break;
            }

            if (score > bestScore)
            {
                bestScore = score;
                best = enemy;
            }
        }

        return best;
    }

    private void AimAt(Vector3 targetPos)
    {
        if (rotatingPart == null) return;

        Vector3 dir = targetPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        rotatingPart.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Shoot(Enemy target)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject obj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Projectile projectile = obj.GetComponent<Projectile>();
        if (projectile != null)
        {
            TowerLevel s = Stats;
            projectile.Launch(target, s.damage, s.splashRadius, s.slowFactor, s.slowDuration);
        }
    }

    public bool Upgrade()
    {
        if (!CanUpgrade) return false;

        int cost = UpgradeCost;
        if (!TowerDefenseManager.Instance.TrySpend(cost)) return false;

        InvestedGold += cost;
        Level++;
        ApplyVisuals();

        return true;
    }

    public void CycleTargetMode()
    {
        int count = System.Enum.GetValues(typeof(TargetMode)).Length;
        targetMode = (TargetMode)(((int)targetMode + 1) % count);
        currentTarget = null;
    }

    private void ApplyVisuals()
    {
        if (spriteRenderer == null) return;
        if (levelSprites == null || Level >= levelSprites.Length) return;

        if (levelSprites[Level] != null)
            spriteRenderer.sprite = levelSprites[Level];
    }

    private void OnDrawGizmosSelected()
    {
        if (levels == null || levels.Length == 0) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, levels[Mathf.Min(Level, levels.Length - 1)].range);
    }
}
