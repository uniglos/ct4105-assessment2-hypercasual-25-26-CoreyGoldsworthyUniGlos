using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class maxShotManager : MonoBehaviour
{

    public float shotCount = 5f;
    public GameObject shooter;


    public void tally()
    {
        shotCount = shotCount - 1f;
    }
    private void Update()
    {
        if(shotCount<= 0)
        {
            shooter.SetActive(false);
        }
    }
}
