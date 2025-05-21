using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

/// <summary>
/// ���� �Է� �� Undo/Redo, �� ����, ��Ʈ �Է��� �����ϴ� �Ŵ���
/// </summary>

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("�� ����")]
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 0.6f);  // ���õ� ���� ��
    [SerializeField] private Color normalColor = Color.white;  // �⺻ �� ��

    private PuzzleCell selectedCell;

    // �Է� ���� ��� (Undo/Redo)
    private Stack<CellSnapshot> undoStack = new Stack<CellSnapshot>();
    private Stack<CellSnapshot> redoStack = new Stack<CellSnapshot>();

    /// <summary>
    /// �� ����(��, isFixed ��) ����� ������
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
    /// �� ���� �� ���� ����
    /// </summary>
    public void SelectCell(PuzzleCell cell)
    {
        if (selectedCell != null)
            selectedCell.SetBackgroundColor(normalColor);

        selectedCell = cell;
        selectedCell.SetBackgroundColor(selectedColor);
    }

    /// <summary>
    /// ���� ��ư Ŭ�� �� ȣ��
    /// </summary>
    public void OnNumberButtonPressed(int number)
    {
        if (selectedCell == null || selectedCell.isFixed)
            return;

        // Undo ���ÿ� ���� ���� ���
        PushUndoSnapshot(selectedCell);

        // ���� �Է�
        selectedCell.cellText.text = number.ToString();

        // Redo ���� �ʱ�ȭ
        redoStack.Clear();

        // ���� ����
        SaveCurrentState();

        // ���� �Ϸ� �˻�
        StartCoroutine(DelayedCheck());
    }

    /// <summary>
    /// 1������ �� ���� �Ϸ� �˻�
    /// </summary>
    private IEnumerator DelayedCheck()
    {
        yield return null;
        PuzzleValidator.Instance?.CheckIfPuzzleCompleted();
    }

    /// <summary>
    /// Undo ����
    /// </summary>
    public void Undo()
    {
        if (undoStack.Count == 0) return;
        var prev = undoStack.Pop();

        // Redo ���ÿ� ���� ���� ���
        PushRedoSnapshot(prev.cell);

        // ���� ���� ����
        prev.cell.cellText.text = prev.value;
        prev.cell.isFixed = prev.isFixed;

        SaveCurrentState();
    }


    /// <summary>
    /// Redo ����
    /// </summary>
    public void Redo()
    {
        if (redoStack.Count == 0) return;
        var next = redoStack.Pop();

        // Undo ���ÿ� ���� ���� ���
        PushUndoSnapshot(next.cell);

        // ���� ����
        next.cell.cellText.text = next.value;
        next.cell.isFixed = next.isFixed;

        SaveCurrentState();
    }

    /// <summary>
    /// ��Ʈ ���: ���õ� ���� ���� �Է� �� ����
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
    /// ���� �� ���¸� Undo ���ÿ� ���
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
    /// ���� �� ���¸� Redo ���ÿ� ���
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
    /// ���� ���¸� SaveManager�� ����
    /// </summary>
    public void SaveCurrentState()
    {
        var board = GameObject.Find("PuzzleBoard");
        var cells = board.GetComponentsInChildren<PuzzleCell>();

        // �̸��� ����
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
