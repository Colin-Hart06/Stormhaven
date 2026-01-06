using UnityEngine;

public class PersonCounter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static int activeCount = 0;
    void OnEnable()
    {
        activeCount++;
        //Debug.Log($"Person spawned. Total: {activeCount}");
    }
    void OnDisable()
    {
        activeCount--;
        //Debug.Log($"Lightning destroyed. Total: {activeCount}");
    }
}
