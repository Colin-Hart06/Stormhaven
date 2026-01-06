using UnityEngine;

public class Stats : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static bool hasStarted = false;
    public static int people;//
    public static int peopleToSpawn;//
    public static double peopleWeight;//
    public static int peopleIncome;//

    public static int numWoodBlocks;//
    public static double woodWeight;
    public static double glueStrength;

    public static int numSteelBlocks;
    public static double SteelBlockScale;

    public void Start()
    {
        if(!hasStarted)
        {
            resetStats();
            hasStarted = true;
        }
    }
    void Update()
    {
        //people = PersonCounter.activeCount;
        //Debug.Log(peopleIncome);
    }
    public void resetStats()
    {
        people = 5;
        peopleWeight = 1;
        peopleIncome = 1;
        peopleToSpawn = 0;

        numWoodBlocks = 5;
        woodWeight = 1;
        glueStrength = 1;

        numSteelBlocks = 0;
    }

    public void UpgradePeopleWeight()
    {
        if(GameController.money>=1)
        {
        peopleWeight+=0.5;
        GameController.money--;
        }
    }
    public void UpgradePeopleCount()
    {
        if(GameController.money>=1)
        {
        peopleToSpawn+=1;
        GameController.money--;
        }
    }
    public void UpgradePeopleIncome()
    {
        if(GameController.money>=1)
        {
        peopleIncome+=1;
        GameController.money--;
        }
    }
     public void UpgradeWoodCount()
    {
        if(GameController.money>=1)
        {
        numWoodBlocks+=1;
        GameController.money--;
        }
    }
    public void UpgradeWoodWeight()
    {
        if(GameController.money>=1)
        {
        woodWeight+=1;
        GameController.money--;
        }
    }

    public void UpgradeGlueStrength()
    {
        if(GameController.money>=1)
        {
        glueStrength+=1;
        GameController.money--;
        }
    }
    public void UpgradeNumSteel()
    {
        if(GameController.money>=1)
        {
        numSteelBlocks+=1;
        GameController.money--;
        }
    }
    public void UpgradeSteelSize()
    {
        if(GameController.money>=1)
        {
        SteelBlockScale+=1;
        GameController.money--;
        }
    }

    public void GiveMoney()
    {
        GameController.money+=999;
    }
}
