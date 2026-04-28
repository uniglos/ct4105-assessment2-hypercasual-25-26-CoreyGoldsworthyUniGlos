using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AmountofCheeseManager : MonoBehaviour
{
    public float CheeseTally = 0f;
    public TextMeshProUGUI CheeseCount;


    private void Start()
    {
        CheeseCount.text = CheeseTally.ToString();
    }
    public void tally()
    {
        CheeseTally = CheeseTally + 1f;
        CheeseCount.text = CheeseTally.ToString();
    }
    
}
