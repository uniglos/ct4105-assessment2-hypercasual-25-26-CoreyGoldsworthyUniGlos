using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class KillTrigger : MonoBehaviour
{
    public EndlessMove platform_1;
    public EndlessMove platform_2;
    public EndlessMove platform_3;
    public SwipeFunctions swipeFunctions;
    public GameObject gameOver;

    private void OnTriggerEnter(Collider Player)
    {
        platform_1.enabled = false;
        platform_2.enabled = false;
        platform_3.enabled = false;
        swipeFunctions.enabled = false;
        gameOver.SetActive(true);
    }
}
