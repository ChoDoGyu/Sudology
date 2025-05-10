using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // TMP 사용

public class StatsManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private TMP_Text clearCountText;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private TMP_Text avgTimeText;
    [SerializeField] private TMP_Text avgHintText;

    private void Awake()
    {
        difficultyDropdown.onValueChanged.AddListener(_ => {
            if (panel.activeSelf)
                UpdateStatsUI();
        });
    }

    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
        if (panel.activeSelf)
            UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        // 선택된 난이도 문자열
        string difficulty = difficultyDropdown.options[difficultyDropdown.value].text;

        // PlayerPrefs 예시 키 (프로젝트 저장 방식에 맞게 조정)
        int clearCount = PlayerPrefs.GetInt($"Stats_{difficulty}_ClearCount", 0);
        float bestTime = PlayerPrefs.GetFloat($"Stats_{difficulty}_BestTime", 0f);
        float avgTime = PlayerPrefs.GetFloat($"Stats_{difficulty}_AvgTime", 0f);
        int avgHints = PlayerPrefs.GetInt($"Stats_{difficulty}_AvgHints", 0);

        clearCountText.text = $"Clear Count ({difficulty}): {clearCount}";
        bestTimeText.text = $"Best Time ({difficulty}): {FormatTime(bestTime)}";
        avgTimeText.text = $"Avg Time ({difficulty}): {FormatTime(avgTime)}";
        avgHintText.text = $"Avg Hints Used ({difficulty}): {avgHints}";
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        return $"{m}:{s:00}";
    }
}
