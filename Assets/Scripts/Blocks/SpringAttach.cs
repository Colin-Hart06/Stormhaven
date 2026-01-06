using UnityEngine;
using System.Collections.Generic;

public class SpringAttach : MonoBehaviour
{
    [Header("Attachment Settings")]
    [SerializeField] private string attachableTag = "Attachable";
    [SerializeField] private float attachRadius = 2f;
    [SerializeField] private float detachRadius = 3f;
    [SerializeField] private int maxConnections = 4; // Maximum number of connections per object
    
    [Header("Spring Settings")]
    [SerializeField] private float springFrequency = 1f;
    [SerializeField] private float springDampingRatio = 0.5f;
    [SerializeField] private float springDistance = 0.5f;
    [SerializeField] private bool enableCollision = true;
    
    [Header("Gravity Settings")]
    [SerializeField] private float attachedGravityMultiplier = 0.5f;
    
    [Header("Visual Debug")]
    [SerializeField] private bool showGizmos = true;
    
    private List<SpringJoint2D> springJoints = new List<SpringJoint2D>();
    private List<GameObject> connectedObjects = new List<GameObject>();
    private Rigidbody2D rb;
    private float originalGravityScale;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }
    }
    
    void Update()
    {
        // Try to attach to nearby objects
        TryAttach();
        
        // Check if any connections should be broken
        CheckDetach();
        
        // Update gravity based on connection state
        UpdateGravity();
    }
    
    void TryAttach()
    {
        // Don't attach if at max connections
        if (springJoints.Count >= maxConnections)
            return;
        
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, attachRadius);
        
        foreach (Collider2D col in nearbyColliders)
        {
            if (springJoints.Count >= maxConnections)
                break;
            
            // Skip if it's this object
            if (col.gameObject == gameObject)
                continue;
            
            // Skip if not attachable
            if (!col.CompareTag(attachableTag))
                continue;
            
            // Skip if already connected to this object
            if (connectedObjects.Contains(col.gameObject))
                continue;
            
            Rigidbody2D targetRb = col.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                // Check if target would exceed their max connections
                SpringAttach targetSpring = col.GetComponent<SpringAttach>();
                if (targetSpring != null && targetSpring.GetConnectionCount() >= targetSpring.maxConnections)
                    continue;
                
                // Create spring joint
                SpringJoint2D newJoint = gameObject.AddComponent<SpringJoint2D>();
                newJoint.connectedBody = targetRb;
                newJoint.autoConfigureDistance = false;
                newJoint.distance = springDistance;
                newJoint.frequency = springFrequency;
                newJoint.dampingRatio = springDampingRatio;
                newJoint.enableCollision = enableCollision;
                
                springJoints.Add(newJoint);
                connectedObjects.Add(col.gameObject);
            }
        }
    }
    
    void CheckDetach()
    {
        for (int i = springJoints.Count - 1; i >= 0; i--)
        {
            if (springJoints[i] == null || connectedObjects[i] == null)
            {
                // Object was destroyed
                if (springJoints[i] != null)
                    Destroy(springJoints[i]);
                springJoints.RemoveAt(i);
                connectedObjects.RemoveAt(i);
            }
            else
            {
                // Check distance
                float distance = Vector2.Distance(transform.position, connectedObjects[i].transform.position);
                if (distance > detachRadius)
                {
                    Destroy(springJoints[i]);
                    springJoints.RemoveAt(i);
                    connectedObjects.RemoveAt(i);
                }
            }
        }
    }
    
    void UpdateGravity()
    {
        if (rb != null)
        {
            if (springJoints.Count > 0)
            {
                // Has connections - reduce gravity
                rb.gravityScale = originalGravityScale * attachedGravityMultiplier;
            }
            else
            {
                // No connections - restore original gravity
                rb.gravityScale = originalGravityScale;
            }
        }
    }
    
    public int GetConnectionCount()
    {
        return springJoints.Count;
    }
    
    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = springJoints.Count == 0 ? Color.yellow : Color.green;
            Gizmos.DrawWireSphere(transform.position, attachRadius);
            
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, detachRadius);
            
            if (springJoints.Count > 0)
            {
                Gizmos.color = Color.cyan;
                foreach (GameObject connected in connectedObjects)
                {
                    if (connected != null)
                    {
                        Gizmos.DrawLine(transform.position, connected.transform.position);
                    }
                }
            }
        }
    }
}