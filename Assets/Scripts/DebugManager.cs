using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool showDebugInfo = true;
    public bool showGridLines = true;
    public KeyCode debugToggleKey = KeyCode.F1;

    private bool debugMode = false;

    void Update()
    {
        if (Input.GetKeyDown(debugToggleKey))
        {
            debugMode = !debugMode;
        }

        if (debugMode)
        {
            HandleDebugInput();
        }
    }

    void HandleDebugInput()
    {
        // Debug shortcuts
        for (int x = 0; x < GameManager.Instance.gridWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.gridHeight; y++)
            {
                GridCell cell = GameManager.Instance.GetCell(x, y);
                if (cell != null && cell.pipeType == PipeType.Source)
                {
                    GameManager.Instance.pathSystem.SpawnCrystal(new Vector2Int(cell.x, cell.y));
                    break;
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.F3))
        {
            // Clear grid
            ClearGrid();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            // Add score
            FindObjectOfType<UIManager>()?.UpdateScore(100);
        }
    }

    void ClearGrid()
    {
        for (int x = 0; x < GameManager.Instance.gridWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.gridHeight; y++)
            {
                GridCell cell = GameManager.Instance.GetCell(x, y);
                if (cell != null && !cell.IsEmpty())
                {
                    cell.RemovePipe();
                }
            }
        }
    }

    void OnGUI()
    {
        if (!debugMode || !showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("DEBUG MODE");
        GUILayout.Label("F2: Spawn Crystal");
        GUILayout.Label("F3: Clear Grid");
        GUILayout.Label("F4: Add Score");

        if (GameManager.Instance != null)
        {
            GUILayout.Label($"Grid: {GameManager.Instance.gridWidth}x{GameManager.Instance.gridHeight}");
        }

        GUILayout.EndArea();
    }

    void OnDrawGizmos()
    {
        if (!showGridLines || GameManager.Instance == null) return;

        // Draw grid lines
        Gizmos.color = Color.gray;
        float cellSize = GameManager.Instance.cellSize;

        for (int x = 0; x <= GameManager.Instance.gridWidth; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, GameManager.Instance.gridHeight * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= GameManager.Instance.gridHeight; y++)
        {
            Vector3 start = new Vector3(0, y * cellSize, 0);
            Vector3 end = new Vector3(GameManager.Instance.gridWidth * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}