using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public static List<Enemy> ActiveEnemies = new List<Enemy>();

    [Header("Stats")]
    public float maxHealth = 30f;
    public float moveSpeed = 1.5f;
    public float armor = 0f;        // flat damage reduction per hit
    public int goldReward = 10;
    public int baseDamage = 1;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Transform healthBarFill; // optional, scaled on X
    public Color hitColor = Color.red;
    public Color slowColor = new Color(0.5f, 0.8f, 1f);

    public float Health { get; private set; }
    public float DistanceTravelled { get; private set; }
    public bool IsDead { get; private set; }

    private float currentMaxHealth;
    private Transform[] waypoints;
    private int waypointIndex = 1;
    private float speedMultiplier = 1f;

    private Color baseColor = Color.white;
    private Vector3 healthBarFullScale = Vector3.one;
    private Coroutine slowRoutine;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            baseColor = spriteRenderer.color;

        if (healthBarFill != null)
            healthBarFullScale = healthBarFill.localScale;
    }

    private void OnEnable()
    {
        ActiveEnemies.Add(this);
    }

    private void OnDisable()
    {
        ActiveEnemies.Remove(this);
    }

    public void Initialize(Transform[] path, float healthMultiplier)
    {
        waypoints = path;
        waypointIndex = 1; // we spawn on waypoint 0

        currentMaxHealth = maxHealth * healthMultiplier;
        Health = currentMaxHealth;

        UpdateHealthBar();
    }

    void Update()
    {
        if (IsDead || waypoints == null) return;

        float step = moveSpeed * speedMultiplier * Time.deltaTime;

        // Loop so fast enemies don't overshoot corners
        while (step > 0f && waypointIndex < waypoints.Length)
        {
            Vector3 toTarget = waypoints[waypointIndex].position - transform.position;
            float dist = toTarget.magnitude;

            if (spriteRenderer != null && Mathf.Abs(toTarget.x) > 0.01f)
                spriteRenderer.flipX = toTarget.x < 0f;

            if (dist <= step)
            {
                transform.position = waypoints[waypointIndex].position;
                DistanceTravelled += dist;
                step -= dist;
                waypointIndex++;
            }
            else
            {
                transform.position += toTarget / dist * step;
                DistanceTravelled += step;
                step = 0f;
            }
        }

        if (waypointIndex >= waypoints.Length)
            ReachBase();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        float finalDamage = Mathf.Max(1f, amount - armor);
        Health -= finalDamage;

        UpdateHealthBar();
        StartCoroutine(HitFlash());

        if (Health <= 0f)
            Die();
    }

    public void ApplySlow(float factor, float duration)
    {
        if (IsDead) return;

        // Keep the stronger slow if one is already running
        if (slowRoutine != null && factor > speedMultiplier) return;

        if (slowRoutine != null)
            StopCoroutine(slowRoutine);

        slowRoutine = StartCoroutine(SlowRoutine(factor, duration));
    }

    private IEnumerator SlowRoutine(float factor, float duration)
    {
        speedMultiplier = factor;
        if (spriteRenderer != null) spriteRenderer.color = slowColor;

        yield return new WaitForSeconds(duration);

        speedMultiplier = 1f;
        if (spriteRenderer != null) spriteRenderer.color = baseColor;

        slowRoutine = null;
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.08f);

        if (!IsDead)
            spriteRenderer.color = speedMultiplier < 1f ? slowColor : baseColor;
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        float percent = Mathf.Clamp01(Health / currentMaxHealth);
        healthBarFill.localScale = new Vector3(healthBarFullScale.x * percent, healthBarFullScale.y, healthBarFullScale.z);
    }

    private void Die()
    {
        IsDead = true;
        ActiveEnemies.Remove(this);

        TowerDefenseManager.Instance.AddGold(goldReward);

        Destroy(gameObject);
    }

    private void ReachBase()
    {
        IsDead = true;
        ActiveEnemies.Remove(this);

        TowerDefenseManager.Instance.DamageBase(baseDamage);

        Destroy(gameObject);
    }

    // Wipes the field with no rewards (used when the base falls)
    public static void DestroyAll()
    {
        for (int i = ActiveEnemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = ActiveEnemies[i];
            if (enemy == null) continue;

            enemy.IsDead = true;
            Destroy(enemy.gameObject);
        }

        ActiveEnemies.Clear();
    }
}
