using UnityEngine;

public class SpaceShooterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private SpaceShooterGameManager gameManager;
    [SerializeField] private float spawnInterval = 1.15f;
    [SerializeField] private float horizontalRange = 7.5f;
    private float nextSpawnTime;

    public void Configure(GameObject prefab, Transform playerTransform, SpaceShooterGameManager manager)
    {
        enemyPrefab = prefab;
        player = playerTransform;
        gameManager = manager;
    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (Time.time < nextSpawnTime || enemyPrefab == null) return;
        Vector3 spawnPosition = new Vector3(Random.Range(-horizontalRange, horizontalRange), 6f, 0f);
        GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        SpaceShooterEnemy enemy = enemyObject.GetComponent<SpaceShooterEnemy>();
        if (enemy != null)
        {
            SpaceShooterEnemy.EnemyType type = (SpaceShooterEnemy.EnemyType)Random.Range(0, 3);
            enemy.Initialize(player, gameManager, type);
        }
        nextSpawnTime = Time.time + spawnInterval;
    }
}