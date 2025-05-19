using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    // Settings 버튼 클릭 시 호출
    public void TogglePanel()
    {
        // SettingsPanel을 활성화/비활성화
        bool isActive = panel.activeSelf;
        panel.SetActive(!isActive);

        if (panel.activeSelf)
        {
            // 패널이 활성화되면 UI 토글을 AudioManager에서 연결하도록 유도
            AudioManager.Instance.ConnectToggles();
        }
    }

    // UI 토글 이벤트를 설정
    public void SetupToggleEvents()
    {
        Toggle bgmToggle = GameObject.FindGameObjectWithTag("BGMToggle").GetComponent<Toggle>();
        Toggle sfxToggle = GameObject.FindGameObjectWithTag("SFXToggle").GetComponent<Toggle>();

        if (bgmToggle != null && sfxToggle != null)
        {
            bgmToggle.onValueChanged.AddListener((bool isOn) =>
            {
                AudioManager.Instance.SetBGMMute(!isOn);
            });

            sfxToggle.onValueChanged.AddListener((bool isOn) =>
            {
                AudioManager.Instance.SetSFXMute(!isOn);
            });
        }
    }
}