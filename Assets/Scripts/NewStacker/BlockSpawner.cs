using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance { get; private set; }

    [Header("Block Setup")]
    [Tooltip("Prefab with the Block component attached.")]
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
    [Tooltip("Maximum slide speed cap.")]
    public float maxSlideSpeed = 6f;

    [Header("Block Colours (leave empty for auto hue-cycle)")]
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
        bool tapped = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);

        
        if (!GameManager.Instance.IsGameRunning)
        {
            if (tapped)
            {
                GameManager.Instance.StartGame();
                SpawnNext();            
            }
            return;                     
        }

        
        if (activeBlock == null) return;

        
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

        
        if (tapped)
            DropActiveBlock();
    }

    

    
    public void SpawnNext()
    {
        if (blockPrefab == null)
        {
            Debug.LogError("[BlockSpawner] blockPrefab is not assigned!");
            return;
        }

        Vector3 stackTop  = StackController.Instance.GetTopPosition();
        Vector3 spawnPos  = new Vector3(-slideRange, stackTop.y + spawnHeightOffset, stackTop.z);

        GameObject go = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
        activeBlock = go.GetComponent<Block>();

        if (activeBlock == null)
        {
            Debug.LogError("[BlockSpawner] blockPrefab is missing the Block component!");
            Destroy(go);
            return;
        }

        
        activeBlock.SetSize(StackController.Instance.GetTopBlockSize());

        
        activeBlock.SetColour(PickColour(blockCount));

        
        float dir   = (blockCount % 2 == 0) ? 1f : -1f;
        float speed = slideSpeed + blockCount * speedIncreasePerBlock;
        currentSlideSpeed = dir * Mathf.Min(speed, maxSlideSpeed);

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
