using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// PuzzleGenerator Ŭ������ IPuzzleGenerator�� �����ϸ�
// ���̵��� ���� ���� ������ ���� �ش� ������ �����մϴ�.
public class PuzzleGenerator : MonoBehaviour, IPuzzleGenerator
{
    [Header("���� ���� ����")]
    public GameObject puzzleCellPrefab;    // PuzzleCell ������
    public RectTransform boardRect;        // PuzzleBoard RectTransform
    public GridLayoutGroup gridLayout;     // GridLayoutGroup ������Ʈ

    private const int GridSize = 9;        // 9x9 ���� ���� ũ��

    private int[,] corrects = new int[9, 9];  // ���� �迭

    // ������/���̾ƿ� ��ȿ�� �˻� (Unity ��ü �ı��� ��쵵 ����)
    private bool IsValid(UnityEngine.Object obj)
    {
        return obj != null;
    }

    /// <summary>
    /// ���̵��� ���� ������ �����ϴ� �⺻ �޼���
    /// </summary>
    public void Generate(Transform parent, int gridSize, Difficulty difficulty)
    {
        // ������ �� ���̾ƿ� ������ �׻� �ֽ����� ���� (�߿�!)
        EnsureBoardConnections();

        // �� ũ�� ����
        ResizeCells();

        // 1) ���� �ش� ���� ����
        int[,] solution = GenerateFullSolution(gridSize);

        // 2) ���̵� ������� ��Ʈ(���� ��) ����ũ ����
        bool[,] mask = GenerateClueMask(solution, difficulty);

        // 3) PuzzleCell �ν��Ͻ� ���� �� �ʱ�ȭ
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject newCell = Instantiate(puzzleCellPrefab, parent);
                newCell.name = $"PuzzleCell_{y}_{x}";
                PuzzleCell cellComp = newCell.GetComponent<PuzzleCell>();
                int answer = solution[y, x];

                cellComp.correctValue = answer;  // ���� �� ����

                if (mask[y, x])
                {
                    // ������ ��Ʈ ��
                    cellComp.isFixed = true;
                    cellComp.cellText.text = answer.ToString();
                    cellComp.cellText.fontStyle = FontStyles.Bold;
                    cellComp.cellText.color = Color.black;
                }
                else
                {
                    // �� ĭ
                    cellComp.isFixed = false;
                    cellComp.cellText.text = string.Empty;
                    cellComp.cellText.fontStyle = FontStyles.Normal;
                }
            }
        }

        //���� �迭�� ����
        for (int y = 0; y < gridSize; y++)
            for (int x = 0; x < gridSize; x++)
                corrects[y, x] = solution[y, x];
    }


    /// <summary>
    /// ����� ���¿��� ������ ����(�ҷ�����)�ϴ� �Լ�
    /// </summary>
    public void GenerateFromState(Transform parent, int gridSize, Difficulty difficulty, int[,] values, bool[,] fixeds, int[,] corrects)
    {
        // ������ �� ���̾ƿ� ������ �׻� �ֽ����� ���� (�߿�!)
        EnsureBoardConnections();

        // ������ ������ �� ���̾ƿ� ���� ����
        ResizeCells();

        // ���� ���� �� ��� ����
        foreach (Transform child in parent)
            Destroy(child.gameObject);

        // 81�� �� ���� ���� (���� ����, �̸����� ��ġ)
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                var cellGO = Instantiate(puzzleCellPrefab, parent);
                cellGO.name = $"PuzzleCell_{r}_{c}"; // �̸� ��ġ
                var cell = cellGO.GetComponent<PuzzleCell>();
                int v = values[r, c];
                cell.cellText.text = v > 0 ? v.ToString() : "";
                cell.isFixed = fixeds[r, c];
                cell.correctValue = corrects[r, c];
            }
        }
    }

    // ������/���̾ƿ� ���� ���� �Լ�
    private void EnsureBoardConnections()
    {
        if (!IsValid(boardRect) || !IsValid(gridLayout))
        {
            var boardGO = GameObject.Find("PuzzleBoard");
            if (boardGO != null)
            {
                boardRect = boardGO.GetComponent<RectTransform>();
                gridLayout = boardGO.GetComponent<GridLayoutGroup>();
            }
        }
        // ���� ��ȿ�� üũ �� �α�
        if (!IsValid(boardRect) || !IsValid(gridLayout))
        {
            Debug.LogWarning("[PuzzleGenerator] boardRect �Ǵ� gridLayout�� ����Ǿ� ���� �ʽ��ϴ�!");
        }
    }

    //���� �迭 ��ȯ (GameManager���� ��������� ���)
    public int[,] GetCorrectValues()
    {
        return corrects;
    }

    /// <summary>
    /// ���� ȣ�� ȣȯ�� ���� �޼��� �����ε�
    /// </summary>
    public void Generate(Transform parent, int gridSize)
    {
        // ����� GameManager ���̵��� ���
        Generate(parent, gridSize, GameManager.Instance.difficulty);
    }

    /// <summary>
    /// ���� ���� ũ�⿡ ���� �� ũ�� ����
    /// </summary>
    private void ResizeCells()
    {
        if (boardRect == null || gridLayout == null)
        {
            Debug.LogWarning("[PuzzleGenerator] boardRect �Ǵ� gridLayout�� ����Ǿ� ���� �ʽ��ϴ�!");
            return;
        }
        float boardW = boardRect.rect.width;
        float boardH = boardRect.rect.height;
        float spaceX = gridLayout.spacing.x;
        float spaceY = gridLayout.spacing.y;

        float cellW = (boardW - spaceX * (GridSize - 1)) / GridSize;
        float cellH = (boardH - spaceY * (GridSize - 1)) / GridSize;
        float cellSize = Mathf.Min(cellW, cellH);

        gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }

    /// <summary>
    /// ��Ʈ��ŷ ������� ���� �ش� ���� ����
    /// </summary>
    private int[,] GenerateFullSolution(int n)
    {
        int[,] board = new int[n, n];
        FillBoard(board, 0, 0);
        return board;
    }

    private bool FillBoard(int[,] board, int row, int col)
    {
        if (row == GridSize) return true;
        int nextR = (col == GridSize - 1) ? row + 1 : row;
        int nextC = (col + 1) % GridSize;

        // 1~9�� ���� ������ �õ�
        var nums = Enumerable.Range(1, GridSize).OrderBy(_ => UnityEngine.Random.value).ToList();
        foreach (int num in nums)
        {
            if (CanPlace(board, row, col, num))
            {
                board[row, col] = num;
                if (FillBoard(board, nextR, nextC)) return true;
                board[row, col] = 0;
            }
        }
        return false;
    }

    /// <summary>
    /// Ư�� ��ġ�� ���ڸ� ���� �� �ִ��� �˻�
    /// </summary>
    private bool CanPlace(int[,] board, int row, int col, int num)
    {
        // ���� ��� �� üũ
        for (int i = 0; i < GridSize; i++)
            if (board[row, i] == num || board[i, col] == num)
                return false;

        // 3x3 �ڽ� üũ
        int boxR = (row / 3) * 3;
        int boxC = (col / 3) * 3;
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                if (board[boxR + r, boxC + c] == num)
                    return false;
        return true;
    }

    /// <summary>
    /// �־��� ���̵��� �°� ��Ʈ ����ũ(���� ��) ����, ���� �ش� ����
    /// </summary>
    private bool[,] GenerateClueMask(int[,] solution, Difficulty diff)
    {
        int n = GridSize;
        int clues = diff == Difficulty.Easy ? 40 :
                    diff == Difficulty.Normal ? 30 : 20;

        // 1) �ش� ����
        int[,] puzzle = solution.Clone() as int[,];

        // 2) �� ��ǥ ����Ʈ�� ���� ����
        var positions = new List<(int r, int c)>();
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                positions.Add((r, c));
        positions = positions.OrderBy(_ => UnityEngine.Random.value).ToList();

        int removed = 0;
        foreach (var (r, c) in positions)
        {
            int backup = puzzle[r, c];
            puzzle[r, c] = 0;

            // ���� �ش����� Ȯ��
            if (CountSolutions(puzzle, 2) != 1)
                puzzle[r, c] = backup;
            else
            {
                removed++;
                if (n * n - removed <= clues) break;
            }
        }

        // 3) ����ũ ���� (true = ���� ��)
        bool[,] mask = new bool[n, n];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                mask[r, c] = puzzle[r, c] != 0;
        return mask;
    }

    /// <summary>
    /// ���� Ƚ������ �ش� ���� ����
    /// </summary>
    private int CountSolutions(int[,] board, int limit)
    {
        int count = 0;
        SolveCount(board, 0, 0, ref count, limit);
        return count;
    }

    private void SolveCount(int[,] board, int row, int col, ref int count, int limit)
    {
        if (count >= limit) return;    // limit �̻��̸� ����
        if (row == GridSize)
        {
            count++;
            return;
        }
        int nextR = (col == GridSize - 1) ? row + 1 : row;
        int nextC = (col + 1) % GridSize;

        if (board[row, col] != 0)
        {
            SolveCount(board, nextR, nextC, ref count, limit);
        }
        else
        {
            for (int num = 1; num <= GridSize; num++)
            {
                if (CanPlace(board, row, col, num))
                {
                    board[row, col] = num;
                    SolveCount(board, nextR, nextC, ref count, limit);
                    board[row, col] = 0;
                    if (count >= limit) return;
                }
            }
        }
    }
}
