using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int targetScore;
    public float crystalSpawnRate;
    public int gridWidth;
    public int gridHeight;
    public List<Vector2Int> predefinedSources;
    public List<Vector2Int> predefinedCollectors;
}

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public List<LevelData> levels;

    private int currentLevelIndex = 0;
    private LevelData currentLevel;

    void Start()
    {
        LoadLevel(0);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            currentLevelIndex = levelIndex;
            currentLevel = levels[levelIndex];
            SetupLevel();
        }
    }

    void SetupLevel()
    {
        // Update game manager with level settings
        GameManager.Instance.gridWidth = currentLevel.gridWidth;
        GameManager.Instance.gridHeight = currentLevel.gridHeight;

        // Place predefined sources and collectors
        foreach (Vector2Int sourcePos in currentLevel.predefinedSources)
        {
            GameManager.Instance.PlacePipe(PipeType.Source, sourcePos.x, sourcePos.y);
        }

        foreach (Vector2Int collectorPos in currentLevel.predefinedCollectors)
        {
            GameManager.Instance.PlacePipe(PipeType.Collector, collectorPos.x, collectorPos.y);
        }

        // Update mining system
        GameManager.Instance.miningSystem.miningInterval = 1f / currentLevel.crystalSpawnRate;
        GameManager.Instance.miningSystem.UpdateSources();

        // Update UI
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager)
        {
            uiManager.UpdateLevel(currentLevel.levelNumber);
        }
    }

    public bool CheckLevelComplete(int currentScore)
    {
        return currentScore >= currentLevel.targetScore;
    }

    public void NextLevel()
    {
        if (currentLevelIndex + 1 < levels.Count)
        {
            LoadLevel(currentLevelIndex + 1);
        }
        else
        {
            // Game completed
            ShowGameComplete();
        }
    }

    void ShowGameComplete()
    {
        // Show completion screen
        Debug.Log("Game Completed!");
    }
}