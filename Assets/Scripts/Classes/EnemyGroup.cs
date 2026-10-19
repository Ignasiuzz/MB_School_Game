using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count = 5;
    public float spawnInterval = 1f;
    public float delayAfterGroup = 2f;
}
