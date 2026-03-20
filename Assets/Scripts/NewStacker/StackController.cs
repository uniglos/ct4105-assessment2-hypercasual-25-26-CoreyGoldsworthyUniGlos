using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour
{
    public static StackController Instance { get; private set; }

    [Header("References")]
    [Tooltip("The first static block the stack rests on.")]
    public Transform basePlatform;

    [Header("Trim Settings")]
    [Tooltip("Fraction of overhang that is allowed before triggering game over (0–1). " +
             "0 = any overhang allowed, 1 = must be perfectly aligned.")]
    [Range(0f, 1f)]
    public float maxOverhangFraction = 0.75f;

    [Tooltip("If the overhang is this small (in world units), award a PERFECT bonus.")]
    public float perfectThreshold = 0.05f;

    [Header("Debris")]
    [Tooltip("How many seconds before trimmed debris is destroyed.")]
    public float debrisLifetime = 1.5f;
    public float debrisFallSpeed = 8f;

    
    private List<Transform> stack = new List<Transform>();
    private Vector3 topBlockSize;  

 

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (basePlatform == null)
        {
            Debug.LogError("[StackController] basePlatform is not assigned!");
            return;
        }
        stack.Add(basePlatform);
        topBlockSize = basePlatform.localScale;
    }

    
    public Vector3 GetTopPosition()
    {
        Transform top = stack[stack.Count - 1];
        return top.position + Vector3.up * (top.localScale.y * 0.5f);
    }

  
    public Vector3 GetTopBlockSize() => topBlockSize;

   
    public void OnBlockLanded(Block block)
    {
        Transform top   = stack[stack.Count - 1];
        Transform bTr   = block.transform;

       
        float topLeft    = top.position.x  - top.localScale.x  * 0.5f;
        float topRight   = top.position.x  + top.localScale.x  * 0.5f;
        float blockLeft  = bTr.position.x  - bTr.localScale.x  * 0.5f;
        float blockRight = bTr.position.x  + bTr.localScale.x  * 0.5f;

        float overlapLeft  = Mathf.Max(topLeft,  blockLeft);
        float overlapRight = Mathf.Min(topRight, blockRight);
        float overlapWidth = overlapRight - overlapLeft;

        if (overlapWidth <= 0f)
        {
            
            SpawnDebris(bTr, bTr.localScale);   
            Destroy(bTr.gameObject, debrisLifetime);
            GameManager.Instance.TriggerGameOver();
            return;
        }

        
        float overhang = bTr.localScale.x - overlapWidth;

        if (overhang / bTr.localScale.x > maxOverhangFraction)
        {
            
            SpawnDebris(bTr, bTr.localScale);
            Destroy(bTr.gameObject, debrisLifetime);
            GameManager.Instance.TriggerGameOver();
            return;
        }

       
        bool isPerfect = overhang < perfectThreshold;

        
        if (!isPerfect)
        {
            float trimWidth = bTr.localScale.x - overlapWidth;

            
            bool overhangsRight = bTr.position.x > top.position.x;
            float debrisCenterX = overhangsRight
                ? overlapRight + trimWidth * 0.5f
                : overlapLeft  - trimWidth * 0.5f;

            
            Vector3 debrisSize = new Vector3(trimWidth, bTr.localScale.y, bTr.localScale.z);
            Vector3 debrisPos  = new Vector3(debrisCenterX, bTr.position.y, bTr.position.z);
            SpawnDebrisAt(bTr, debrisPos, debrisSize);

           
            float newCenterX = overlapLeft + overlapWidth * 0.5f;
            bTr.position   = new Vector3(newCenterX, bTr.position.y, bTr.position.z);
            bTr.localScale = new Vector3(overlapWidth, bTr.localScale.y, bTr.localScale.z);
        }

       
        stack.Add(bTr);
        topBlockSize = bTr.localScale;

       
        GameManager.Instance.AddScore(isPerfect ? 2 : 1);

       
        CameraFollow.Instance?.SetTargetY(GetTopPosition().y);

      
        BlockSpawner.Instance.OnBlockLanded();
    }

   
    void SpawnDebris(Transform source, Vector3 size)
        => SpawnDebrisAt(source, source.position, size);

    void SpawnDebrisAt(Transform source, Vector3 worldPos, Vector3 size)
    {
       
        GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debris.transform.position   = worldPos;
        debris.transform.localScale = size;

       
        var srcRend  = source.GetComponent<Renderer>();
        var debRend  = debris.GetComponent<Renderer>();
        if (srcRend != null && debRend != null)
            debRend.material.CopyPropertiesFromMaterial(srcRend.material);

       
        var rb = debris.AddComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(
            Random.Range(-1f, 1f),
            -debrisFallSpeed,
            Random.Range(-1f, 1f));
        rb.angularVelocity = Random.insideUnitSphere * 3f;

        Destroy(debris, debrisLifetime);
    }
}
