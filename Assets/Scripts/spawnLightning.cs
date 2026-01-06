using UnityEngine;

public class spawnLightning : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnDelay;
    public float destroyDelay = 3.5f;
    public GameObject Lightning;
    public Vector2 spawnOffset = Vector2.zero;
    
    void Start()
    {
        Invoke("SummonLightning", spawnDelay);
    }
    
    public void SummonLightning()
    {
        // Calculate spawn position with offset
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset.x, spawnOffset.y, 0);
        
        // Instantiate lightning at offset position
        Instantiate(Lightning, spawnPosition, Quaternion.identity);
        
        Invoke("DestroySelf", destroyDelay);
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}