using UnityEngine;
using System.Collections.Generic;
public class DestroyObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public float destroyAfterSeconds = 1.3f;
    void Start()
    {
        Invoke("DestorySelf",destroyAfterSeconds);
    }

    // Update is called once per frame
    private void DestorySelf()
    {
        Destroy(gameObject);
    }
}
