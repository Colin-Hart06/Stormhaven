using UnityEngine;
using System.Collections.Generic;

public class Tornado : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float directionChangeInterval = 2f;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    [Header("Pull Settings")]
    [SerializeField] private float pullRadius = 5f;
    [SerializeField] private float pullForce = 10f;

    [Header("Capture Settings")]
    [SerializeField] private float captureRadius = 2f;
    [SerializeField] private float minHoldTime = 2f;
    [SerializeField] private float maxHoldTime = 5f;

    [Header("Spring Settings")]
    [SerializeField] private float springFrequency = 2f;
    [SerializeField] private float springDampingRatio = 0.3f;
    [SerializeField] private float orbitDistance = 1.5f;
    [SerializeField] private float spinTorque = 50f;

    [Header("Eject Settings")]
    [SerializeField] private float ejectForce = 20f;
    [SerializeField] private float ejectAngleRange = 60f;

    [Header("Cooldown Settings")]
    [SerializeField] private float recaptureCooldown = 2f;

    [Header("Visual Debug")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private LayerMask ignorePullLayers;


    private float currentDirection = 1f;
    private float directionTimer;

    private Rigidbody2D tornadoRb;

    private List<CapturedObject> capturedObjects = new();
    private Dictionary<Rigidbody2D, float> recentlyEjectedObjects = new();

    private class CapturedObject
    {
        public Rigidbody2D rb;
        public SpringJoint2D joint;
        public float ejectTime;
        public float originalGravity;
        public float originalAngularDrag;

        public CapturedObject(Rigidbody2D rb, SpringJoint2D joint, float ejectTime)
        {
            this.rb = rb;
            this.joint = joint;
            this.ejectTime = ejectTime;
            originalGravity = rb.gravityScale;
            originalAngularDrag = rb.angularDamping;
        }
    }

    void Start()
    {
        tornadoRb = GetComponent<Rigidbody2D>();
        if (!tornadoRb)
            tornadoRb = gameObject.AddComponent<Rigidbody2D>();

        tornadoRb.bodyType = RigidbodyType2D.Kinematic;
    }

    void FixedUpdate()
    {
        HandleMovement();
        UpdateCooldowns();
        PullNearbyObjects();
        CaptureObjects();
        SpinCapturedObjects();
        EjectObjects();
    }

    void HandleMovement()
    {
        directionTimer += Time.fixedDeltaTime;

        if (directionTimer >= directionChangeInterval)
        {
            directionTimer = 0f;
            currentDirection = Random.value > 0.5f ? 1f : -1f;
        }

        float newX = transform.position.x + currentDirection * moveSpeed * Time.fixedDeltaTime;
        newX = Mathf.Clamp(newX, minX, maxX);

        if (newX == minX || newX == maxX)
            currentDirection *= -1f;

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void UpdateCooldowns()
    {
        List<Rigidbody2D> remove = new();

        foreach (var pair in recentlyEjectedObjects)
        {
            if (pair.Key == null || Time.time >= pair.Value)
                remove.Add(pair.Key);
        }

        foreach (var rb in remove)
            recentlyEjectedObjects.Remove(rb);
    }

    void PullNearbyObjects()
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, pullRadius))
        {
            Rigidbody2D rb = col.attachedRigidbody;
            
            if (!rb || rb.bodyType == RigidbodyType2D.Kinematic || rb == tornadoRb)
                continue;

            if (((1 << rb.gameObject.layer) & ignorePullLayers) != 0)
           continue;

            if (recentlyEjectedObjects.ContainsKey(rb))
                continue;

            if (IsCaptured(rb))
                continue;

            float dist = Vector2.Distance(transform.position, rb.position);
            if (dist <= captureRadius)
                continue;

            Vector2 dir = ((Vector2)transform.position - rb.position).normalized;
            rb.AddForce(dir * pullForce);
        }
    }

    void CaptureObjects()
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, captureRadius))
        {
            Rigidbody2D rb = col.attachedRigidbody;
            if (!rb || rb.bodyType == RigidbodyType2D.Kinematic || rb == tornadoRb)
                continue;
                if (((1 << rb.gameObject.layer) & ignorePullLayers) != 0)
            continue;

            if (recentlyEjectedObjects.ContainsKey(rb))
                continue;

            if (IsCaptured(rb))
                continue;

            SpringJoint2D joint = rb.gameObject.AddComponent<SpringJoint2D>();
            joint.connectedBody = tornadoRb;
            joint.autoConfigureDistance = false;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = Vector2.zero;
            joint.distance = orbitDistance;
            joint.frequency = springFrequency;
            joint.dampingRatio = springDampingRatio;
            joint.enableCollision = true;

            float ejectTime = Time.time + Random.Range(minHoldTime, maxHoldTime);
            capturedObjects.Add(new CapturedObject(rb, joint, ejectTime));

            rb.gravityScale = 0.2f;
            rb.angularDamping = 0f;
        }
    }

    void SpinCapturedObjects()
    {
        foreach (var c in capturedObjects)
        {
            if (!c.rb)
                continue;

            Vector2 toCenter = (Vector2)transform.position - c.rb.position;
            Vector2 tangent = new Vector2(-toCenter.y, toCenter.x).normalized;


            c.rb.AddForce(tangent * spinTorque);
        }
    }

    void EjectObjects()
    {
        for (int i = capturedObjects.Count - 1; i >= 0; i--)
        {
            CapturedObject c = capturedObjects[i];

            if (!c.rb)
            {
                if (c.joint) Destroy(c.joint);
                capturedObjects.RemoveAt(i);
                continue;
            }

            if (Time.time < c.ejectTime)
                continue;

            Rigidbody2D rb = c.rb;

            rb.gravityScale = c.originalGravity;
            rb.angularDamping = c.originalAngularDrag;

            if (c.joint)
            {
                c.joint.enabled = false;
                Destroy(c.joint);
            }

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            recentlyEjectedObjects[rb] = Time.time + recaptureCooldown;

            float angle = Random.Range(90f - ejectAngleRange, 90f + ejectAngleRange);
            Vector2 dir = new(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            rb.AddForce(dir * ejectForce, ForceMode2D.Impulse);

            capturedObjects.RemoveAt(i);
        }
    }

    bool IsCaptured(Rigidbody2D rb)
    {
        foreach (var c in capturedObjects)
            if (c.rb == rb)
                return true;
        return false;
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, captureRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, orbitDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minX, transform.position.y - 2f),
                        new Vector3(minX, transform.position.y + 2f));
        Gizmos.DrawLine(new Vector3(maxX, transform.position.y - 2f),
                        new Vector3(maxX, transform.position.y + 2f));
    }
}
