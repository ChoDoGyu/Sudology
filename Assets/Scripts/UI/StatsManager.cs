using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private TMP_Text clearCountText;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private TMP_Text avgTimeText;
    [SerializeField] private TMP_Text avgHintText;

    private string statsFilePath => Application.persistentDataPath + "/stats.json"; // JSON 파일 경로

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
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

        // JSON 파일에서 해당 난이도의 통계 불러오기
        StatsData stats = LoadStats(difficulty);

        clearCountText.text = $"Clear Count ({difficulty}): {stats.clearCount}";
        bestTimeText.text = $"Best Time ({difficulty}): {FormatTime(stats.bestTime)}";
        avgTimeText.text = $"Avg Time ({difficulty}): {FormatTime(stats.avgTime)}";
        avgHintText.text = $"Avg Hints Used ({difficulty}): {stats.avgHintUsage}";
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        return $"{m}:{s:00}";
    }

    // JSON에서 통계 데이터 불러오기
    public StatsData LoadStats(string difficulty)
    {
        StatsData stats = new StatsData();

        if (File.Exists(statsFilePath))
        {
            // JSON 파일을 읽고 StatsData 객체로 변환
            string json = File.ReadAllText(statsFilePath);
            Dictionary<string, StatsData> allStats = JsonUtility.FromJson<Wrapper<Dictionary<string, StatsData>>>(json).data;

            if (allStats.ContainsKey(difficulty))
                stats = allStats[difficulty];
        }
        else
        {
            Debug.Log("No save data found. Using default values.");
        }

        return stats;
    }

    // JSON에 통계 데이터 저장
    public void SaveStats(string difficulty, StatsData stats)
    {
        Dictionary<string, StatsData> allStats = new Dictionary<string, StatsData>();

        // 파일이 있다면 기존 데이터 읽기
        if (File.Exists(statsFilePath))
        {
            string json = File.ReadAllText(statsFilePath);
            allStats = JsonUtility.FromJson<Wrapper<Dictionary<string, StatsData>>>(json).data;
        }

        // 데이터 갱신
        allStats[difficulty] = stats;

        // JSON 형식으로 변환하여 파일에 저장
        string jsonData = JsonUtility.ToJson(new Wrapper<Dictionary<string, StatsData>> { data = allStats }, true);
        File.WriteAllText(statsFilePath, jsonData);
    }
}
