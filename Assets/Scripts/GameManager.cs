using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Puzzle Generator")]
    public IPuzzleGenerator puzzleGenerator;  // 인터페이스 필드
    public Transform puzzleParent;            // PuzzleBoard Transform
    public int gridSize = 9;

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
                Debug.LogError("PuzzleBoard 오브젝트를 찾지 못했습니다.");
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
        if (puzzleGenerator != null)
            puzzleGenerator.Generate(puzzleParent, gridSize);
        else
            Debug.LogError("퍼즐 생성기가 할당되지 않았습니다!");
    }
}
