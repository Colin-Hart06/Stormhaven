using UnityEngine;

public class WeightUpgrade : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D rb;
    public float startMass;
    void Start()
    {
        startMass = rb.mass;
    }

    // Update is called once per frame
    void Update()
    {
        rb.mass = startMass*(float)Stats.peopleWeight;
    }
}
