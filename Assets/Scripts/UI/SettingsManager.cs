using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    // Settings ��ư Ŭ�� �� ȣ��
    public void TogglePanel()
    {
        // SettingsPanel�� Ȱ��ȭ/��Ȱ��ȭ
        bool isActive = panel.activeSelf;
        panel.SetActive(!isActive);

        if (panel.activeSelf)
        {
            // �г��� Ȱ��ȭ�Ǹ� UI ����� AudioManager���� �����ϵ��� ����
            AudioManager.Instance.ConnectToggles();
        }
    }

    // UI ��� �̺�Ʈ�� ����
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