using UnityEngine;

public class Sky : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     private SpriteRenderer spr;
     public Sprite day;
     public Sprite night;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    public GameController gc;
    void Update()
    {
        
        if(gc.IsDay)
        {
            spr.sprite = day;
        }
        else
        {
            spr.sprite = night;
        }
    }
}
