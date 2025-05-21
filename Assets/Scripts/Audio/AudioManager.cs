using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 모든 오디오(효과음, BGM) 및 버튼 효과음 자동 등록을 관리하는 싱글톤 매니저
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources & Clips")]
    [SerializeField] public AudioSource sfxSource;
    [SerializeField] public AudioSource bgmSource;
    [SerializeField] private AudioClip buttonSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        RegisterButtonSounds();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterButtonSounds();
    }

    /// <summary>
    /// 씬 내 모든 Button에 클릭음 자동 등록
    /// UI가 여러 Panel/Group으로 분산되어 있고, 일괄 효과음 적용이 필요할 때 전체 검색 허용(성능 영향 없음)
    /// </summary>
    private void RegisterButtonSounds()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // 모든 버튼 검색 (비활성 포함)
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(OnButtonClickSound); // 중복 방지
            btn.onClick.AddListener(OnButtonClickSound);
        }
    }

    private void OnButtonClickSound()
    {
        PlaySFX("ButtonClick");
    }

    /// <summary>
    /// SFX 재생 (효과음 종류별)
    /// </summary>
    public void PlaySFX(string type)
    {
        if (type == "ButtonClick" && buttonSFX != null)
            sfxSource.PlayOneShot(buttonSFX);
        // 필요시 SFX 종류별 분기 추가
    }
}

