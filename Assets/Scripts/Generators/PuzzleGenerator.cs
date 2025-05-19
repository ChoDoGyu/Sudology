using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// PuzzleGenerator 클래스는 IPuzzleGenerator를 구현하며
// 난이도에 따른 퍼즐 생성과 유일 해답 보장을 수행합니다.
public class PuzzleGenerator : MonoBehaviour, IPuzzleGenerator
{
    [Header("퍼즐 참조 설정")]
    public GameObject puzzleCellPrefab;    // PuzzleCell 프리팹
    public RectTransform boardRect;        // PuzzleBoard RectTransform
    public GridLayoutGroup gridLayout;     // GridLayoutGroup 컴포넌트

    private const int GridSize = 9;        // 9x9 퍼즐 고정 크기

    private int[,] corrects = new int[9, 9];  // 정답 배열

    /// <summary>
    /// 난이도에 따라 퍼즐을 생성하는 기본 메서드
    /// </summary>
    public void Generate(Transform parent, int gridSize, Difficulty difficulty)
    {
        // 셀 크기 조정
        ResizeCells();

        // 1) 완전 해답 보드 생성
        int[,] solution = GenerateFullSolution(gridSize);

        // 2) 난이도 기반으로 힌트(고정 셀) 마스크 생성
        bool[,] mask = GenerateClueMask(solution, difficulty);


        // 3) PuzzleCell 인스턴스 생성 및 초기화
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject newCell = Instantiate(puzzleCellPrefab, parent);
                newCell.name = $"PuzzleCell_{x}_{y}";
                PuzzleCell cellComp = newCell.GetComponent<PuzzleCell>();
                int answer = solution[y, x];

                cellComp.correctValue = answer;  // 정답 값 설정

                if (mask[y, x])
                {
                    // 고정된 힌트 셀
                    cellComp.isFixed = true;
                    cellComp.cellText.text = answer.ToString();
                    cellComp.cellText.fontStyle = FontStyles.Bold;
                    cellComp.cellText.color = Color.black;
                }
                else
                {
                    // 빈 칸
                    cellComp.isFixed = false;
                    cellComp.cellText.text = string.Empty;
                    cellComp.cellText.fontStyle = FontStyles.Normal;
                }
            }
        }


        //정답 배열에 복사
        for (int y = 0; y < gridSize; y++)
            for (int x = 0; x < gridSize; x++)
                corrects[y, x] = solution[y, x];

    }

    //정답 배열 반환 (GameManager에서 저장용으로 사용)
    public int[,] GetCorrectValues()
    {
        return corrects;
    }

    /// <summary>
    /// 기존 호출 호환을 위한 메서드 오버로드
    /// </summary>
    public void Generate(Transform parent, int gridSize)
    {
        // 저장된 GameManager 난이도를 사용
        Generate(parent, gridSize, GameManager.Instance.difficulty);
    }

    /// <summary>
    /// 퍼즐 보드 크기에 맞춰 셀 크기 조정
    /// </summary>
    private void ResizeCells()
    {
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
    /// 백트래킹 방식으로 완전 해답 보드 생성
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

        // 1~9를 랜덤 순서로 시도
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
    /// 특정 위치에 숫자를 놓을 수 있는지 검사
    /// </summary>
    private bool CanPlace(int[,] board, int row, int col, int num)
    {
        // 같은 행과 열 체크
        for (int i = 0; i < GridSize; i++)
            if (board[row, i] == num || board[i, col] == num)
                return false;

        // 3x3 박스 체크
        int boxR = (row / 3) * 3;
        int boxC = (col / 3) * 3;
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                if (board[boxR + r, boxC + c] == num)
                    return false;
        return true;
    }

    /// <summary>
    /// 주어진 난이도에 맞게 힌트 마스크(고정 셀) 생성, 유일 해답 보장
    /// </summary>
    private bool[,] GenerateClueMask(int[,] solution, Difficulty diff)
    {
        int n = GridSize;
        int clues = diff == Difficulty.Easy ? 40 :
                    diff == Difficulty.Normal ? 30 : 20;

        // 1) 해답 복사
        int[,] puzzle = solution.Clone() as int[,];

        // 2) 셀 좌표 리스트를 랜덤 섞기
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

            // 유일 해답인지 확인
            if (CountSolutions(puzzle, 2) != 1)
                puzzle[r, c] = backup;
            else
            {
                removed++;
                if (n * n - removed <= clues) break;
            }
        }

        // 3) 마스크 생성 (true = 고정 셀)
        bool[,] mask = new bool[n, n];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                mask[r, c] = puzzle[r, c] != 0;
        return mask;
    }

    /// <summary>
    /// 제한 횟수까지 해답 개수 세기
    /// </summary>
    private int CountSolutions(int[,] board, int limit)
    {
        int count = 0;
        SolveCount(board, 0, 0, ref count, limit);
        return count;
    }

    private void SolveCount(int[,] board, int row, int col, ref int count, int limit)
    {
        if (count >= limit) return;    // limit 이상이면 종료
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
