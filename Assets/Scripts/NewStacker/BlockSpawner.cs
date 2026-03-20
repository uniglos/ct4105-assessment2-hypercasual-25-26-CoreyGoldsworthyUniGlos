using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance { get; private set; }

    [Header("Block Setup")]
    [Tooltip("Prefab that has the Block component attached.")]
    public GameObject blockPrefab;

    [Header("Slide Settings")]
    [Tooltip("How high above the last landed block to spawn new blocks.")]
    public float spawnHeightOffset = 6f;
    [Tooltip("Half-width of the slide range on each axis.")]
    public float slideRange = 3f;
    [Tooltip("Starting slide speed (units per second).")]
    public float slideSpeed = 2f;
    [Tooltip("Speed increase per successful placement.")]
    public float speedIncreasePerBlock = 0.1f;
    [Tooltip("Maximum slide speed.")]
    public float maxSlideSpeed = 6f;

    [Header("Block Colours (optional – leave empty for random)")]
    public Color[] palette;

    
    private Block activeBlock;          
    private float currentSlideSpeed;
    private int blockCount;

   

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        currentSlideSpeed = slideSpeed;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameRunning || activeBlock == null) return;

       
        float newX = activeBlock.transform.position.x + currentSlideSpeed * Time.deltaTime;

        
        if (Mathf.Abs(newX) >= slideRange)
        {
            currentSlideSpeed = -currentSlideSpeed;
            newX = Mathf.Clamp(newX, -slideRange, slideRange);
        }

        activeBlock.transform.position = new Vector3(
            newX,
            activeBlock.transform.position.y,
            activeBlock.transform.position.z);

        
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            DropActiveBlock();
    }

    
    public void SpawnNext()
    {
        if (blockPrefab == null) { Debug.LogError("[BlockSpawner] blockPrefab is not assigned!"); return; }

        Vector3 stackTop = StackController.Instance.GetTopPosition();
        Vector3 spawnPos = new Vector3(
            -slideRange,                            // start from one edge
            stackTop.y + spawnHeightOffset,
            stackTop.z);

        GameObject go = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
        activeBlock = go.GetComponent<Block>();

        if (activeBlock == null)
        {
            Debug.LogError("[BlockSpawner] blockPrefab is missing the Block component!");
            Destroy(go);
            return;
        }

       
        Vector3 lastSize = StackController.Instance.GetTopBlockSize();
        activeBlock.SetSize(lastSize);

        
        activeBlock.SetColour(PickColour(blockCount));

        currentSlideSpeed = Mathf.Min(
            (blockCount % 2 == 0 ? 1f : -1f) *   
            (slideSpeed + blockCount * speedIncreasePerBlock),
            maxSlideSpeed);

        blockCount++;
    }

    
    public void OnBlockLanded()
    {
        activeBlock = null;
        SpawnNext();
    }

    

    void DropActiveBlock()
    {
        if (activeBlock == null) return;
        activeBlock.Drop();
        activeBlock = null;        
    }

    Color PickColour(int index)
    {
        if (palette != null && palette.Length > 0)
            return palette[index % palette.Length];

       
        return Color.HSVToRGB((index * 0.13f) % 1f, 0.7f, 0.95f);
    }
}
