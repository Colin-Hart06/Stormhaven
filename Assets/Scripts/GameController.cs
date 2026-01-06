using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   [SerializeField] public bool IsDay;
   [SerializeField] public bool IsNight;
   public static bool StaticNight;
   public bool NightCleared;
   public static int money;
   public static int day;
   public GameObject people;
   public GameObject wood;
   //private SpriteRenderer spr;
    void Start()
    {
        IsDay = true;
        spawnPeople(Stats.people);
        spawnWood(Stats.numWoodBlocks);
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(day);
        StaticNight = IsNight;
        if (IsDay)
        {
            IsNight=false;
        }else
        {
            IsDay = false;
            IsNight=true;
        }
        if(NightCleared)
        {
            money += PersonCounter.activeCount*Stats.peopleIncome;
            IsDay = true;
            NightCleared=false;
            day++;
            Stats.people = PersonCounter.activeCount;
            SceneManager.LoadScene(0);
        }
    }
    public void spawnPeople(int a)
    {
        a+=Stats.peopleToSpawn;
        for(int i = 0;i<a;i++){
        Instantiate(people);
        }
        Stats.peopleToSpawn = 0;
    }
    public void spawnWood(int a)
    {
        for(int i = 0;i<a;i++){
        Instantiate(wood);
        }
    }
}
