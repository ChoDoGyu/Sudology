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
        continueButton.onClick.AddListener(OnContinue);
    }

    void Start()
    {
        bool hasSave = SaveManager.Instance.HasSave();

        // Continue ��ư�� Ȱ��/��Ȱ��
        continueButton.interactable = hasSave;

        // �г� ��ü �帮��/Ŭ�� ���� �� otherButtonsGroup ��� ���� Ȱ��ȭ
        foreach (Transform button in otherButtonsGroup.transform)
        {
            var btn = button.GetComponent<Button>();

            // Continue ��ư�� hasSave�� ���� Ȱ��ȭ, �������� �׻� Ȱ��ȭ
            if (btn == continueButton)
            {
                btn.interactable = hasSave;
            }
            else
            {
                btn.interactable = true;
            }
        }

        // �ð��� ȿ�� ���� (�������� �״�� �����ϰ� Ŭ���� �����ϰ� ��)
        otherButtonsGroup.alpha = 1f;
        otherButtonsGroup.interactable = true;
        otherButtonsGroup.blocksRaycasts = true;
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

    private void OnContinue()
    {
        GameManager.Instance.StartContinueGame(); //����� ���� �ҷ�����
    }
}