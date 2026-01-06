using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas canv;
    void Start()
    {
        if(canv!=null)
        canv.enabled = true;
    }

    // Update is called once per frame
    public swapCameras sc;
    public GameController gc;
    void Update()
    {
        if(!sc.gameCamActive||gc.IsNight)
        {
           canv.enabled = false;
        }
        else
        {
            canv.enabled = true;
        }
    }
}
