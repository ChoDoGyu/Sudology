using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public Color selectedColor = new Color(1f, 1f, 0.6f);  // ���õ� ���� ��
    public Color normalColor = Color.white;  // �⺻ �� ��

    private PuzzleCell selectedCell;
    private Stack<Action> undoStack = new Stack<Action>();   // Undo ����
    private Stack<Action> redoStack = new Stack<Action>();   // Redo ����

    // Undo/Redo ����
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

    // �� ���� �޼���
    public void SelectCell(PuzzleCell cell)
    {
        if (selectedCell != null)
            selectedCell.SetBackgroundColor(normalColor);

        selectedCell = cell;
        selectedCell.SetBackgroundColor(selectedColor);
    }

    // ���� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void OnNumberButtonPressed(int number)
    {
        if (selectedCell == null || selectedCell.isFixed)
            return;

        // ���� �� ���
        var prev = selectedCell.cellText.text;
        var act = new Action { cell = selectedCell, previousValue = prev };

        // ���� �Է�
        selectedCell.cellText.text = number.ToString();

        // Undo ���ÿ� �߰� (�ִ� 3��)
        undoStack.Push(act);
        if (undoStack.Count > 3)
            undoStack = new Stack<Action>(undoStack.Take(3).Reverse());

        // Redo �ʱ�ȭ
        redoStack.Clear();

        // ���� ����
        SaveCurrentState();

        Debug.Log($"Cell {selectedCell.name}�� {number} �Է�");
    }

    // Undo ���
    public void Undo()
    {
        if (undoStack.Count == 0)
            return;

        var act = undoStack.Pop();
        // ���� ���� Redo ���ÿ� ����
        redoStack.Push(new Action { cell = act.cell, previousValue = act.cell.cellText.text });

        // �ǵ�����
        act.cell.cellText.text = act.previousValue;

        SaveCurrentState();
        Debug.Log($"Undo: {act.cell.name} �� {act.previousValue}");
    }

    // Redo ���
    public void Redo()
    {
        if (redoStack.Count == 0)
            return;

        var act = redoStack.Pop();
        // ���� ���� Undo ���ÿ� ����
        undoStack.Push(new Action { cell = act.cell, previousValue = act.cell.cellText.text });

        // ����
        act.cell.cellText.text = act.previousValue;

        SaveCurrentState();
        Debug.Log($"Redo: {act.cell.name} �� {act.previousValue}");
    }

    // ��Ʈ ����
    public void ProvideHint()
    {
        if (selectedCell == null || selectedCell.isFixed)
            return;

        selectedCell.cellText.text = selectedCell.correctValue.ToString();
        selectedCell.isFixed = true;

        SaveCurrentState();
        Debug.Log("��Ʈ ����");
    }

    // ���� ���� ���¸� SaveManager�� ����
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
