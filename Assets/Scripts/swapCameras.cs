using UnityEngine;

public class swapCameras : MonoBehaviour
{
    public Camera gameCam;
    public Camera shopCam;
    public GameController gc;
    public bool gameCamActive;
    void Start()
    {
        useGameCam();
    }
    void Update()
    {
        if (gc.IsNight)
        {
            useGameCam();
        }
    }

    public void useGameCam()
    {
        gameCamActive = true;
        gameCam.enabled = true;
        shopCam.enabled = false;
    }
    public void useShopCam()
    {
        gameCamActive = false;
        gameCam.enabled = false;
        shopCam.enabled = true;
    }
    public void swapCam()
    {
        if(gameCam.enabled)
        {
            useShopCam();
        }
        else
        useGameCam();
    }
}
