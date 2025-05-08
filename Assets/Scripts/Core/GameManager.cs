using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Puzzle Generator")]
    public IPuzzleGenerator puzzleGenerator;  // ���� ������
    public Transform puzzleParent;            // PuzzleBoard Transform
    public int gridSize = 9;                  // �׸��� ũ��

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
                Debug.LogError("PuzzleBoard ������Ʈ�� ã�� �� �����ϴ�.");
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
        // 1) ���� ����
        puzzleGenerator.Generate(puzzleParent, gridSize);

        // 2) ����� ���� �ҷ�����
        var saved = SaveManager.Instance.LoadState();
        if (saved != null)
        {
            var cells = puzzleParent.GetComponentsInChildren<PuzzleCell>();
            for (int i = 0; i < cells.Length; i++)
            {
                // 1���� �ε����� 2���� ��/���� ��ȯ
                int r = i / gridSize, c = i % gridSize;

                // ����� ��/���� �÷��� ����
                cells[i].isFixed = saved.fixeds[i];
                cells[i].cellText.text = saved.values[i] == 0
                                                   ? ""
                                                   : saved.values[i].ToString();
                cells[i].ResetColor();  // ���� ����
            }
        }
        else
        {
            // �� ���� �� ���� ������ ����
            SaveManager.Instance.ClearState();
        }
    }
}
