using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;   // first = spawn, last = base

    [Header("Manual Waves (optional)")]
    public List<Wave> waves = new List<Wave>();

    [Header("Auto Generated Waves")]
    public bool autoGenerateWaves = true;
    public GameObject[] enemyPrefabs;   // weakest -> strongest
    public int generatedWaveCount = 8;
    public int baseEnemyCount = 5;

    [Header("Timing")]
    public float firstWaveDelay = 20f;
    public float timeBetweenWaves = 12f;

    [Header("Scaling")]
    public float healthScalingPerWave = 0.15f;

    public int CurrentWaveIndex { get; private set; }

    private bool skipRequested = false;
    private Coroutine waveRoutine;

    public void StartWaves()
    {
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning("WaveManager needs at least 2 waypoints!");
            return;
        }

        if (autoGenerateWaves || waves.Count == 0)
            GenerateWaves();

        StopWaves();
        waveRoutine = StartCoroutine(WaveRoutine());
    }

    public void StopWaves()
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = null;
    }

    public void SkipCountdown()
    {
        skipRequested = true;
    }

    private void GenerateWaves()
    {
        waves = new List<Wave>();

        for (int w = 0; w < generatedWaveCount; w++)
        {
            Wave wave = new Wave
            {
                waveName = $"WAVE   {w + 1}",
                clearBonusGold = 20 + w * 5
            };

            for (int t = 0; t < enemyPrefabs.Length; t++)
            {
                // Tougher enemy types only show up in later waves
                if (w < t * 2) continue;

                int count = Mathf.Max(2, baseEnemyCount + w - t * 2);

                wave.groups.Add(new EnemyGroup
                {
                    enemyPrefab = enemyPrefabs[t],
                    count = count,
                    spawnInterval = Mathf.Max(0.3f, 1.2f - w * 0.08f),
                    delayAfterGroup = 2f
                });
            }

            waves.Add(wave);
        }
    }

    private IEnumerator WaveRoutine()
    {
        TowerDefenseManager manager = TowerDefenseManager.Instance;

        for (int w = 0; w < waves.Count; w++)
        {
            CurrentWaveIndex = w;
            Wave wave = waves[w];

            // Prep time, player builds towers here
            yield return StartCoroutine(Countdown(w == 0 ? firstWaveDelay : timeBetweenWaves, w + 1));

            manager.SetState(DefenseState.WaveInProgress);
            TowerDefenseUI.Instance.UpdateWave(w + 1, waves.Count);

            float healthMultiplier = 1f + w * healthScalingPerWave;

            for (int g = 0; g < wave.groups.Count; g++)
            {
                EnemyGroup group = wave.groups[g];

                for (int i = 0; i < group.count; i++)
                {
                    SpawnEnemy(group.enemyPrefab, healthMultiplier);
                    yield return new WaitForSeconds(group.spawnInterval);
                }

                yield return new WaitForSeconds(group.delayAfterGroup);
            }

            // Wait for the player to kill (or leak) everything
            yield return new WaitUntil(() => Enemy.ActiveEnemies.Count == 0);

            manager.OnWaveCleared(w, waves.Count, wave.clearBonusGold);
        }

        manager.HandleWin();
    }

    private IEnumerator Countdown(float seconds, int nextWaveNumber)
    {
        skipRequested = false;
        float remaining = seconds;

        while (remaining > 0f && !skipRequested)
        {
            TowerDefenseUI.Instance.UpdateCountdown(Mathf.CeilToInt(remaining), nextWaveNumber);
            remaining -= Time.deltaTime;
            yield return null;
        }

        // Reward calling the wave early
        if (skipRequested)
            TowerDefenseManager.Instance.AddGold(Mathf.CeilToInt(remaining));

        TowerDefenseUI.Instance.HideCountdown();
    }

    private void SpawnEnemy(GameObject prefab, float healthMultiplier)
    {
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, waypoints[0].position, Quaternion.identity);
        Enemy enemy = obj.GetComponent<Enemy>();

        if (enemy != null)
            enemy.Initialize(waypoints, healthMultiplier);
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            Gizmos.DrawWireSphere(waypoints[i].position, 0.15f);

            if (i + 1 < waypoints.Length && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
