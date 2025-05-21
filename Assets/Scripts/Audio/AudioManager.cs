using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ��� �����(ȿ����, BGM) �� ��ư ȿ���� �ڵ� ����� �����ϴ� �̱��� �Ŵ���
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
    /// �� �� ��� Button�� Ŭ���� �ڵ� ���
    /// UI�� ���� Panel/Group���� �л�Ǿ� �ְ�, �ϰ� ȿ���� ������ �ʿ��� �� ��ü �˻� ���(���� ���� ����)
    /// </summary>
    private void RegisterButtonSounds()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // ��� ��ư �˻� (��Ȱ�� ����)
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(OnButtonClickSound); // �ߺ� ����
            btn.onClick.AddListener(OnButtonClickSound);
        }
    }

    private void OnButtonClickSound()
    {
        PlaySFX("ButtonClick");
    }

    /// <summary>
    /// SFX ��� (ȿ���� ������)
    /// </summary>
    public void PlaySFX(string type)
    {
        if (type == "ButtonClick" && buttonSFX != null)
            sfxSource.PlayOneShot(buttonSFX);
        // �ʿ�� SFX ������ �б� �߰�
    }
}

