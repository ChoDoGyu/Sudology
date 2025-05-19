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
    [SerializeField] private AudioSource bgmSource;  // �ݺ� ����� BGM��
    [SerializeField] private AudioSource sfxSource;  // ȿ������

    [Header("UI Toggles")]
    [SerializeField] private Toggle bgmToggle;  // BGM Toggle
    [SerializeField] private Toggle sfxToggle;  // SFX Toggle

    [Header("SFX Clips")]
    public AudioClip clickSound;  //��ư Ŭ���� ȿ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM�� �ִٸ� ��� ����
            if (bgmSource != null && bgmSource.clip != null)
            {
                bgmSource.Play();
            }

            // ���� �ε�� ������ UI ����� �����մϴ�.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // �� ���� ��� ��ư�� ã�Ƽ� Ŭ�� ���� ����
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => AudioManager.Instance?.PlayClickSound());
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // SettingsManager�� ã��, UI ��� ������ ����մϴ�.
        SettingsManager settingsManager = FindObjectOfType<SettingsManager>();

        if (settingsManager != null)
        {
            settingsManager.TogglePanel(); // SettingsPanel�� Ȱ��ȭ
            settingsManager.SetupToggleEvents();  // UI ��� ����
        }
    }

    // ȿ���� ���
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null && !sfxSource.mute)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    //��ư ���� ȿ����
    public void PlayClickSound()
    {
        PlaySFX(clickSound);
    }

    public void ConnectToggles()
    {
        // SettingsPanel�� Ȱ��ȭ�� �� UI ����� ����
        GameObject bgmToggleObj = GameObject.FindGameObjectWithTag("BGMToggle");
        GameObject sfxToggleObj = GameObject.FindGameObjectWithTag("SFXToggle");

        if (bgmToggleObj != null && sfxToggleObj != null)
        {
            bgmToggle = bgmToggleObj.GetComponent<Toggle>();
            sfxToggle = sfxToggleObj.GetComponent<Toggle>();

            // ���� ����
            bgmToggle.onValueChanged.AddListener(OnBGMToggleChanged);
            sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
        }
    }

    // BGM ���Ұ� ���
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

    // SFX ���Ұ� ���
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
        SetBGMMute(!isOn);  // ����� ������ ���Ұ�, ������ ���Ұ� ����
    }

    private void OnSFXToggleChanged(bool isOn)
    {
        SetSFXMute(!isOn);  // ����� ������ ���Ұ�, ������ ���Ұ� ����
    }


}

