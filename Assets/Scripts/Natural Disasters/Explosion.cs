using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject explosivePrefab;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float spawnHeight = 20f;
    [SerializeField] private KeyCode spawnKey = KeyCode.E;
    
    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnExplosive();
        }
    }
    
    void SpawnExplosive()
    {
        // Random X position between min and max
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0f);
        
        // Spawn the prefab
        Instantiate(explosivePrefab, spawnPosition, Quaternion.identity);
    }
}