using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpdateMoneyText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI moneyOnHand;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        moneyOnHand.text = GameController.money.ToString();//string.Format(GameController.money);
    }
}
