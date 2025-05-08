using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public Color selectedColor = new Color(1f, 1f, 0.6f);  // 선택된 셀의 색
    public Color normalColor = Color.white;  // 기본 셀 색

    private PuzzleCell selectedCell;
    private Stack<Action> undoStack = new Stack<Action>();   // Undo 스택
    private Stack<Action> redoStack = new Stack<Action>();   // Redo 스택

    // Undo/Redo 구조
    private struct Action
    {
        public PuzzleCell cell;
        public string previousValue;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 셀 선택 메서드
    public void SelectCell(PuzzleCell cell)
    {
        if (selectedCell != null)
            selectedCell.SetBackgroundColor(normalColor);

        selectedCell = cell;
        selectedCell.SetBackgroundColor(selectedColor);
    }

    // 숫자 버튼 클릭 시 호출되는 메서드
    public void OnNumberButtonPressed(int number)
    {
        if (selectedCell == null || selectedCell.isFixed)
            return;

        // 이전 값 기록
        var prev = selectedCell.cellText.text;
        var act = new Action { cell = selectedCell, previousValue = prev };

        // 숫자 입력
        selectedCell.cellText.text = number.ToString();

        // Undo 스택에 추가 (최대 3개)
        undoStack.Push(act);
        if (undoStack.Count > 3)
            undoStack = new Stack<Action>(undoStack.Take(3).Reverse());

        // Redo 초기화
        redoStack.Clear();

        // 상태 저장
        SaveCurrentState();

        Debug.Log($"Cell {selectedCell.name}에 {number} 입력");
    }

    // Undo 기능
    public void Undo()
    {
        if (undoStack.Count == 0)
            return;

        var act = undoStack.Pop();
        // 현재 값을 Redo 스택에 저장
        redoStack.Push(new Action { cell = act.cell, previousValue = act.cell.cellText.text });

        // 되돌리기
        act.cell.cellText.text = act.previousValue;

        SaveCurrentState();
        Debug.Log($"Undo: {act.cell.name} → {act.previousValue}");
    }

    // Redo 기능
    public void Redo()
    {
        if (redoStack.Count == 0)
            return;

        var act = redoStack.Pop();
        // 현재 값을 Undo 스택에 저장
        undoStack.Push(new Action { cell = act.cell, previousValue = act.cell.cellText.text });

        // 복원
        act.cell.cellText.text = act.previousValue;

        SaveCurrentState();
        Debug.Log($"Redo: {act.cell.name} → {act.previousValue}");
    }

    // 힌트 제공
    public void ProvideHint()
    {
        if (selectedCell == null || selectedCell.isFixed)
            return;

        selectedCell.cellText.text = selectedCell.correctValue.ToString();
        selectedCell.isFixed = true;

        SaveCurrentState();
        Debug.Log("힌트 제공");
    }

    // 현재 보드 상태를 SaveManager에 저장
    private void SaveCurrentState()
    {
        var cells = FindObjectsOfType<PuzzleCell>();
        int[,] values = new int[9, 9];
        bool[,] fixeds = new bool[9, 9];

        for (int i = 0; i < cells.Length; i++)
        {
            int r = i / 9, c = i % 9;
            values[r, c] =
                string.IsNullOrEmpty(cells[i].cellText.text)
                    ? 0
                    : int.Parse(cells[i].cellText.text);
            fixeds[r, c] = cells[i].isFixed;
        }

        SaveManager.Instance.SaveState(values, fixeds);
    }
}
