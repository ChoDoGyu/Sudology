using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �г��� ���(BGM/SFX)�� AudioManager ���¸� ����ȭ�ϴ� �Ŵ���
/// �г��� ���� ������ ��� UI�� AudioManager ���� ���·� �����.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private GameObject panel;

    private void Start()
    {
        // ���ʿ��� AudioManager ���·� UI ����ȭ
        SyncTogglesWithAudio();
        // ��� �̺�Ʈ ����
        bgmToggle.onValueChanged.AddListener(OnBGMToggleChanged);
        sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
    }

    /// <summary>
    /// ���� �г� ����/�ݱ�, �� ������ AudioManager�� ����ȭ
    /// </summary>
    public void TogglePanel()
    {
        // �г� ���� �������� AudioManager ���·� ��� UI ����
        if (!panel.activeSelf)
            SyncTogglesWithAudio();
        panel.SetActive(!panel.activeSelf);
    }

    /// <summary>
    /// AudioManager�� mute ���¿� ��� UI�� ����ȭ
    /// </summary>
    private void SyncTogglesWithAudio()
    {
        if (AudioManager.Instance != null)
        {
            bgmToggle.isOn = !(AudioManager.Instance.bgmSource != null && AudioManager.Instance.bgmSource.mute);
            sfxToggle.isOn = !(AudioManager.Instance.sfxSource != null && AudioManager.Instance.sfxSource.mute);
        }
    }
    private void OnBGMToggleChanged(bool on)
    {
        if (AudioManager.Instance?.bgmSource != null)
            AudioManager.Instance.bgmSource.mute = !on;
    }

    private void OnSFXToggleChanged(bool on)
    {
        if (AudioManager.Instance?.sfxSource != null)
            AudioManager.Instance.sfxSource.mute = !on;
    }
}