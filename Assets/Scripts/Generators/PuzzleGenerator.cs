using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class PuzzleGenerator : MonoBehaviour, IPuzzleGenerator
{
    [Header("References")]
    public GameObject puzzleCellPrefab;  // PuzzleCell ������
    public RectTransform boardRect;      // PuzzleBoard RectTransform
    public GridLayoutGroup gridLayout;   // GridLayoutGroup ������Ʈ ����

    private const int GridSize = 9; // 9x9 ����


    public void Generate(Transform parent, int gridSize)
    {
        ResizeCells();

        // 1) ���� �ش� ���� ����
        int[,] solution = GenerateFullSolution(gridSize);

        // 2) ���̵��� Ŭ��(��ĭ) ����ũ ���� (���� �ش� ����)
        bool[,] mask = GenerateClueMask(solution, GameManager.Instance.difficulty);

        // 3) �� ���� �� �ʱ�ȭ
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject newCell = Instantiate(puzzleCellPrefab, parent);
                newCell.name = $"PuzzleCell_{x}_{y}";
                var cellComp = newCell.GetComponent<PuzzleCell>();
                int answer = solution[y, x];
                cellComp.correctValue = answer;

                if (mask[y, x])
                {
                    // ���� ��(����)
                    cellComp.isFixed = true;
                    cellComp.cellText.text = answer.ToString();
                    cellComp.cellText.fontStyle = FontStyles.Bold;
                    cellComp.cellText.color = Color.black;
                }
                else
                {
                    // ��ĭ
                    cellComp.isFixed = false;
                    cellComp.cellText.text = "";
                    cellComp.cellText.fontStyle = FontStyles.Normal;
                }
            }
        }
    }

    private void ResizeCells()
    {
        float boardW = boardRect.rect.width;
        float boardH = boardRect.rect.height;
        float spacingX = gridLayout.spacing.x;
        float spacingY = gridLayout.spacing.y;

        float cellW = (boardW - spacingX * (GridSize - 1)) / GridSize;
        float cellH = (boardH - spacingY * (GridSize - 1)) / GridSize;
        float cellSize = Mathf.Min(cellW, cellH);

        gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }

    // 1) ��Ʈ��ŷ���� ���� �ش� ���� ����
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
        var nums = Enumerable.Range(1, GridSize).OrderBy(_ => Random.value).ToList();
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

    private bool CanPlace(int[,] board, int row, int col, int num)
    {
        // ��/�� üũ
        for (int i = 0; i < GridSize; i++)
            if (board[row, i] == num || board[i, col] == num)
                return false;
        // 3x3 �ڽ� üũ
        int boxR = (row / 3) * 3, boxC = (col / 3) * 3;
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                if (board[boxR + r, boxC + c] == num)
                    return false;
        return true;
    }

    // 2) ���̵��� Ŭ�� ����ũ ���� (���� �ش� ����)
    private bool[,] GenerateClueMask(int[,] solution, Difficulty diff)
    {
        int n = GridSize;
        int clues = diff == Difficulty.Easy ? 40 :
                    diff == Difficulty.Normal ? 30 : 20;

        // 1) ��� ĭ�� ������ ���·� ����
        int[,] puzzle = solution.Clone() as int[,];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                puzzle[r, c] = solution[r, c];

        // 2) ĭ ���� ����Ʈ�� ���� ������ �غ�
        var positions = new List<(int r, int c)>();
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                positions.Add((r, c));
        positions = positions.OrderBy(_ => Random.value).ToList();

        int removed = 0;
        foreach (var (r, c) in positions)
        {
            int backup = puzzle[r, c];
            puzzle[r, c] = 0;

            // ���� �ش����� �˻� (2�� �̻��̸� false)
            if (CountSolutions(puzzle, 2) != 1)
            {
                // ���ϼ��� ������ ����
                puzzle[r, c] = backup;
            }
            else
            {
                removed++;
                if (n * n - removed <= clues) break;
            }
        }

        // 3) mask ��ȯ (true=���� ��)
        bool[,] mask = new bool[n, n];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                mask[r, c] = puzzle[r, c] != 0;
        return mask;
    }

    // ���忡�� �ش� ������ ����, limit �̻��̸� ���� ����
    private int CountSolutions(int[,] board, int limit)
    {
        int count = 0;
        SolveCount(board, 0, 0, ref count, limit);
        return count;
    }

    private void SolveCount(int[,] board, int row, int col, ref int count, int limit)
    {
        if (count >= limit) return;  // limit �̻��̸� ���� ����
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
