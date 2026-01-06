using UnityEngine;

public class ActiveAtDay : MonoBehaviour
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
        if(gc.IsDay)
        {
            obj.SetActive(true);
        }
        else
        obj.SetActive(false);
    }
}
