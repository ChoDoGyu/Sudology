using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private CanvasGroup otherButtonsGroup;

    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button cancelButton;

    void Awake()
    {
        newGameButton.onClick.AddListener(OnNewGame);
        cancelButton.onClick.AddListener(OnCancel);
    }

    void Start()
    {
        bool hasSave = SaveManager.Instance.HasSave();
        // Continue ��ư Ȱ��/��Ȱ��
        continueButton.interactable = hasSave;
        // �г� ��ü �帮�⡤Ŭ�� ����
        otherButtonsGroup.alpha = hasSave ? 1f : 0.5f;
        otherButtonsGroup.interactable = hasSave;
        otherButtonsGroup.blocksRaycasts = hasSave;
    }

    private void OnNewGame()
    {
        // ���̵� ���� �г� ����
        difficultyPanel.SetActive(true);
        otherButtonsGroup.interactable = false;  // �� ��ư ���
    }

    private void OnCancel()
    {
        // ���̵� ���� �г� �ݱ�
        difficultyPanel.SetActive(false);
        otherButtonsGroup.interactable = true;   // �� ��ư ����
    }
}
