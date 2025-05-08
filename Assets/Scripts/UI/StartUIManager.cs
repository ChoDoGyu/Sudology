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
        // Continue 버튼 활성/비활성
        continueButton.interactable = hasSave;
        // 패널 전체 흐리기·클릭 차단
        otherButtonsGroup.alpha = hasSave ? 1f : 0.5f;
        otherButtonsGroup.interactable = hasSave;
        otherButtonsGroup.blocksRaycasts = hasSave;
    }

    private void OnNewGame()
    {
        // 난이도 선택 패널 열기
        difficultyPanel.SetActive(true);
        otherButtonsGroup.interactable = false;  // 뒤 버튼 잠금
    }

    private void OnCancel()
    {
        // 난이도 선택 패널 닫기
        difficultyPanel.SetActive(false);
        otherButtonsGroup.interactable = true;   // 뒤 버튼 복원
    }
}
