using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 힌트 관리 싱글톤 매니저
/// (씬 전환에도 살아있어야 하므로 DontDestroyOnLoad 적용)
/// </summary>
public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    [SerializeField] private int maxHintsPerPuzzle = 3;
    private int hintsUsed = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    /// <summary>
    /// 힌트 사용 가능 여부
    /// </summary>
    public bool CanUseHint()
    {
        return hintsUsed < maxHintsPerPuzzle;
    }

    /// <summary>
    /// 힌트 사용 시 호출
    /// </summary>
    public void ProvideHint()
    {
        if (!CanUseHint())
        {
            //Debug.Log("힌트 횟수 소진");
            return;
        }

        // 퍼즐판에서 빈 칸 중 하나에 정답 제공 (로직 예시)
        var board = GameObject.Find("PuzzleBoard");
        if (board != null)
        {
            var emptyCells = board.GetComponentsInChildren<PuzzleCell>();
            foreach (var cell in emptyCells)
            {
                if (!cell.isFixed && string.IsNullOrEmpty(cell.cellText.text))
                {
                    cell.cellText.text = cell.correctValue.ToString();
                    cell.isFixed = true;
                    hintsUsed++;
                    //Debug.Log($"힌트 사용: {hintsUsed}/{maxHintsPerPuzzle}");
                    break;
                }
            }
        }
    }

    public void ResetHintCount()
    {
        hintsUsed = 0;
    }

    public int GetHintCount()
    {
        return hintsUsed;
    }

    public void SetHintCount(int count)
    {
        hintsUsed = count;
    }
}
