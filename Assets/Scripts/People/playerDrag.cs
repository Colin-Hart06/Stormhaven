using UnityEngine;

public class playerDrag : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 offset;
    private Vector3 lastPosition;
    private Vector2 velocity;
    private Rigidbody2D rb;
    private Collider2D col;
    
    [SerializeField] private float velocityMultiplier = 1f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    
    void Update()
    {
        if(GameController.StaticNight)
        return;
        if (!dragging) return;
        
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        Vector3 newPosition = mouseWorldPos + offset;
        
        velocity = (newPosition - lastPosition) / Time.deltaTime;
        transform.position = newPosition;
        lastPosition = newPosition;
    }
    
    private void OnMouseDown()
    {
        if(GameController.StaticNight)
        return;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        offset = transform.position - mouseWorldPos;
        
        dragging = true;
        lastPosition = transform.position;
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
        
        if (col != null)
        {
            col.enabled = false; // Prevents pushing other rigidbodies
        }
    }
    
    private void OnMouseUp()
    {
        dragging = false;
        
        if (col != null)
        {
            col.enabled = true; // Restore collisions
        }
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // ✅ Changed from Kinematic to Dynamic
            rb.linearVelocity = velocity * velocityMultiplier;
        }
    }
}