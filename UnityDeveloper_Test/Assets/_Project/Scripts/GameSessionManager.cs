using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }
    public bool IsGameActive => isGameActive;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI cubeCounterText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 120f;
    [SerializeField] private int totalCubes = 5;

    private float currentTime;
    private int cubesCollected = 0;
    private bool isGameActive = true;

    private void Awake()
    {
        Time.timeScale = 1f;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentTime = timeLimit;
        gameOverPanel.SetActive(false);
        UpdateCubeUI();
    }

    private void Update()
    {
        if (!isGameActive) return;

        // Timer Logic
        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0)
        {
            EndGame("TIME UP! Game Over.");
        }
    }

    public void CollectCube()
    {
        cubesCollected++;
        UpdateCubeUI();

        if (cubesCollected >= totalCubes)
        {
            EndGame("MISSION COMPLETE! You collected all cubes.");
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateCubeUI()
    {
        cubeCounterText.text = $"Cubes: {cubesCollected} / {totalCubes}";
    }

    public void EndGame(string message)
    {
        isGameActive = false;
        Time.timeScale = 0; // Freeze the game
        resultText.text = message;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // Reset global time scale so the reloaded scene isn't paused
        Time.timeScale = 1f;

        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}