using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public Button pauseButton;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Preview UI")]
    public Transform previewParent;
    public GameObject previewSlotPrefab;

    private int currentScore = 0;
    private int currentLevel = 1;

    void Start()
    {
        UpdateUI();
        SetupButtons();
    }

    void SetupButtons()
    {
        pauseButton.onClick.AddListener(TogglePause);
    }

    public void UpdateScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    public void UpdateLevel(int level)
    {
        currentLevel = level;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + currentScore;
        levelText.text = "Level: " + currentLevel;
    }

    public void TogglePause()
    {
        bool isPaused = !pausePanel.activeInHierarchy;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}