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

    // 퍼즐 정답 여부 검사
    public void CheckIfPuzzleCompleted()
    {
        var cells = FindObjectsOfType<PuzzleCell>();

        foreach (var cell in cells)
        {
            if (string.IsNullOrEmpty(cell.cellText.text)) return;
            if (!int.TryParse(cell.cellText.text, out int input)) return;
            if (input != cell.correctValue) return;
        }

        // 전부 정답일 경우
        if (!isCompleted)
        {
            isCompleted = true;
            GameManager.Instance.CompleteGame();
        }
    }

    private bool isCompleted = false;  // 중복 실행 방지
}
