using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleGenerator : MonoBehaviour, IPuzzleGenerator
{
    [Header("References")]
    public GameObject puzzleCellPrefab;  // PuzzleCell 프리팹
    public RectTransform boardRect;      // PuzzleBoard RectTransform
    public GridLayoutGroup gridLayout;   // GridLayoutGroup 컴포넌트 참조

    private const int GridSize = 9; // 9x9 퍼즐


    /// <summary>
    /// PuzzleBoard 크기에 맞춰 GridLayoutGroup.cellSize를 동적으로 계산합니다.
    /// </summary>
    private void ResizeCells()
    {
        // 보드의 실제 너비와 높이
        float boardWidth = boardRect.rect.width;
        float boardHeight = boardRect.rect.height;

        // GridLayoutGroup의 스페이싱
        float spacingX = gridLayout.spacing.x;
        float spacingY = gridLayout.spacing.y;

        // 셀 크기를 가로, 세로 중 작은 쪽 기준으로 계산
        float cellSizeX = (boardWidth - spacingX * (GridSize - 1)) / GridSize;
        float cellSizeY = (boardHeight - spacingY * (GridSize - 1)) / GridSize;
        float cellSize = Mathf.Min(cellSizeX, cellSizeY);

        gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }

    /// <summary>
    /// PuzzleCell 프리팹을 9×9로 Instantiate 하고,
    /// 문제 셀(isFixed)에는 미리 숫자를 채워 고정 처리합니다.
    /// </summary>
    private void GeneratePuzzleBoard()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                GameObject newCell = Instantiate(puzzleCellPrefab, gridLayout.transform);
                newCell.name = $"PuzzleCell_{x}_{y}";

                PuzzleCell cellComp = newCell.GetComponent<PuzzleCell>();
                TextMeshProUGUI cellText = cellComp.cellText;

                // 예시: 첫 행(y==0)의 앞 3칸(x<3)을 문제 셀로 고정
                if (y == 0 && x < 3)
                {
                    cellComp.isFixed = true;
                    int presetNumber = Random.Range(1, 10);
                    cellText.text = presetNumber.ToString();
                    cellText.color = Color.black;

                    var bgImage = newCell.transform
                                      .Find("Background")
                                      .GetComponent<Image>();
                    bgImage.color = new Color(0.9f, 0.9f, 0.9f);
                }
                else
                {
                    cellComp.isFixed = false;
                    cellText.text = "";
                }
            }
        }
    }
    public void Generate(Transform parent, int gridSize)
    {
        ResizeCells(); // 셀 크기 조정
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                GameObject newCell = Instantiate(puzzleCellPrefab, gridLayout.transform);
                newCell.name = $"PuzzleCell_{x}_{y}";

                PuzzleCell cellComp = newCell.GetComponent<PuzzleCell>();
                TextMeshProUGUI cellText = cellComp.cellText;

                // 예시: 첫 행(y==0)의 앞 3칸(x<3)을 문제 셀로 고정
                if (y == 0 && x < 3)
                {
                    cellComp.isFixed = true;
                    int presetNumber = Random.Range(1, 10);
                    cellText.text = presetNumber.ToString();
                    cellText.color = Color.black;

                    var bgImage = newCell.transform
                                      .Find("Background")
                                      .GetComponent<Image>();
                    bgImage.color = new Color(0.9f, 0.9f, 0.9f);
                }
                else
                {
                    cellComp.isFixed = false;
                    cellText.text = "";
                }
            }
        }
    }

}
