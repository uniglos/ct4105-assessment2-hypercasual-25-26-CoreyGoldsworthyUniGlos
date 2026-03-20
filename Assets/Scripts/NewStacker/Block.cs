using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Block : MonoBehaviour
{
    [Header("Fall Settings")]
    [Tooltip("Speed at which the block falls after being dropped (units/sec).")]
    public float fallSpeed = 12f;

    [Header("Settle Detection")]
    [Tooltip("How close to the target Y before we consider the block 'landed'.")]
    public float landThreshold = 0.05f;

    
    private Rigidbody rb;
    private bool isDropped;
    private bool hasLanded;
    private float targetY;          

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity  = false;
        rb.isKinematic = true;
    }

    void Update()
    {
        if (!isDropped || hasLanded) return;

        
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        
        if (transform.position.y <= targetY)
        {
            
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
            Land();
        }
    }

    
    public void Drop()
    {
        if (isDropped) return;

        // targetY = top of stack + half of this block's height
        float stackTopY       = StackController.Instance.GetTopPosition().y;
        float blockHalfHeight = transform.localScale.y * 0.5f;
        targetY = stackTopY + blockHalfHeight;

        isDropped = true;
    }

   
    public void SetSize(Vector3 size)
    {
        transform.localScale = size;
    }

   
    public void SetColour(Color colour)
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            
            var mat = rend.material;
            if (mat.HasProperty("_Color"))       mat.color         = colour;
            if (mat.HasProperty("_BaseColor"))   mat.SetColor("_BaseColor", colour);
        }
    }

    

    void Land()
    {
        hasLanded = true;
       
        StackController.Instance.OnBlockLanded(this);
    }
}
