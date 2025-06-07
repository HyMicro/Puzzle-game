using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int highScore;
    public int currentLevel;
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public float musicVolume = 0.5f;
    public float sfxVolume = 1f;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private SaveData saveData;
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "savedata.json");
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, jsonData);
            Debug.Log("Game saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public void LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string jsonData = File.ReadAllText(savePath);
                saveData = JsonUtility.FromJson<SaveData>(jsonData);
                Debug.Log("Game loaded successfully!");
            }
            else
            {
                saveData = new SaveData();
                Debug.Log("No save file found, creating new save data.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load game: " + e.Message);
            saveData = new SaveData();
        }

        ApplyLoadedSettings();
    }

    void ApplyLoadedSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(saveData.musicVolume);
            AudioManager.Instance.SetSFXVolume(saveData.sfxVolume);
        }
    }

    public void UpdateHighScore(int score)
    {
        if (score > saveData.highScore)
        {
            saveData.highScore = score;
            SaveGame();
        }
    }

    public int GetHighScore() => saveData.highScore;
    public int GetCurrentLevel() => saveData.currentLevel;

    public void SetCurrentLevel(int level)
    {
        saveData.currentLevel = level;
        SaveGame();
    }
}