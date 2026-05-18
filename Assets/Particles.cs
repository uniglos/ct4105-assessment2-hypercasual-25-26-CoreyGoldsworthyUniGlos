using UnityEngine;

public class Particles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ParticleSystem particle;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("erfger");
        particle.Play();
    }
}

