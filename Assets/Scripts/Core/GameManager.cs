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
    // IPuzzleGenerator 인터페이스로 변환하여, 난이도까지 받아 처리하도록 구현
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

    // 씬 로드 시 퍼즐 초기화/불러오기
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            // 퍼즐 관련 오브젝트 자동 연결 (GameScene에 존재해야 함)
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

    // 새 게임 시작(난이도 지정)
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
        // 퍼즐 생성
        puzzleGenerator.Generate(puzzleParent, gridSize, difficulty);

        // 힌트/타이머 리셋
        if (hintManager != null) hintManager.ResetHintCount();
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
            gameTimer.StartTimer();
        }
    }

    // 저장된 게임 불러오기
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
            // 퍼즐판 구성 (puzzleGenerator에 "복원용 생성" 기능이 있어야 함)
            puzzleGenerator.GenerateFromState(puzzleParent, gridSize, difficulty, values, fixeds, corrects);

            // 힌트/타이머 복원
            if (hintManager != null) hintManager.SetHintCount(hintCount);
            if (gameTimer != null)
            {
                gameTimer.SetElapsedTime(elapsedTime);
                gameTimer.StartTimer();
            }
        }
        else
        {
            // 저장 데이터가 없으면 새 게임으로
            Debug.LogWarning("[GameManager] 저장된 데이터 없음, 새 게임 시작");
            StartNewGameInternal();
        }
    }

    // 저장 호출 (예: 씬 이동/일시정지/임시저장 등)
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

    // 게임 클리어 시 통계 저장, UI 처리 등...
    public void CompleteGame()
    {
        float clearTime = gameTimer != null ? gameTimer.GetElapsedTime() : 0f;
        int hintCount = hintManager != null ? hintManager.GetHintCount() : 0;

        StatsManager.Instance.UpdateStats(difficulty, clearTime, hintCount);
        GameClearPanel.Instance.Show(clearTime);
    }
}
