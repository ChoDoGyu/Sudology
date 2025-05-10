using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (scene.name != "GameScene")
            return;

        // 1) PuzzleBoard ������Ʈ ã��
        var boardGO = GameObject.Find("PuzzleBoard");
        if (boardGO == null)
        {
            Debug.LogError("PuzzleBoard ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // 2) ���� �Ҵ�
        puzzleParent = boardGO.transform;
        puzzleGenerator = boardGO.GetComponent<IPuzzleGenerator>();
        if (puzzleGenerator == null)
        {
            Debug.LogError("PuzzleGenerator ������Ʈ�� ã�� ���߽��ϴ�.");
            return;
        }

        // 3) ���� ���� �� ���� ����ȭ
        GeneratePuzzle();
    }

    /// <summary>
    /// ���̵��� �����մϴ�. 
    /// DifficultySelector���� ȣ���.
    /// </summary>
    public void SetDifficulty(Difficulty d)
    {
        difficulty = d;
    }

    /// <summary>
    /// ������ ���� �����ϰų�, ����� ���°� ������ �ҷ��ͼ� ����ȭ�մϴ�.
    /// </summary>
    public void GeneratePuzzle()
    {
        // (A) ���� ����: ���̵����� �Բ� ����
        //    IPuzzleGenerator.Generate(Transform parent, int size, Difficulty diff) �����ε� �ʿ�
        puzzleGenerator.Generate(puzzleParent, gridSize, difficulty);

        // (B) ����� ���°� ������ �ҷ�����, ������ Clear
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
    /// Continue ��ư���� ȣ��˴ϴ�.
    /// ����� ������ ������ ��� ����, ������ GameScene���� ��ȯ�մϴ�.
    /// </summary>
    public void StartContinueGame()
    {
        if (!SaveManager.Instance.HasSave())
        {
            Debug.LogWarning("����� ������ �����ϴ�!");
            return;
        }

        // GameScene �ε� �� OnSceneLoaded �ݹ鿡�� ���� & ���� ����ȭ�� �Ͼ�ϴ�.
        SceneManager.LoadScene("GameScene");
    }
}
