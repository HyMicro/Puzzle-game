using UnityEngine;
using System.Collections.Generic;

public class PathFollowSystem : MonoBehaviour
{
    [Header("Path Settings")]
    public float crystalSpeed = 2f;
    public GameObject crystalPrefab;

    private List<List<Vector2Int>> activePaths;
    private List<Crystal> activeCrystals;

    void Start()
    {
        activePaths = new List<List<Vector2Int>>();
        activeCrystals = new List<Crystal>();
    }

    public void UpdatePaths()
    {
        activePaths.Clear();

        // Find all source points
        var sources = FindSourcePoints();

        foreach (var source in sources)
        {
            List<Vector2Int> path = FindPathFromSource(source);
            if (path.Count > 1) // Valid path found
            {
                activePaths.Add(path);
            }
        }
    }

    List<Vector2Int> FindSourcePoints()
    {
        List<Vector2Int> sources = new List<Vector2Int>();

        for (int x = 0; x < GameManager.Instance.gridWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.gridHeight; y++)
            {
                GridCell cell = GameManager.Instance.GetCell(x, y);
                if (cell != null && cell.pipeType == PipeType.Source)
                {
                    sources.Add(new Vector2Int(x, y));
                }
            }
        }

        return sources;
    }

    List<Vector2Int> FindPathFromSource(Vector2Int source)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = source;
        Vector2Int previous = new Vector2Int(-1, -1);

        path.Add(current);

        while (true)
        {
            Vector2Int next = FindNextCell(current, previous);

            if (next.x == -1) // No valid next cell
                break;

            path.Add(next);

            GridCell nextCell = GameManager.Instance.GetCell(next.x, next.y);
            if (nextCell.pipeType == PipeType.Collector)
                break; // Reached collector

            previous = current;
            current = next;
        }

        return path;
    }

    Vector2Int FindNextCell(Vector2Int current, Vector2Int previous)
    {
        GridCell currentCell = GameManager.Instance.GetCell(current.x, current.y);
        if (currentCell == null) return new Vector2Int(-1, -1);

        // Get possible directions based on pipe type and rotation
        List<Direction> possibleDirs = GetPipeOutputDirections(currentCell.pipeType, currentCell.rotation);

        foreach (Direction dir in possibleDirs)
        {
            Vector2Int neighbor = GetNeighborInDirection(current, dir);

            // Skip if this is where we came from
            if (neighbor == previous) continue;

            // Check if neighbor exists and can be connected to
            GridCell neighborCell = GameManager.Instance.GetCell(neighbor.x, neighbor.y);
            if (neighborCell != null && !neighborCell.IsEmpty())
            {
                Direction oppositeDir = GetOppositeDirection(dir);
                if (CanConnectFromDirection(neighborCell, oppositeDir))
                {
                    return neighbor;
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public void SpawnCrystal(Vector2Int sourcePos)
    {
        // Find path from this source
        List<Vector2Int> path = FindPathFromSource(sourcePos);

        if (path.Count > 1)
        {
            GameObject crystalObj = Instantiate(crystalPrefab);
            Crystal crystal = crystalObj.GetComponent<Crystal>();
            if (crystal == null)
                crystal = crystalObj.AddComponent<Crystal>();

            crystal.InitializePath(path, crystalSpeed);
            activeCrystals.Add(crystal);
        }
    }

    // Helper methods
    List<Direction> GetPipeOutputDirections(PipeType pipeType, int rotation)
    {
        // Implementation depends on pipe type and rotation
        // Return list of directions where this pipe has outputs
        List<Direction> outputs = new List<Direction>();

        switch (pipeType)
        {
            case PipeType.Straight:
                if (rotation % 2 == 0) // Vertical
                {
                    outputs.Add(Direction.North);
                    outputs.Add(Direction.South);
                }
                else // Horizontal
                {
                    outputs.Add(Direction.East);
                    outputs.Add(Direction.West);
                }
                break;
                // ... implement for other pipe types
        }

        return outputs;
    }

    Vector2Int GetNeighborInDirection(Vector2Int pos, Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return new Vector2Int(pos.x, pos.y + 1);
            case Direction.East: return new Vector2Int(pos.x + 1, pos.y);
            case Direction.South: return new Vector2Int(pos.x, pos.y - 1);
            case Direction.West: return new Vector2Int(pos.x - 1, pos.y);
        }
        return pos;
    }

    Direction GetOppositeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.South;
            case Direction.East: return Direction.West;
            case Direction.South: return Direction.North;
            case Direction.West: return Direction.East;
        }
        return dir;
    }

    bool CanConnectFromDirection(GridCell cell, Direction fromDir)
    {
        List<Direction> inputs = GetPipeInputDirections(cell.pipeType, cell.rotation);
        return inputs.Contains(fromDir);
    }

    List<Direction> GetPipeInputDirections(PipeType pipeType, int rotation)
    {
        // Similar to GetPipeOutputDirections but for inputs
        // Most pipes have same inputs and outputs, but some might differ
        return GetPipeOutputDirections(pipeType, rotation);
    }
}