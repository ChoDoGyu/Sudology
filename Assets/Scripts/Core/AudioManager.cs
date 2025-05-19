using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private const string PREF_BGM = "IsBGMMuted";
    private const string PREF_SFX = "IsSFXMuted";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;  // 반복 재생할 BGM용
    [SerializeField] private AudioSource sfxSource;  // 효과음용

    [Header("UI Toggles")]
    [SerializeField] private Toggle bgmToggle;  // BGM Toggle
    [SerializeField] private Toggle sfxToggle;  // SFX Toggle

    [Header("SFX Clips")]
    public AudioClip clickSound;  //버튼 클릭용 효과음

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM이 있다면 재생 시작
            if (bgmSource != null && bgmSource.clip != null)
            {
                bgmSource.Play();
            }

            // 씬이 로드될 때마다 UI 토글을 연결합니다.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 씬 내의 모든 버튼을 찾아서 클릭 사운드 연결
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => AudioManager.Instance?.PlayClickSound());
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // SettingsManager를 찾고, UI 토글 연결을 대기합니다.
        SettingsManager settingsManager = FindObjectOfType<SettingsManager>();

        if (settingsManager != null)
        {
            settingsManager.TogglePanel(); // SettingsPanel을 활성화
            settingsManager.SetupToggleEvents();  // UI 토글 연결
        }
    }

    // 효과음 재생
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null && !sfxSource.mute)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    //버튼 전용 효과음
    public void PlayClickSound()
    {
        PlaySFX(clickSound);
    }

    public void ConnectToggles()
    {
        // SettingsPanel이 활성화된 후 UI 토글을 연결
        GameObject bgmToggleObj = GameObject.FindGameObjectWithTag("BGMToggle");
        GameObject sfxToggleObj = GameObject.FindGameObjectWithTag("SFXToggle");

        if (bgmToggleObj != null && sfxToggleObj != null)
        {
            bgmToggle = bgmToggleObj.GetComponent<Toggle>();
            sfxToggle = sfxToggleObj.GetComponent<Toggle>();

            // 동적 연결
            bgmToggle.onValueChanged.AddListener(OnBGMToggleChanged);
            sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
        }
    }

    // BGM 음소거 토글
    public void SetBGMMute(bool isMuted)
    {
        if (bgmSource == null) return;
        bgmSource.mute = isMuted;

        if (!isMuted)
        {
            if (!bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
            else
            {
                bgmSource.UnPause();
            }
        }
        else
        {
            bgmSource.Pause();
        }
    }

    // SFX 음소거 토글
    public void SetSFXMute(bool isMuted)
    {
        if (sfxSource == null) return;
        sfxSource.mute = isMuted;

        if (!isMuted)
        {
            if (!sfxSource.isPlaying)
            {
                sfxSource.Play();
            }
            else
            {
                sfxSource.UnPause();
            }
        }
        else
        {
            sfxSource.Pause();
        }
    }

    private void OnBGMToggleChanged(bool isOn)
    {
        SetBGMMute(!isOn);  // 토글이 꺼지면 음소거, 켜지면 음소거 해제
    }

    private void OnSFXToggleChanged(bool isOn)
    {
        SetSFXMute(!isOn);  // 토글이 꺼지면 음소거, 켜지면 음소거 해제
    }


}

