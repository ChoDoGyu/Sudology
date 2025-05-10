using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (scene.name != "GameScene")
            return;

        // 1) PuzzleBoard 오브젝트 찾기
        var boardGO = GameObject.Find("PuzzleBoard");
        if (boardGO == null)
        {
            Debug.LogError("PuzzleBoard 오브젝트를 찾을 수 없습니다.");
            return;
        }

        // 2) 참조 할당
        puzzleParent = boardGO.transform;
        puzzleGenerator = boardGO.GetComponent<IPuzzleGenerator>();
        if (puzzleGenerator == null)
        {
            Debug.LogError("PuzzleGenerator 컴포넌트를 찾지 못했습니다.");
            return;
        }

        // 3) 퍼즐 생성 및 상태 동기화
        GeneratePuzzle();
    }

    /// <summary>
    /// 난이도를 설정합니다. 
    /// DifficultySelector에서 호출됨.
    /// </summary>
    public void SetDifficulty(Difficulty d)
    {
        difficulty = d;
    }

    /// <summary>
    /// 퍼즐을 새로 생성하거나, 저장된 상태가 있으면 불러와서 동기화합니다.
    /// </summary>
    public void GeneratePuzzle()
    {
        // (A) 퍼즐 생성: 난이도까지 함께 전달
        //    IPuzzleGenerator.Generate(Transform parent, int size, Difficulty diff) 오버로드 필요
        puzzleGenerator.Generate(puzzleParent, gridSize, difficulty);

        // (B) 저장된 상태가 있으면 불러오기, 없으면 Clear
        var saved = SaveManager.Instance.LoadState();
        if (saved != null)
        {
            var cells = puzzleParent.GetComponentsInChildren<PuzzleCell>();
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].isFixed = saved.fixeds[i];
                cells[i].cellText.text =
                    saved.values[i] == 0 ? "" : saved.values[i].ToString();
                cells[i].ResetColor();
            }
        }
        else
        {
            SaveManager.Instance.ClearState();
        }
    }

    /// <summary>
    /// Continue 버튼에서 호출됩니다.
    /// 저장된 게임이 없으면 경고만 띄우고, 있으면 GameScene으로 전환합니다.
    /// </summary>
    public void StartContinueGame()
    {
        if (!SaveManager.Instance.HasSave())
        {
            Debug.LogWarning("저장된 게임이 없습니다!");
            return;
        }

        // GameScene 로드 → OnSceneLoaded 콜백에서 퍼즐 & 상태 동기화가 일어납니다.
        SceneManager.LoadScene("GameScene");
    }
}
