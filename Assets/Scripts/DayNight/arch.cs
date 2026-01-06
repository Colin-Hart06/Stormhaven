using UnityEngine;

public class arch : MonoBehaviour
{
    [Header("Arc Points")]
    [SerializeField] private Vector3 startPoint = new Vector3(-5f, 0f, 0f);
    [SerializeField] private Vector3 endPoint = new Vector3(5f, 0f, 0f);
    
    [Header("Arc Settings")]
    [SerializeField] private float arcHeight = 3f;
    [SerializeField] private float travelTime = 2f;
    
    [Header("Light Settings")]
    [SerializeField] private Light globalLight; // For 3D projects
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D globalLight2D; // For 2D projects
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float minIntensity = 0f;
    
    [Header("Options")]
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool destroyOnComplete = false;
    
    private float currentTime = 0f;
    private bool isMoving = false;
    public GameController gc;
    public GameObject moon;
    
    void Start()
    {
        if (startOnAwake)
        {
            if(moon != null)
                moon.SetActive(false);
            StartMovement();
        }
    }
    
    void Update()
    {
        if (isMoving)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / travelTime;
            
            if (t >= 1f)
            {
                // Reached end
                transform.position = endPoint;
                UpdateLightIntensity(1f); // Ensure light is at min intensity
                
                if (loop)
                {
                    currentTime = 0f;
                }
                else
                {
                    isMoving = false;
                    
                    if (destroyOnComplete)
                    {
                        gc.IsDay = false;
                        Destroy(gameObject);
                        if(moon != null)
                            moon.SetActive(true);
                        if(moon == null)
                            gc.NightCleared = true;
                    }
                }
            }
            else
            {
                // Calculate position along arc
                transform.position = CalculateArcPosition(t);
                
                // Update light intensity based on progress
                UpdateLightIntensity(t);
            }
        }
    }
    
    void UpdateLightIntensity(float t)
    {
        // Interpolate from max to min as t goes from 0 to 1
        float intensity = Mathf.Lerp(maxIntensity, minIntensity, t);
        
        // Update whichever light component is assigned
        if (globalLight != null)
        {
            globalLight.intensity = intensity;
        }
        
        if (globalLight2D != null)
        {
            globalLight2D.intensity = intensity;
        }
    }
    
    Vector3 CalculateArcPosition(float t)
    {
        Vector3 linearPosition = Vector3.Lerp(startPoint, endPoint, t);
        float heightOffset = arcHeight * 4f * t * (1f - t);
        linearPosition.y += heightOffset;
        return linearPosition;
    }
    
    public void StartMovement()
    {
        isMoving = true;
        currentTime = 0f;
        transform.position = startPoint;
        UpdateLightIntensity(0f); // Set light to max intensity at start
    }
    
    public void StopMovement()
    {
        isMoving = false;
    }
    
    public void SetPoints(Vector3 start, Vector3 end)
    {
        startPoint = start;
        endPoint = end;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        Gizmos.DrawWireSphere(startPoint, 0.2f);
        Gizmos.DrawWireSphere(endPoint, 0.2f);
        
        Vector3 previousPoint = startPoint;
        int segments = 20;
        
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 currentPoint = CalculateArcPosition(t);
            
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
        
        Gizmos.color = Color.yellow;
        Vector3 peakPoint = CalculateArcPosition(0.5f);
        Gizmos.DrawWireSphere(peakPoint, 0.15f);
    }
}