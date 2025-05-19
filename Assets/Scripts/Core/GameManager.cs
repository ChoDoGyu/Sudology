using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Puzzle Generator")]
    // IPuzzleGenerator �������̽��� ��ȯ�Ͽ�, ���̵����� �޾� ó���ϵ��� ����
    public IPuzzleGenerator puzzleGenerator;
    public Transform puzzleParent;
    public int gridSize = 9;

    [Header("Game Settings")]
    public Difficulty difficulty = Difficulty.Normal;

    [SerializeField] 
    public GameTimer gameTimer;

    [HideInInspector]
    public int currentHintCount = 0;

    [HideInInspector]
    public bool isNewGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // �� ��ȯ �� �ݹ�
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ���� �ε�� ������ ȣ��˴ϴ�.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            gameTimer = FindObjectOfType<GameTimer>();
            if (gameTimer == null)
            {
                Debug.LogError("[GameManager] GameTimer�� ã�� ���߽��ϴ�.");
            }
            else
            {
                Debug.Log("[GameManager] GameTimer ���� ����");
            }

            var boardGO = GameObject.Find("PuzzleBoard");
            if (boardGO == null) { Debug.LogError("PuzzleBoard ����"); return; }

            puzzleParent = boardGO.transform;
            puzzleGenerator = boardGO.GetComponent<IPuzzleGenerator>();
            if (puzzleGenerator == null) { Debug.LogError("PuzzleGenerator ����"); return; }

            //���⼭ �׻� �� ������Ʈ���� ����� (��, ���� �����)
            puzzleGenerator.Generate(puzzleParent, gridSize, difficulty);

            if (isNewGame)
                GenerateNewPuzzle();
            else
                LoadSavedPuzzle();
        }
        else
        {
            // GameScene�� �ƴ� ������ ���� ���� Ÿ�̸� ���� ����
            if (gameTimer != null)
            {
                float time = gameTimer.StopTimer();
                PlayerPrefs.SetFloat("LastElapsedTime", time);
                PlayerPrefs.Save();
                Debug.Log($"[GameManager] GameScene ��Ż - Ÿ�̸� ���� �� �ð� ����: {time}��");
            }
        }
    }

    /// <summary>
    /// ���̵��� �����մϴ�. 
    /// DifficultySelector���� ȣ���.
    /// </summary>
    public void SetDifficulty(Difficulty d)
    {
        difficulty = d;
    }

    private void GenerateNewPuzzle()
    {
        var newCells = puzzleParent.GetComponentsInChildren<PuzzleCell>();


        // �̸��� ���� (�׻� �»�� �� ���ϴ�)
        var orderedCells = newCells.OrderBy(cell => cell.name).ToArray();

        // ���� ������ ����
        int[,] values = new int[9, 9];
        bool[,] fixeds = new bool[9, 9];
        int[,] corrects = (puzzleGenerator as PuzzleGenerator)?.GetCorrectValues();

        // ���� �� ���� �� ������ �迭 ����

        for (int i = 0; i < orderedCells.Length; i++)
        {
            int r = i / 9, c = i % 9;
            var cell = orderedCells[i];
            values[r, c] = string.IsNullOrEmpty(cell.cellText.text) ? 0 : int.Parse(cell.cellText.text);
            fixeds[r, c] = cell.isFixed;
            // corrects�� PuzzleGenerator���� ��ȯ�� �� �״�� ���
        }

        // **���⼭ ���� ����!**
        SaveManager.Instance.SaveState(values, fixeds, corrects);
        PlayerPrefs.SetFloat("LastElapsedTime", 0f);
        PlayerPrefs.SetInt("HintCount", 0);
        PlayerPrefs.Save();

        currentHintCount = 0;
        gameTimer?.StartTimer();

        Debug.Log("[GameManager] �� ���� ���� �� ��� ���� �Ϸ�");
    }

    private void LoadSavedPuzzle()
    {
        var saved = SaveManager.Instance.LoadState();

        if (saved == null)
        {
            Debug.LogWarning("����� ������ ���� �� ������ �����մϴ�.");
            return;
        }

        var cells = puzzleParent.GetComponentsInChildren<PuzzleCell>();
        //���� ���� ���� �ٽ� Generate ���� ������.
        if (cells.Length == 0)
        {
            Debug.LogError("���� ���� �������� �ʽ��ϴ�. LoadSavedPuzzle ����!");
            return;
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].isFixed = saved.fixeds[i];
            cells[i].cellText.text = saved.values[i] == 0 ? "" : saved.values[i].ToString();
            cells[i].correctValue = saved.corrects[i];
            cells[i].ResetColor();
        }

        float resumeTime = PlayerPrefs.GetFloat("LastElapsedTime", 0f);
        gameTimer?.ResumeFrom(resumeTime);
        currentHintCount = PlayerPrefs.GetInt("HintCount", 0);

        Debug.Log("[GameManager] ����� ���� �ҷ����� �Ϸ�");
    }

    /// <summary>
    /// Continue ��ư���� ȣ��˴ϴ�.
    /// ����� ������ ������ ��� ����, ������ GameScene���� ��ȯ�մϴ�.
    /// </summary>
    public void StartContinueGame()
    {
        isNewGame = false;
        // GameScene �ε� �� OnSceneLoaded �ݹ鿡�� ���� & ���� ����ȭ�� �Ͼ�ϴ�.
        SceneManager.LoadScene("GameScene");
    }

    public void StartNewGame()
    {
        isNewGame = true;
        SceneManager.LoadScene("GameScene");  // GameScene���� �̵�
    }

    public void CompleteGame()
    {
        float finalTime = gameTimer.StopTimer();
        Debug.Log("���� �ҿ� �ð�: " + finalTime + "��");
        SaveGameTime(finalTime);

        //��� ������Ʈ
        string difficultyKey = difficulty.ToString();  // ex: "Easy", "Normal", "Hard"

        StatsData stats = StatsManager.Instance.LoadStats(difficultyKey);

        // Ŭ���� Ƚ�� ����
        stats.clearCount++;

        // �ִ� �ð� ���� (ó���̰ų�, �̹� ����� �� ª�ٸ� ����)
        if (stats.bestTime <= 0 || finalTime < stats.bestTime)
            stats.bestTime = finalTime;

        // ��� �ð� ���
        stats.avgTime = ((stats.avgTime * (stats.clearCount - 1)) + finalTime) / stats.clearCount;

        // ��� ��Ʈ ��� Ƚ�� ���
        int hintsUsed = currentHintCount;
        stats.avgHintUsage = ((stats.avgHintUsage * (stats.clearCount - 1)) + hintsUsed) / stats.clearCount;

        StatsManager.Instance.SaveStats(difficultyKey, stats);  //����

        GameClearPanel.Instance.Show(finalTime);  // ���â ����
    }

    // JSON �Ǵ� PlayerPrefs�� ����
    private void SaveGameTime(float time)
    {
        PlayerPrefs.SetFloat("LastGameTime", time);
        PlayerPrefs.Save();
    }
}
