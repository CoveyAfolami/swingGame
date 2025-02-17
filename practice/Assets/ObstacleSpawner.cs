using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // List of different obstacle types (e.g., cactus, bird, rock)
    public int poolSize = 7; // Number of obstacles to keep in the pool
    public float minSpawnX = 10f; // X position to spawn obstacles (off-screen)
    public float maxSpawnY = -2f; // Y position (adjust for ground height)
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;

    private List<GameObject> obstaclePool; // List to store pooled obstacles
    private float spawnTimer;

    void Start()
    {
        // Initialize the object pool
        obstaclePool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]);
            obj.SetActive(false); // Disable them initially
            obstaclePool.Add(obj);
        }

        // Set initial spawn time
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime; // Decrease timer each frame

        if (spawnTimer <= 0f)
        {
            SpawnObstacle(); // Spawn an obstacle when timer reaches 0
            spawnTimer = Random.Range(minSpawnTime, maxSpawnTime); // Reset timer
        }
    }

    void SpawnObstacle()
    {
        GameObject obstacle = GetPooledObject(); // Get an inactive obstacle from the pool

        if (obstacle != null)
        {
            float spawnX = transform.position.x + minSpawnX; // Spawn off-screen
            obstacle.transform.position = new Vector2(spawnX, maxSpawnY);
            obstacle.SetActive(true); // Activate the obstacle
        }
    }

    GameObject GetPooledObject()
    {
        // Find the first inactive object in the pool and return it
        foreach (GameObject obj in obstaclePool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null; // If all objects are active, return null (shouldn't happen often)
    }
}
