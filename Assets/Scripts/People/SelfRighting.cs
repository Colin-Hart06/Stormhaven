using UnityEngine;

public class SelfRighting : MonoBehaviour
{
    [Header("Self-Righting Settings")]
    [SerializeField] private float uprightTorque = 50f;
    [SerializeField] private float uprightDamping = 5f;
    [SerializeField] private float activationAngle = 20f;
    
    [Header("Target Rotation")]
    [SerializeField] private float targetRotation = 0f;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, 0); // Manual adjustment if needed
    
    [Header("Debug")]
    [SerializeField] private bool showGroundCheck = true;
    
    private Rigidbody2D rb;
    private Collider2D col;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    
    void FixedUpdate()
    {
        if (rb == null) return;
        
        // Only self-right if grounded
        if (!IsGrounded())
            return;
        
        // Get current rotation (normalized to -180 to 180)
        float currentRotation = NormalizeAngle(transform.eulerAngles.z);
        float targetRot = NormalizeAngle(targetRotation);
        
        // Calculate angle difference
        float angleDifference = Mathf.DeltaAngle(currentRotation, targetRot);
        
        // Only apply torque if tilted beyond activation angle
        if (Mathf.Abs(angleDifference) > activationAngle)
        {
            // Calculate torque needed (proportional to angle difference)
            float torque = angleDifference * uprightTorque;
            
            // Apply damping to prevent oscillation
            float dampingTorque = -rb.angularVelocity * uprightDamping;
            
            // Apply combined torque
            rb.AddTorque((torque + dampingTorque) * Time.fixedDeltaTime);
        }
    }
    
    bool IsGrounded()
    {
        if (col == null)
            return false;
        
        // Use collider bounds for accurate bottom position
        Bounds bounds = col.bounds;
        Vector2 bottomPoint = new Vector2(bounds.center.x, bounds.min.y);
        bottomPoint += groundCheckOffset;
        
        // Check if there's ground below
        RaycastHit2D hit = Physics2D.Raycast(
            bottomPoint, 
            Vector2.down, 
            groundCheckDistance, 
            groundLayer
        );
        
        return hit.collider != null;
    }
    
    // Normalize angle to -180 to 180 range
    float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180)
            angle -= 360;
        return angle;
    }
    
    // Visual debug
    void OnDrawGizmos()
    {
        // Draw upright direction
        Gizmos.color = Color.green;
        Vector3 uprightDirection = Quaternion.Euler(0, 0, targetRotation) * Vector3.up;
        Gizmos.DrawLine(transform.position, transform.position + uprightDirection * 2f);
        
        // Draw ground check
        if (showGroundCheck && col != null)
        {
            Bounds bounds = col.bounds;
            Vector2 bottomPoint = new Vector2(bounds.center.x, bounds.min.y);
            bottomPoint += groundCheckOffset;
            
            // Draw ground check ray
            Gizmos.color = IsGrounded() ? Color.green : Color.red;
            Gizmos.DrawSphere(bottomPoint, 0.1f);
            Gizmos.DrawLine(bottomPoint, bottomPoint + Vector2.down * groundCheckDistance);
        }
    }
}