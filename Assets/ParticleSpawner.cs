using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public ParticleSystem particle;
    Vector3 spawnPoint;
    public GameObject target;

    private void Start()
    {
        spawnPoint = new Vector3(target.transform.position.x, target.transform.position.y);
    }
    public void SpawnParticle()
    {
        Instantiate(particle, spawnPoint, transform.rotation);
    }
}
