using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

    [Header("Explosion Visual Settings")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Vector2 explosionSpawnOffset = Vector2.zero;

    [Header("Explosion Force Settings")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private Vector2 forceOriginOffset = Vector2.zero;
    [SerializeField] private float explosionForceDelay = 0f;
    [SerializeField] private LayerMask excludedLayers;

    [Header("Animation Durations")]
    [SerializeField] private float windupDuration = 0.5f;
    [SerializeField] private float activeDuration = 0.3f;
    [SerializeField] private float decayDuration = 0.5f;

    private enum LightningState { Idle, Windup, Active, Decay }
    private LightningState currentState = LightningState.Idle;

    private float stateTimer;
    //private bool hasAppliedForce;
    private Coroutine forceCoroutine;
    private GameObject spawnedExplosion;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        TriggerLightning();
    }

    private void Update()
    {
        if (currentState == LightningState.Idle) return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case LightningState.Windup:
                if (stateTimer >= windupDuration)
                    TransitionToActive();
                break;

            case LightningState.Active:
                if (stateTimer >= activeDuration)
                    TransitionToDecay();
                break;

            case LightningState.Decay:
                if (stateTimer >= decayDuration)
                    CompleteLightning();
                break;
        }
    }

    public void TriggerLightning()
    {
        currentState = LightningState.Windup;
        stateTimer = 0f;
        //hasAppliedForce = false;

        animator.Play("Lightning_Windup");
    }

    private void TransitionToActive()
    {
        currentState = LightningState.Active;
        stateTimer = 0f;

        animator.Play("Lightning_Active");
        SpawnExplosionVisual();

        if (forceCoroutine != null)
            StopCoroutine(forceCoroutine);

        forceCoroutine = StartCoroutine(DelayedExplosionForce());
    }

    private void TransitionToDecay()
    {
        currentState = LightningState.Decay;
        stateTimer = 0f;

        animator.Play("Lightning_Decay");
    }

    private void CompleteLightning()
    {
        currentState = LightningState.Idle;

        if (forceCoroutine != null)
            StopCoroutine(forceCoroutine);

        if (spawnedExplosion != null)
            Destroy(spawnedExplosion);

        Destroy(gameObject);
    }

    private IEnumerator DelayedExplosionForce()
    {
        if (explosionForceDelay > 0f)
            yield return new WaitForSeconds(explosionForceDelay);

        ApplyExplosionForce();
        //hasAppliedForce = true;
    }

    private void SpawnExplosionVisual()
    {
        if (explosionPrefab == null) return;

        Vector2 spawnPos = (Vector2)transform.position + explosionSpawnOffset;
        spawnedExplosion = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
    }

    private void ApplyExplosionForce()
    {
        Vector2 forceOrigin = (Vector2)transform.position + forceOriginOffset;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(forceOrigin, explosionRadius);

        foreach (Collider2D col in colliders)
        {
            if (IsInLayerMask(col.gameObject.layer, excludedLayers))
                continue;

            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb == null || rb.bodyType == RigidbodyType2D.Kinematic) continue;

            Vector2 dir = (Vector2)col.transform.position - forceOrigin;
            float distance = dir.magnitude;
            if (distance > explosionRadius) continue;

            float falloff = 1f - (distance / explosionRadius);
            rb.AddForce(dir.normalized * explosionForce * falloff, ForceMode2D.Impulse);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 forceOrigin = (Vector2)transform.position + forceOriginOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(forceOrigin, explosionRadius);

        Vector2 explosionPos = (Vector2)transform.position + explosionSpawnOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(explosionPos, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, forceOrigin);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, explosionPos);
    }
}
