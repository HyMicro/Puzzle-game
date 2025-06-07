using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiningSystem : MonoBehaviour
{
    [Header("Mining Settings")]
    public float miningInterval = 3f;
    public int crystalsPerMining = 1;

    private List<Vector2Int> activeSources;
    private Coroutine miningCoroutine;

    void Start()
    {
        activeSources = new List<Vector2Int>();
        StartMining();
    }

    public void RegisterSource(Vector2Int sourcePos)
    {
        if (!activeSources.Contains(sourcePos))
        {
            activeSources.Add(sourcePos);
        }
    }

    public void UnregisterSource(Vector2Int sourcePos)
    {
        activeSources.Remove(sourcePos);
    }

    void StartMining()
    {
        if (miningCoroutine != null)
            StopCoroutine(miningCoroutine);

        miningCoroutine = StartCoroutine(MiningLoop());
    }

    IEnumerator MiningLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(miningInterval);

            foreach (Vector2Int source in activeSources)
            {
                for (int i = 0; i < crystalsPerMining; i++)
                {
                    GameManager.Instance.pathSystem.SpawnCrystal(source);
                    yield return new WaitForSeconds(0.2f); // Small delay between crystals
                }
            }
        }
    }

    public void UpdateSources()
    {
        activeSources.Clear();

        // Find all source points in the grid
        for (int x = 0; x < GameManager.Instance.gridWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.gridHeight; y++)
            {
                GridCell cell = GameManager.Instance.GetCell(x, y);
                if (cell != null && cell.pipeType == PipeType.Source)
                {
                    RegisterSource(new Vector2Int(x, y));
                }
            }
        }
    }
}