using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Follow Settings")]
    [Tooltip("How quickly the camera catches up to the target Y position.")]
    public float smoothSpeed = 3f;

    [Tooltip("Fixed X and Z offset from the stack centre.")]
    public Vector3 offset = new Vector3(0f, 8f, -14f);

    private float targetY;
    private float startY;

    

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        startY  = transform.position.y;
        targetY = startY;
    }

    void LateUpdate()
    {
        float desiredY = targetY + offset.y;

        float newY = Mathf.Lerp(transform.position.y, desiredY, smoothSpeed * Time.deltaTime);

        
        newY = Mathf.Max(newY, startY);

        transform.position = new Vector3(
            offset.x,
            newY,
            offset.z);

        
        transform.LookAt(new Vector3(0f, targetY, 0f));
    }

    

    
    public void SetTargetY(float newTopY)
    {
        
        if (newTopY > targetY)
            targetY = newTopY;
    }
}
