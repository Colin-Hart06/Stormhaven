using UnityEngine;

public class wind : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float spawnInterval = 3f; // Time between spawns
    [SerializeField] private float lifetimeDuration = 1.2f; // How long before deletion
    
    [Header("Spawn Area")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;
    
    [Header("Options")]
    [SerializeField] private bool startSpawningOnStart = true;
    
    private float spawnTimer = 0f;
    private bool isSpawning = false;
    
    void Start()
    {
        if (startSpawningOnStart)
        {
            StartSpawning();
        }
    }
        public GameController gc;

    void Update()
    {
        if (isSpawning&& gc.IsNight)
        {
            spawnTimer += Time.deltaTime;
            
            if (spawnTimer >= spawnInterval)
            {
                SpawnPrefab();
                spawnTimer = 0f;
            }
        }
    }
    
    public void SpawnPrefab()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned to spawn!");
            return;
        }
        
        // Random position within bounds
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);
        
        // Spawn the prefab
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        
        // Destroy it after lifetime duration
        Destroy(spawnedObject, lifetimeDuration);
    }
    
    public void StartSpawning()
    {
        isSpawning = true;
        spawnTimer = 0f;
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
    }
    
    // Visual debug - shows spawn area
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}