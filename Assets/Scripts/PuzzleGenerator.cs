using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleGenerator : MonoBehaviour, IPuzzleGenerator
{
    [Header("References")]
    public GameObject puzzleCellPrefab;  // PuzzleCell ������
    public RectTransform boardRect;      // PuzzleBoard RectTransform
    public GridLayoutGroup gridLayout;   // GridLayoutGroup ������Ʈ ����

    private const int GridSize = 9; // 9x9 ����


    /// <summary>
    /// PuzzleBoard ũ�⿡ ���� GridLayoutGroup.cellSize�� �������� ����մϴ�.
    /// </summary>
    private void ResizeCells()
    {
        // ������ ���� �ʺ�� ����
        float boardWidth = boardRect.rect.width;
        float boardHeight = boardRect.rect.height;

        // GridLayoutGroup�� �����̽�
        float spacingX = gridLayout.spacing.x;
        float spacingY = gridLayout.spacing.y;

        // �� ũ�⸦ ����, ���� �� ���� �� �������� ���
        float cellSizeX = (boardWidth - spacingX * (GridSize - 1)) / GridSize;
        float cellSizeY = (boardHeight - spacingY * (GridSize - 1)) / GridSize;
        float cellSize = Mathf.Min(cellSizeX, cellSizeY);

        gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }

    /// <summary>
    /// PuzzleCell �������� 9��9�� Instantiate �ϰ�,
    /// ���� ��(isFixed)���� �̸� ���ڸ� ä�� ���� ó���մϴ�.
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

                // ����: ù ��(y==0)�� �� 3ĭ(x<3)�� ���� ���� ����
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
        ResizeCells(); // �� ũ�� ����
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                GameObject newCell = Instantiate(puzzleCellPrefab, gridLayout.transform);
                newCell.name = $"PuzzleCell_{x}_{y}";

                PuzzleCell cellComp = newCell.GetComponent<PuzzleCell>();
                TextMeshProUGUI cellText = cellComp.cellText;

                // ����: ù ��(y==0)�� �� 3ĭ(x<3)�� ���� ���� ����
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
