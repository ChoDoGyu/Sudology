using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameClearPanel : MonoBehaviour
{
    public static GameClearPanel Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text timeText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(float timeInSeconds)
    {
        int m = Mathf.FloorToInt(timeInSeconds / 60);
        int s = Mathf.FloorToInt(timeInSeconds % 60);
        timeText.text = $"클리어 시간: {m}분 {s:D2}초";

        panel.SetActive(true);
    }

    public void OnReturnToTitle()
    {
        SceneManager.LoadScene("StartScene");
    }
}
