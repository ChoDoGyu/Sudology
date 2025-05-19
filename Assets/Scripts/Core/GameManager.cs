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
    // IPuzzleGenerator 인터페이스로 변환하여, 난이도까지 받아 처리하도록 구현
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
            // 씬 전환 시 콜백
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때마다 호출됩니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            gameTimer = FindObjectOfType<GameTimer>();
            if (gameTimer == null)
            {
                Debug.LogError("[GameManager] GameTimer를 찾지 못했습니다.");
            }
            else
            {
                Debug.Log("[GameManager] GameTimer 연결 성공");
            }

            var boardGO = GameObject.Find("PuzzleBoard");
            if (boardGO == null) { Debug.LogError("PuzzleBoard 없음"); return; }

            puzzleParent = boardGO.transform;
            puzzleGenerator = boardGO.GetComponent<IPuzzleGenerator>();
            if (puzzleGenerator == null) { Debug.LogError("PuzzleGenerator 없음"); return; }

            //여기서 항상 셀 오브젝트부터 만든다 (단, 값은 비워둠)
            puzzleGenerator.Generate(puzzleParent, gridSize, difficulty);

            if (isNewGame)
                GenerateNewPuzzle();
            else
                LoadSavedPuzzle();
        }
        else
        {
            // GameScene이 아닌 씬으로 나갈 때는 타이머 강제 정지
            if (gameTimer != null)
            {
                float time = gameTimer.StopTimer();
                PlayerPrefs.SetFloat("LastElapsedTime", time);
                PlayerPrefs.Save();
                Debug.Log($"[GameManager] GameScene 이탈 - 타이머 멈춤 및 시간 저장: {time}초");
            }
        }
    }

    /// <summary>
    /// 난이도를 설정합니다. 
    /// DifficultySelector에서 호출됨.
    /// </summary>
    public void SetDifficulty(Difficulty d)
    {
        difficulty = d;
    }

    private void GenerateNewPuzzle()
    {
        var newCells = puzzleParent.GetComponentsInChildren<PuzzleCell>();


        // 이름순 정렬 (항상 좌상단 → 우하단)
        var orderedCells = newCells.OrderBy(cell => cell.name).ToArray();

        // 퍼즐 데이터 생성
        int[,] values = new int[9, 9];
        bool[,] fixeds = new bool[9, 9];
        int[,] corrects = (puzzleGenerator as PuzzleGenerator)?.GetCorrectValues();

        // 셀에 값 적용 및 데이터 배열 생성

        for (int i = 0; i < orderedCells.Length; i++)
        {
            int r = i / 9, c = i % 9;
            var cell = orderedCells[i];
            values[r, c] = string.IsNullOrEmpty(cell.cellText.text) ? 0 : int.Parse(cell.cellText.text);
            fixeds[r, c] = cell.isFixed;
            // corrects는 PuzzleGenerator에서 반환된 값 그대로 사용
        }

        // **여기서 퍼즐 저장!**
        SaveManager.Instance.SaveState(values, fixeds, corrects);
        PlayerPrefs.SetFloat("LastElapsedTime", 0f);
        PlayerPrefs.SetInt("HintCount", 0);
        PlayerPrefs.Save();

        currentHintCount = 0;
        gameTimer?.StartTimer();

        Debug.Log("[GameManager] 새 퍼즐 생성 및 즉시 저장 완료");
    }

    private void LoadSavedPuzzle()
    {
        var saved = SaveManager.Instance.LoadState();

        if (saved == null)
        {
            Debug.LogWarning("저장된 게임이 없어 새 퍼즐을 생성합니다.");
            return;
        }

        var cells = puzzleParent.GetComponentsInChildren<PuzzleCell>();
        //절대 퍼즐 셀을 다시 Generate 하지 마세요.
        if (cells.Length == 0)
        {
            Debug.LogError("퍼즐 셀이 존재하지 않습니다. LoadSavedPuzzle 실패!");
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

        Debug.Log("[GameManager] 저장된 퍼즐 불러오기 완료");
    }

    /// <summary>
    /// Continue 버튼에서 호출됩니다.
    /// 저장된 게임이 없으면 경고만 띄우고, 있으면 GameScene으로 전환합니다.
    /// </summary>
    public void StartContinueGame()
    {
        isNewGame = false;
        // GameScene 로드 → OnSceneLoaded 콜백에서 퍼즐 & 상태 동기화가 일어납니다.
        SceneManager.LoadScene("GameScene");
    }

    public void StartNewGame()
    {
        isNewGame = true;
        SceneManager.LoadScene("GameScene");  // GameScene으로 이동
    }

    public void CompleteGame()
    {
        float finalTime = gameTimer.StopTimer();
        Debug.Log("최종 소요 시간: " + finalTime + "초");
        SaveGameTime(finalTime);

        //통계 업데이트
        string difficultyKey = difficulty.ToString();  // ex: "Easy", "Normal", "Hard"

        StatsData stats = StatsManager.Instance.LoadStats(difficultyKey);

        // 클리어 횟수 증가
        stats.clearCount++;

        // 최단 시간 저장 (처음이거나, 이번 기록이 더 짧다면 갱신)
        if (stats.bestTime <= 0 || finalTime < stats.bestTime)
            stats.bestTime = finalTime;

        // 평균 시간 계산
        stats.avgTime = ((stats.avgTime * (stats.clearCount - 1)) + finalTime) / stats.clearCount;

        // 평균 힌트 사용 횟수 계산
        int hintsUsed = currentHintCount;
        stats.avgHintUsage = ((stats.avgHintUsage * (stats.clearCount - 1)) + hintsUsed) / stats.clearCount;

        StatsManager.Instance.SaveStats(difficultyKey, stats);  //저장

        GameClearPanel.Instance.Show(finalTime);  // 결과창 띄우기
    }

    // JSON 또는 PlayerPrefs에 저장
    private void SaveGameTime(float time)
    {
        PlayerPrefs.SetFloat("LastGameTime", time);
        PlayerPrefs.Save();
    }
}
