using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Puzzle Generator")]
    // IPuzzleGenerator �������̽��� ��ȯ�Ͽ�, ���̵����� �޾� ó���ϵ��� ����
    [SerializeField] private IPuzzleGenerator puzzleGenerator;
    [SerializeField] private Transform puzzleParent;
    [SerializeField] private int gridSize = 9;

    [Header("Game Settings")]
    public Difficulty difficulty = Difficulty.Normal;

    public GameTimer gameTimer;
    [SerializeField] private HintManager hintManager;

    private bool isNewGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // �� �ε� �� ���� �ʱ�ȭ/�ҷ�����
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            // ���� ���� ������Ʈ �ڵ� ���� (GameScene�� �����ؾ� ��)
            if (puzzleGenerator == null)
                puzzleGenerator = FindObjectOfType<PuzzleGenerator>();
            if (puzzleParent == null)
                puzzleParent = GameObject.Find("PuzzleBoard")?.transform;
            if (gameTimer == null)
                gameTimer = FindObjectOfType<GameTimer>();
            if (hintManager == null)
                hintManager = FindObjectOfType<HintManager>();

            if (isNewGame)
            {
                StartNewGameInternal();
            }
            else
            {
                LoadSavedGameInternal();
            }
        }
    }

    // �� ���� ����(���̵� ����)
    public void SetDifficulty(Difficulty diff)
    {
        difficulty = diff;
    }

    public void StartNewGame()
    {
        isNewGame = true;
        SceneManager.LoadScene("GameScene");
    }

    private void StartNewGameInternal()
    {
        // ���� ����
        puzzleGenerator.Generate(puzzleParent, gridSize, difficulty);

        // ��Ʈ/Ÿ�̸� ����
        if (hintManager != null) hintManager.ResetHintCount();
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
            gameTimer.StartTimer();
        }
    }

    // ����� ���� �ҷ�����
    public void SetContinueGame()
    {
        isNewGame = false;
        SceneManager.LoadScene("GameScene");
    }

    private void LoadSavedGameInternal()
    {
        int[,] values; bool[,] fixeds; int[,] corrects; int hintCount; float elapsedTime;
        if (SaveManager.Instance.LoadState(out values, out fixeds, out corrects, out hintCount, out elapsedTime))
        {
            // ������ ���� (puzzleGenerator�� "������ ����" ����� �־�� ��)
            puzzleGenerator.GenerateFromState(puzzleParent, gridSize, difficulty, values, fixeds, corrects);

            // ��Ʈ/Ÿ�̸� ����
            if (hintManager != null) hintManager.SetHintCount(hintCount);
            if (gameTimer != null)
            {
                gameTimer.SetElapsedTime(elapsedTime);
                gameTimer.StartTimer();
            }
        }
        else
        {
            // ���� �����Ͱ� ������ �� ��������
            Debug.LogWarning("[GameManager] ����� ������ ����, �� ���� ����");
            StartNewGameInternal();
        }
    }

    // ���� ȣ�� (��: �� �̵�/�Ͻ�����/�ӽ����� ��)
    public void SaveCurrentState()
    {
        int[,] values = new int[9, 9];
        bool[,] fixeds = new bool[9, 9];
        int[,] corrects = new int[9, 9];

        var cells = puzzleParent.GetComponentsInChildren<PuzzleCell>();
        for (int i = 0; i < cells.Length; i++)
        {
            int r = i / 9;
            int c = i % 9;
            string text = cells[i].cellText != null ? cells[i].cellText.text : "";
            values[r, c] = int.TryParse(text, out int v) ? v : 0;
            fixeds[r, c] = cells[i].isFixed;
            corrects[r, c] = cells[i].correctValue;
        }

        int hintCount = hintManager != null ? hintManager.GetHintCount() : 0;
        float elapsedTime = gameTimer != null ? gameTimer.GetElapsedTime() : 0f;

        SaveManager.Instance.SaveState(values, fixeds, corrects, hintCount, elapsedTime);
    }

    // ���� Ŭ���� �� ��� ����, UI ó�� ��...
    public void CompleteGame()
    {
        float clearTime = gameTimer != null ? gameTimer.GetElapsedTime() : 0f;
        int hintCount = hintManager != null ? hintManager.GetHintCount() : 0;

        StatsManager.Instance.UpdateStats(difficulty, clearTime, hintCount);
        GameClearPanel.Instance.Show(clearTime);
    }
}
