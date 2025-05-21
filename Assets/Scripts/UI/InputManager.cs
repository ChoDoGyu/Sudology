using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

/// <summary>
/// 퍼즐 입력 및 Undo/Redo, 셀 선택, 힌트 입력을 관리하는 매니저
/// </summary>

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("셀 색상")]
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 0.6f);  // 선택된 셀의 색
    [SerializeField] private Color normalColor = Color.white;  // 기본 셀 색

    private PuzzleCell selectedCell;

    // 입력 변경 기록 (Undo/Redo)
    private Stack<CellSnapshot> undoStack = new Stack<CellSnapshot>();
    private Stack<CellSnapshot> redoStack = new Stack<CellSnapshot>();

    /// <summary>
    /// 셀 상태(값, isFixed 등) 저장용 스냅샷
    /// </summary>
    private struct CellSnapshot
    {
        public PuzzleCell cell;
        public string value;
        public bool isFixed;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 셀 선택 시 색상 변경
    /// </summary>
    public void SelectCell(PuzzleCell cell)
    {
        if (selectedCell != null)
            selectedCell.SetBackgroundColor(normalColor);

        selectedCell = cell;
        selectedCell.SetBackgroundColor(selectedColor);
    }

    /// <summary>
    /// 숫자 버튼 클릭 시 호출
    /// </summary>
    public void OnNumberButtonPressed(int number)
    {
        if (selectedCell == null || selectedCell.isFixed)
            return;

        // Undo 스택에 현재 상태 기록
        PushUndoSnapshot(selectedCell);

        // 숫자 입력
        selectedCell.cellText.text = number.ToString();

        // Redo 스택 초기화
        redoStack.Clear();

        // 상태 저장
        SaveCurrentState();

        // 퍼즐 완료 검사
        StartCoroutine(DelayedCheck());
    }

    /// <summary>
    /// 1프레임 후 퍼즐 완료 검사
    /// </summary>
    private IEnumerator DelayedCheck()
    {
        yield return null;
        PuzzleValidator.Instance?.CheckIfPuzzleCompleted();
    }

    /// <summary>
    /// Undo 실행
    /// </summary>
    public void Undo()
    {
        if (undoStack.Count == 0) return;
        var prev = undoStack.Pop();

        // Redo 스택에 현재 상태 기록
        PushRedoSnapshot(prev.cell);

        // 이전 상태 복원
        prev.cell.cellText.text = prev.value;
        prev.cell.isFixed = prev.isFixed;

        SaveCurrentState();
    }


    /// <summary>
    /// Redo 실행
    /// </summary>
    public void Redo()
    {
        if (redoStack.Count == 0) return;
        var next = redoStack.Pop();

        // Undo 스택에 현재 상태 기록
        PushUndoSnapshot(next.cell);

        // 상태 복원
        next.cell.cellText.text = next.value;
        next.cell.isFixed = next.isFixed;

        SaveCurrentState();
    }

    /// <summary>
    /// 힌트 사용: 선택된 셀에 정답 입력 후 고정
    /// </summary>
    public void ProvideHint()
    {
        if (selectedCell == null || selectedCell.isFixed)
            return;

        PushUndoSnapshot(selectedCell);

        selectedCell.cellText.text = selectedCell.correctValue.ToString();
        selectedCell.isFixed = true;

        redoStack.Clear();
        SaveCurrentState();
    }

    /// <summary>
    /// 현재 셀 상태를 Undo 스택에 기록
    /// </summary>
    private void PushUndoSnapshot(PuzzleCell cell)
    {
        undoStack.Push(new CellSnapshot
        {
            cell = cell,
            value = cell.cellText.text,
            isFixed = cell.isFixed
        });
    }

    /// <summary>
    /// 현재 셀 상태를 Redo 스택에 기록
    /// </summary>
    private void PushRedoSnapshot(PuzzleCell cell)
    {
        redoStack.Push(new CellSnapshot
        {
            cell = cell,
            value = cell.cellText.text,
            isFixed = cell.isFixed
        });
    }

    /// <summary>
    /// 퍼즐 상태를 SaveManager에 저장
    /// </summary>
    public void SaveCurrentState()
    {
        var board = GameObject.Find("PuzzleBoard");
        var cells = board.GetComponentsInChildren<PuzzleCell>();

        // 이름순 정렬
        cells = cells.OrderBy(cell => cell.name).ToArray();

        int[,] values = new int[9, 9];
        bool[,] fixeds = new bool[9, 9];
        int[,] corrects = new int[9, 9];

        for (int i = 0; i < cells.Length; i++)
        {
            int r = i / 9, c = i % 9;
            values[r, c] = string.IsNullOrEmpty(cells[i].cellText.text) ? 0 : int.Parse(cells[i].cellText.text);
            fixeds[r, c] = cells[i].isFixed;
            corrects[r, c] = cells[i].correctValue;
        }

        int hintCount = HintManager.Instance != null ? HintManager.Instance.GetHintCount() : 0;
        float elapsedTime = GameManager.Instance != null && GameManager.Instance.gameTimer != null
            ? GameManager.Instance.gameTimer.GetElapsedTime() : 0f;

        SaveManager.Instance.SaveState(values, fixeds, corrects, hintCount, elapsedTime);
    }
}
