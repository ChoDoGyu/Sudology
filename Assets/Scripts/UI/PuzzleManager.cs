using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    // 9��9 ���� ��ü�� ����
    public void CheckPuzzle()
    {
        foreach (var cell in FindObjectsOfType<PuzzleCell>())
        {
            if (cell.isFixed)
            {
                cell.ResetColor();
                continue;
            }

            if (int.TryParse(cell.cellText.text, out int input) &&
                input != cell.correctValue)
                cell.HighlightError();
            else
                cell.ResetColor();
        }
    }
}
