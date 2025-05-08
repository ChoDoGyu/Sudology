using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Puzzle Generator")]
    public IPuzzleGenerator puzzleGenerator;  // 퍼즐 생성기
    public Transform puzzleParent;            // PuzzleBoard Transform
    public int gridSize = 9;                  // 그리드 크기

    [Header("Game Settings")]
    public Difficulty difficulty = Difficulty.Normal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 씬 로드 콜백 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
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
            // 1) PuzzleBoard 오브젝트 찾기
            var boardGO = GameObject.Find("PuzzleBoard");
            if (boardGO == null)
            {
                Debug.LogError("PuzzleBoard 오브젝트를 찾을 수 없습니다.");
                return;
            }

            // 2) Transform 할당
            puzzleParent = boardGO.transform;

            // 3) PuzzleGenerator 컴포넌트 할당
            var gen = boardGO.GetComponent<PuzzleGenerator>();
            if (gen == null)
            {
                Debug.LogError("PuzzleGenerator 컴포넌트를 찾지 못했습니다.");
                return;
            }
            puzzleGenerator = gen;

            // 4) 퍼즐 생성 호출
            GeneratePuzzle();
        }
    }

    public void SetDifficulty(Difficulty d)
    {
        difficulty = d;
    }

    public void GeneratePuzzle()
    {
        // 1) 퍼즐 생성
        puzzleGenerator.Generate(puzzleParent, gridSize);

        // 2) 저장된 상태 불러오기
        var saved = SaveManager.Instance.LoadState();
        if (saved != null)
        {
            var cells = puzzleParent.GetComponentsInChildren<PuzzleCell>();
            for (int i = 0; i < cells.Length; i++)
            {
                // 1차원 인덱스를 2차원 행/열로 변환
                int r = i / gridSize, c = i % gridSize;

                // 저장된 값/고정 플래그 적용
                cells[i].isFixed = saved.fixeds[i];
                cells[i].cellText.text = saved.values[i] == 0
                                                   ? ""
                                                   : saved.values[i].ToString();
                cells[i].ResetColor();  // 배경색 복원
            }
        }
        else
        {
            // 새 게임 시 이전 데이터 삭제
            SaveManager.Instance.ClearState();
        }
    }
}
