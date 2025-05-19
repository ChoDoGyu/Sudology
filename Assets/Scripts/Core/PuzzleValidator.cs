using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleValidator : MonoBehaviour
{
    public static PuzzleValidator Instance;

    private void Awake()
    {
        Instance = this;
    }

    // ���� ���� ���� �˻�
    public void CheckIfPuzzleCompleted()
    {
        var cells = FindObjectsOfType<PuzzleCell>();

        foreach (var cell in cells)
        {
            if (string.IsNullOrEmpty(cell.cellText.text)) return;
            if (!int.TryParse(cell.cellText.text, out int input)) return;
            if (input != cell.correctValue) return;
        }

        // ���� ������ ���
        if (!isCompleted)
        {
            isCompleted = true;
            GameManager.Instance.CompleteGame();
        }
    }

    private bool isCompleted = false;  // �ߺ� ���� ����
}
