using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PuzzleCell : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool isFixed = false;      // 문제 셀 플래그
    public TextMeshProUGUI cellText;                   // 숫자 표시용 텍스트

    private void Awake()
    {
        // Prefab에서 cellText 자동 연결
        if (cellText == null)
            cellText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // 사용자가 셀을 클릭했을 때 호출
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFixed)
        {
            Debug.Log("이 셀은 문제 셀이라 수정 불가");
            return;  // 고정된 셀은 무시
        }
        // TODO: 숫자 입력 UI 열기, 선택 처리 등
        Debug.Log(name + " 셀 클릭됨");
    }

}
