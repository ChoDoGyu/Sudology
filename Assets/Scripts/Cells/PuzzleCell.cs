using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class PuzzleCell : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool isFixed = false;      // 문제 셀 플래그
    public TextMeshProUGUI cellText;                   // 숫자 표시용 텍스트

    [HideInInspector] public int correctValue;    // 정답 값 저장
    private Image bgImage;                        // 배경 Image 캐시
    private Color normalColor, errorColor;

    private void Awake()
    {
        // Prefab에서 cellText 자동 연결
        if (cellText == null)
            cellText = GetComponentInChildren<TextMeshProUGUI>();

        var background = transform.Find("Background").GetComponent<Image>();
        bgImage = background;
        normalColor = background.color;
        errorColor = new Color(1f, 0.6f, 0.6f); // 연한 빨강
    }

    //오답 표시
    public void HighlightError()
    {
        bgImage.color = errorColor;
    }

    //정상 색으로 복원
    public void ResetColor()
    {
        bgImage.color = normalColor;
    }

    // 사용자가 셀을 클릭했을 때 호출
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFixed)
        {
            Debug.Log("이 셀은 문제 셀이라 수정 불가");
            return;
        }
        InputManager.Instance.SelectCell(this);
        Debug.Log($"{name} 셀 선택됨");
    }

    public void SetBackgroundColor(Color c)
    {
        var img = transform.Find("Background").GetComponent<UnityEngine.UI.Image>();
        img.color = c;
    }
}
