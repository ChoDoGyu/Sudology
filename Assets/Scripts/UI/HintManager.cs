using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HintManager : MonoBehaviour
{
    public int maxHintsPerPuzzle = 3;
    private int hintsUsed = 0;

    // ȣ��: HintButton.OnClick()
    public void ProvideHint()
    {
        if (hintsUsed >= maxHintsPerPuzzle)
        {
            Debug.Log("��Ʈ Ƚ�� ����");
            return;
        }

        // ���� ä������ ���� �� ĭ �� �ϳ� ����
        var emptyCells = FindObjectsOfType<PuzzleCell>()
            .Where(c => !c.isFixed && string.IsNullOrEmpty(c.cellText.text))
            .ToList();
        if (emptyCells.Count == 0)
        {
            Debug.Log("���� �� ĭ ����");
            return;
        }

        // �������� �ϳ� �̾� �̸� ���� ���� ä����
        var cell = emptyCells[Random.Range(0, emptyCells.Count)];
        cell.cellText.text = cell.correctValue.ToString();
        cell.isFixed = true;         // �� �̻� ���� �Ұ�
        hintsUsed++;

        GameManager.Instance.currentHintCount++;

        Debug.Log($"��Ʈ ��� {hintsUsed}/{maxHintsPerPuzzle}");
    }
}
