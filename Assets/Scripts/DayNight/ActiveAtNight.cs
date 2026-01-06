using UnityEngine;

public class ActiveAtNight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public GameController gc;
    public GameObject obj;
    void Update()
    {
        if(gc.IsNight)
        {
            obj.SetActive(true);
        }
        if(gc.IsDay)
        obj.SetActive(false);
    }
}
