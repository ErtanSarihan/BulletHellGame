using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies {
  public class EnemySpawner : MonoBehaviour {
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private float spawnDelay = 2f;

    private float _nextSpawnTime;

    private void Start() {
      _nextSpawnTime = Time.time + spawnDelay;
    }

    void Update() {
      if (Time.time >= _nextSpawnTime) {
        SpawnEnemy();
        _nextSpawnTime = Time.time + spawnDelay;
      }
    }

    void SpawnEnemy() {
      // Debug.Log(enemyPrefab + "/" + spawnPoints.Length);
      if (enemyPrefab is null || spawnPoints.Length == 0) {
        Debug.LogError("Missing enemy prefab or spawn points!");
        return;
      }

      Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

      Instantiate(enemyPrefab, randomPoint.position, Quaternion.identity);
    }
  }
}