using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ��Ʈ ���� �̱��� �Ŵ���
/// (�� ��ȯ���� ����־�� �ϹǷ� DontDestroyOnLoad ����)
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
            DontDestroyOnLoad(gameObject);  // �� ��ȯ���� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    /// <summary>
    /// ��Ʈ ��� ���� ����
    /// </summary>
    public bool CanUseHint()
    {
        return hintsUsed < maxHintsPerPuzzle;
    }

    /// <summary>
    /// ��Ʈ ��� �� ȣ��
    /// </summary>
    public void ProvideHint()
    {
        if (!CanUseHint())
        {
            //Debug.Log("��Ʈ Ƚ�� ����");
            return;
        }

        // �����ǿ��� �� ĭ �� �ϳ��� ���� ���� (���� ����)
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
                    //Debug.Log($"��Ʈ ���: {hintsUsed}/{maxHintsPerPuzzle}");
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
