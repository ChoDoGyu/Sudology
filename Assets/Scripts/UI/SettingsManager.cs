using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 설정 패널의 토글(BGM/SFX)과 AudioManager 상태를 동기화하는 매니저
/// 패널이 열릴 때마다 토글 UI를 AudioManager 실제 상태로 맞춘다.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private GameObject panel;

    private void Start()
    {
        // 최초에도 AudioManager 상태로 UI 동기화
        SyncTogglesWithAudio();
        // 토글 이벤트 연결
        bgmToggle.onValueChanged.AddListener(OnBGMToggleChanged);
        sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
    }

    /// <summary>
    /// 설정 패널 열기/닫기, 열 때마다 AudioManager와 동기화
    /// </summary>
    public void TogglePanel()
    {
        // 패널 열기 직전마다 AudioManager 상태로 토글 UI 맞춤
        if (!panel.activeSelf)
            SyncTogglesWithAudio();
        panel.SetActive(!panel.activeSelf);
    }

    /// <summary>
    /// AudioManager의 mute 상태와 토글 UI를 동기화
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