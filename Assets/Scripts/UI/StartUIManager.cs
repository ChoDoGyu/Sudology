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

        // Continue 버튼만 활성/비활성
        continueButton.interactable = hasSave;

        // 패널 전체 흐리기/클릭 차단 → otherButtonsGroup 대신 개별 활성화
        foreach (Transform button in otherButtonsGroup.transform)
        {
            var btn = button.GetComponent<Button>();

            // Continue 버튼만 hasSave에 따라 활성화, 나머지는 항상 활성화
            if (btn == continueButton)
            {
                btn.interactable = hasSave;
            }
            else
            {
                btn.interactable = true;
            }
        }

        // 시각적 효과 수정 (반투명도는 그대로 유지하고 클릭만 가능하게 함)
        otherButtonsGroup.alpha = 1f;
        otherButtonsGroup.interactable = true;
        otherButtonsGroup.blocksRaycasts = true;
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

    private void OnContinue()
    {
        GameManager.Instance.StartContinueGame(); //저장된 퍼즐 불러오기
    }
}