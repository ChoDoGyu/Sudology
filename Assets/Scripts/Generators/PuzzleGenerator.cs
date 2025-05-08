using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class PuzzleGenerator : MonoBehaviour, IPuzzleGenerator
{
    [Header("References")]
    public GameObject puzzleCellPrefab;  // PuzzleCell 프리팹
    public RectTransform boardRect;      // PuzzleBoard RectTransform
    public GridLayoutGroup gridLayout;   // GridLayoutGroup 컴포넌트 참조

    private const int GridSize = 9; // 9x9 퍼즐


    public void Generate(Transform parent, int gridSize)
    {
        ResizeCells();

        // 1) 완전 해답 보드 생성
        int[,] solution = GenerateFullSolution(gridSize);

        // 2) 난이도별 클루(빈칸) 마스크 생성 (유일 해답 보장)
        bool[,] mask = GenerateClueMask(solution, GameManager.Instance.difficulty);

        // 3) 셀 생성 및 초기화
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
                    // 문제 셀(고정)
                    cellComp.isFixed = true;
                    cellComp.cellText.text = answer.ToString();
                    cellComp.cellText.fontStyle = FontStyles.Bold;
                    cellComp.cellText.color = Color.black;
                }
                else
                {
                    // 빈칸
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

    // 1) 백트래킹으로 완전 해답 보드 생성
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

        // 1~9를 랜덤 순서로 시도
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
        // 행/열 체크
        for (int i = 0; i < GridSize; i++)
            if (board[row, i] == num || board[i, col] == num)
                return false;
        // 3x3 박스 체크
        int boxR = (row / 3) * 3, boxC = (col / 3) * 3;
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                if (board[boxR + r, boxC + c] == num)
                    return false;
        return true;
    }

    // 2) 난이도별 클루 마스크 생성 (유일 해답 보장)
    private bool[,] GenerateClueMask(int[,] solution, Difficulty diff)
    {
        int n = GridSize;
        int clues = diff == Difficulty.Easy ? 40 :
                    diff == Difficulty.Normal ? 30 : 20;

        // 1) 모든 칸을 가려둔 상태로 시작
        int[,] puzzle = solution.Clone() as int[,];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                puzzle[r, c] = solution[r, c];

        // 2) 칸 제거 리스트를 랜덤 순서로 준비
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

            // 유일 해답인지 검사 (2개 이상이면 false)
            if (CountSolutions(puzzle, 2) != 1)
            {
                // 유일성이 깨지면 복원
                puzzle[r, c] = backup;
            }
            else
            {
                removed++;
                if (n * n - removed <= clues) break;
            }
        }

        // 3) mask 반환 (true=문제 셀)
        bool[,] mask = new bool[n, n];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                mask[r, c] = puzzle[r, c] != 0;
        return mask;
    }

    // 보드에서 해답 개수를 세고, limit 이상이면 조기 종료
    private int CountSolutions(int[,] board, int limit)
    {
        int count = 0;
        SolveCount(board, 0, 0, ref count, limit);
        return count;
    }

    private void SolveCount(int[,] board, int row, int col, ref int count, int limit)
    {
        if (count >= limit) return;  // limit 이상이면 조기 종료
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
