using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HintManager : MonoBehaviour
{
    public int maxHintsPerPuzzle = 3;
    private int hintsUsed = 0;

    // 호출: HintButton.OnClick()
    public void ProvideHint()
    {
        if (hintsUsed >= maxHintsPerPuzzle)
        {
            Debug.Log("힌트 횟수 소진");
            return;
        }

        // 아직 채워지지 않은 빈 칸 중 하나 선택
        var emptyCells = FindObjectsOfType<PuzzleCell>()
            .Where(c => !c.isFixed && string.IsNullOrEmpty(c.cellText.text))
            .ToList();
        if (emptyCells.Count == 0)
        {
            Debug.Log("남은 빈 칸 없음");
            return;
        }

        // 랜덤으로 하나 뽑아 미리 계산된 정답 채워줌
        var cell = emptyCells[Random.Range(0, emptyCells.Count)];
        cell.cellText.text = cell.correctValue.ToString();
        cell.isFixed = true;         // 더 이상 수정 불가
        hintsUsed++;

        GameManager.Instance.currentHintCount++;

        Debug.Log($"힌트 사용 {hintsUsed}/{maxHintsPerPuzzle}");
    }
}
