using UnityEngine;

public class drag : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 offset;
    private Rigidbody2D rb;
    
    [SerializeField] private float springFrequency = 10f;
    [SerializeField] private float springDampingRatio = 1f;
    [SerializeField] private float dragDamping = 0.95f;
    
    private GameObject mouseAnchor;
    private Rigidbody2D mouseAnchorRb;
    private SpringJoint2D springJoint;
    private float originalLinearDrag;
    private float originalAngularDrag;
    private string originalTag;
    private bool changedTag = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            originalLinearDrag = rb.linearDamping;
            originalAngularDrag = rb.angularDamping;
        }
    }
    
    void Update()
    {
        // Handle right-click drag start (OnMouseDown only works for left-click)
        if (!dragging && Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                StartDragging(mouseWorldPos, true);
            }
        }
        
        if (dragging && mouseAnchor != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = transform.position.z;
            Vector3 targetPosition = mouseWorldPos + offset;
            
            // Move the invisible anchor to follow the mouse
            mouseAnchor.transform.position = targetPosition;
            
            // Check if right mouse button is held while dragging
            if (Input.GetMouseButton(1))
            {
                RemoveOtherSpringJoints();
            }
        }
        
        // Check for mouse up on either button to stop dragging
        if (dragging && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            StopDragging();
        }
    }
    
    void FixedUpdate()
    {
        if (dragging && rb != null)
        {
            // Heavily dampen all movement while dragging to prevent swinging
            rb.linearVelocity *= dragDamping;
            rb.angularVelocity *= dragDamping;
        }
    }
    
    private void OnMouseDown()
    {
        // OnMouseDown only triggers on left-click, handle left-click here
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartDragging(mouseWorldPos, false);
        }
    }
    
    private void StartDragging(Vector3 mouseWorldPos, bool isRightClick)
    {
        if (dragging) return;
        
        mouseWorldPos.z = transform.position.z;
        offset = transform.position - mouseWorldPos;
        offset.z = 0;
        
        dragging = true;
        
        // If right-click, save and change tag to prevent attachment
        if (isRightClick)
        {
            originalTag = gameObject.tag;
            gameObject.tag = "Untagged";
            changedTag = true;
            RemoveOtherSpringJoints();
        }
        else
        {
            changedTag = false;
        }
        
        if (rb != null)
        {
            // Increase drag significantly while dragging
            rb.linearDamping = 20f;
            rb.angularDamping = 20f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        
        // Create invisible anchor at mouse position
        mouseAnchor = new GameObject("MouseAnchor");
        mouseAnchor.transform.position = mouseWorldPos + offset;
        mouseAnchorRb = mouseAnchor.AddComponent<Rigidbody2D>();
        mouseAnchorRb.bodyType = RigidbodyType2D.Kinematic;
        
        // Create spring joint connecting object to anchor
        springJoint = gameObject.AddComponent<SpringJoint2D>();
        springJoint.connectedBody = mouseAnchorRb;
        springJoint.autoConfigureDistance = false;
        springJoint.distance = 0f;
        springJoint.frequency = springFrequency;
        springJoint.dampingRatio = springDampingRatio;
        springJoint.enableCollision = true;
    }
    
    private void OnMouseUp()
    {
        // OnMouseUp is called for any button, but we handle it in Update now
        // Keep this for compatibility but the actual cleanup happens in StopDragging()
    }
    
    private void StopDragging()
    {
        dragging = false;
        
        // Restore original tag if it was changed
        if (changedTag)
        {
            gameObject.tag = originalTag;
            changedTag = false;
        }
        
        if (rb != null)
        {
            // Restore original drag values
            rb.linearDamping = originalLinearDrag;
            rb.angularDamping = originalAngularDrag;
        }
        
        // Clean up spring joint and anchor
        if (springJoint != null)
        {
            Destroy(springJoint);
        }
        if (mouseAnchor != null)
        {
            Destroy(mouseAnchor);
        }
    }
    
    private void RemoveOtherSpringJoints()
    {
        // Get all SpringJoint2D components on this object
        SpringJoint2D[] allSpringJoints = GetComponents<SpringJoint2D>();
        
        // Destroy all except the one we created
        foreach (SpringJoint2D joint in allSpringJoints)
        {
            if (joint != springJoint && joint != null)
            {
                Destroy(joint);
            }
        }
    }
}