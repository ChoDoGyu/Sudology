using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PuzzleCell : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool isFixed = false;      // ���� �� �÷���
    public TextMeshProUGUI cellText;                   // ���� ǥ�ÿ� �ؽ�Ʈ

    private void Awake()
    {
        // Prefab���� cellText �ڵ� ����
        if (cellText == null)
            cellText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // ����ڰ� ���� Ŭ������ �� ȣ��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFixed)
        {
            Debug.Log("�� ���� ���� ���̶� ���� �Ұ�");
            return;  // ������ ���� ����
        }
        // TODO: ���� �Է� UI ����, ���� ó�� ��
        Debug.Log(name + " �� Ŭ����");
    }

}
