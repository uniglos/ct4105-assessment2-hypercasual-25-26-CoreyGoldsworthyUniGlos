using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class maxShotManager : MonoBehaviour
{

    public float shotCount = 5f;
    public GameObject shooter;
    public TextMeshProUGUI shotsLeft;


    private void Start()
    {
        shotsLeft.text = shotCount.ToString();
    }
    public void tally()
    {
        shotCount = shotCount - 1f;
        shotsLeft.text = shotCount.ToString();
    }
    private void Update()
    {
        if(shotCount<= 0)
        {
            shooter.SetActive(false);
        }
    }
}
