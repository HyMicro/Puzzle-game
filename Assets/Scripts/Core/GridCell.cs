using UnityEngine;

[System.Serializable]
public class GridCell
{
    public int x, y;
    public Vector3 worldPosition;
    public PipeType pipeType;
    public int rotation;
    public bool isEmpty;
    public GameObject pipeObject;

    public GridCell(int x, int y, Vector3 worldPos)
    {
        this.x = x;
        this.y = y;
        this.worldPosition = worldPos;
        this.isEmpty = true;
        this.pipeType = PipeType.None;
        this.rotation = 0;
    }

    public bool IsEmpty() => isEmpty;

    public void PlacePipe(PipeType type, int rot = 0)
    {
        pipeType = type;
        rotation = rot;
        isEmpty = false;

        // Instantiate pipe prefab
        CreatePipeVisual();
    }

    public void RemovePipe()
    {
        if (pipeObject != null)
            Object.Destroy(pipeObject);

        isEmpty = true;
        pipeType = PipeType.None;
        rotation = 0;
    }

    void CreatePipeVisual()
    {
        if (pipeType == PipeType.None)
        {
            Debug.LogWarning($"[GridCell] Tried to create pipe with type 'None' at ({x},{y})");
            return;
        }

        if (PipeManager.Instance == null)
        {
            Debug.LogError("[GridCell] PipeManager.Instance is NULL!");
            return;
        }

        GameObject prefab = PipeManager.Instance.GetPipePrefab(pipeType);

        if (prefab == null)
        {
            Debug.LogError($"[GridCell] No prefab found for pipe type: {pipeType}");
            return;
        }

        pipeObject = Object.Instantiate(prefab, worldPosition, Quaternion.Euler(0, 0, rotation * 90));
    }
}
