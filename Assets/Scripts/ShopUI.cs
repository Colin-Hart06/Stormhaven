using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas canv;
    void Start()
    {
        if(canv!=null)
        canv.enabled = false;
    }

    // Update is called once per frame
    public swapCameras sc;
    void Update()
    {
        if(!sc.gameCamActive)
        {
           canv.enabled = true;
        }
        else
        {
            canv.enabled = false;
        }
    }
}
