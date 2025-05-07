using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Puzzle Generator")]
    public IPuzzleGenerator puzzleGenerator;  // �������̽� �ʵ�
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

            // �� �ε� �ݹ� ���
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

    // ���� �ε�� ������ ȣ��˴ϴ�.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            // 1) PuzzleBoard ������Ʈ ã��
            var boardGO = GameObject.Find("PuzzleBoard");
            if (boardGO == null)
            {
                Debug.LogError("PuzzleBoard ������Ʈ�� ã�� ���߽��ϴ�.");
                return;
            }

            // 2) Transform �Ҵ�
            puzzleParent = boardGO.transform;

            // 3) PuzzleGenerator ������Ʈ �Ҵ�
            var gen = boardGO.GetComponent<PuzzleGenerator>();
            if (gen == null)
            {
                Debug.LogError("PuzzleGenerator ������Ʈ�� ã�� ���߽��ϴ�.");
                return;
            }
            puzzleGenerator = gen;

            // 4) ���� ���� ȣ��
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
            Debug.LogError("���� �����Ⱑ �Ҵ���� �ʾҽ��ϴ�!");
    }
}
