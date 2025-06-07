using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int gridWidth = 8;
    public int gridHeight = 12;
    public float cellSize = 1f;

    [Header("References")]
    public Transform gridParent;
    public PipelineTetris tetrisSystem;
    public MiningSystem miningSystem;
    public PathFollowSystem pathSystem;

    private GridCell[,] gameGrid;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGame();
        }
        else Destroy(gameObject);
    }

    void InitializeGame()
    {
        CreateGrid();
        SetupSources();
        SetupCollectors();
    }

    void CreateGrid()
    {
        Debug.Log($"Creating grid of size {gridWidth}x{gridHeight}");

        gameGrid = new GridCell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSize, y * cellSize, 0);
                gameGrid[x, y] = new GridCell(x, y, worldPos);
            }
        }
    }


    public GridCell GetCell(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            return gameGrid[x, y];
        return null;
    }

    public bool PlacePipe(PipeType pipeType, int x, int y, int rotation = 0)
    {
        GridCell cell = GetCell(x, y);

        if (cell == null)
        {
            Debug.LogError($"[PlacePipe] Invalid cell at ({x},{y})");
            return false;
        }

        if (cell.IsEmpty())
        {
            cell.PlacePipe(pipeType, rotation);
            pathSystem?.UpdatePaths();
            return true;
        }

        return false;
    }


    void SetupSources()
    {
        PlacePipe(PipeType.Source, 1, 1);
        PlacePipe(PipeType.Source, 5, 11);
    }

    void SetupCollectors()
    {
        PlacePipe(PipeType.Collector, 3, 10);
        PlacePipe(PipeType.Collector, 5, 10);
    }

    public void CollectCrystal(Crystal crystal)
    {
        Debug.Log("Crystal collected!");
        FindObjectOfType<UIManager>()?.UpdateScore(10);
        Destroy(crystal.gameObject);
    }

    void OptimizeForMobile()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        System.GC.Collect();
    }
}
