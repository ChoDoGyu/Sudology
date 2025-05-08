using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class PuzzleCell : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool isFixed = false;      // ���� �� �÷���
    public TextMeshProUGUI cellText;                   // ���� ǥ�ÿ� �ؽ�Ʈ

    [HideInInspector] public int correctValue;    // ���� �� ����
    private Image bgImage;                        // ��� Image ĳ��
    private Color normalColor, errorColor;

    private void Awake()
    {
        // Prefab���� cellText �ڵ� ����
        if (cellText == null)
            cellText = GetComponentInChildren<TextMeshProUGUI>();

        var background = transform.Find("Background").GetComponent<Image>();
        bgImage = background;
        normalColor = background.color;
        errorColor = new Color(1f, 0.6f, 0.6f); // ���� ����
    }

    //���� ǥ��
    public void HighlightError()
    {
        bgImage.color = errorColor;
    }

    //���� ������ ����
    public void ResetColor()
    {
        bgImage.color = normalColor;
    }

    // ����ڰ� ���� Ŭ������ �� ȣ��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFixed)
        {
            Debug.Log("�� ���� ���� ���̶� ���� �Ұ�");
            return;
        }
        InputManager.Instance.SelectCell(this);
        Debug.Log($"{name} �� ���õ�");
    }

    public void SetBackgroundColor(Color c)
    {
        var img = transform.Find("Background").GetComponent<UnityEngine.UI.Image>();
        img.color = c;
    }
}
