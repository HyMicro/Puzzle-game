using UnityEngine;
using System.Collections.Generic;

public class PipeManager : MonoBehaviour
{
    public static PipeManager Instance;

    [Header("Pipe Prefabs")]
    public GameObject straightPipe;
    public GameObject cornerPipe;
    public GameObject tJunctionPipe;
    public GameObject crossPipe;
    public GameObject sourcePipe;
    public GameObject collectorPipe;

    private Dictionary<PipeType, GameObject> pipePrefabs;

    void Awake()
    {
        Instance = this;
        InitializePipes();
    }

    void InitializePipes()
    {
        pipePrefabs = new Dictionary<PipeType, GameObject>
        {
            { PipeType.Straight, straightPipe },
            { PipeType.Corner, cornerPipe },
            { PipeType.T_Junction, tJunctionPipe },
            { PipeType.Cross, crossPipe },
            { PipeType.Source, sourcePipe },
            { PipeType.Collector, collectorPipe }
        };
    }

    public GameObject GetPipePrefab(PipeType type)
    {
        pipePrefabs.TryGetValue(type, out GameObject prefab);
        return prefab;
    }

    public bool CanConnect(PipeType pipeA, int rotationA, Direction dirA,
                           PipeType pipeB, int rotationB, Direction dirB)
    {
        return HasOutput(pipeA, rotationA, dirA) && HasInput(pipeB, rotationB, dirB);
    }

    bool HasOutput(PipeType pipe, int rotation, Direction dir)
    {
        List<Direction> outputs = GetPipeOutputDirections(pipe, rotation);
        return outputs.Contains(dir);
    }

    bool HasInput(PipeType pipe, int rotation, Direction dir)
    {
        Direction opposite = GetOppositeDirection(dir);
        return HasOutput(pipe, rotation, opposite);
    }

    List<Direction> GetPipeOutputDirections(PipeType pipe, int rotation)
    {
        List<Direction> outputs = new List<Direction>();

        switch (pipe)
        {
            case PipeType.Straight:
                if (rotation % 2 == 0)
                    outputs.AddRange(new[] { Direction.North, Direction.South });
                else
                    outputs.AddRange(new[] { Direction.East, Direction.West });
                break;

            case PipeType.Corner:
                switch (rotation % 4)
                {
                    case 0: outputs.AddRange(new[] { Direction.North, Direction.East }); break;
                    case 1: outputs.AddRange(new[] { Direction.East, Direction.South }); break;
                    case 2: outputs.AddRange(new[] { Direction.South, Direction.West }); break;
                    case 3: outputs.AddRange(new[] { Direction.West, Direction.North }); break;
                }
                break;

            case PipeType.T_Junction:
                switch (rotation % 4)
                {
                    case 0: outputs.AddRange(new[] { Direction.North, Direction.East, Direction.West }); break;
                    case 1: outputs.AddRange(new[] { Direction.East, Direction.South, Direction.North }); break;
                    case 2: outputs.AddRange(new[] { Direction.South, Direction.East, Direction.West }); break;
                    case 3: outputs.AddRange(new[] { Direction.West, Direction.North, Direction.South }); break;
                }
                break;

            case PipeType.Cross:
                outputs.AddRange(new[] { Direction.North, Direction.South, Direction.East, Direction.West });
                break;

            case PipeType.Source:
                outputs.Add(Direction.North);
                break;

            case PipeType.Collector:
                // collector menerima input saja, tidak ada output
                break;
        }

        return outputs;
    }

    Direction GetOppositeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.South;
            case Direction.South: return Direction.North;
            case Direction.East: return Direction.West;
            case Direction.West: return Direction.East;
        }
        return dir;
    }
}
