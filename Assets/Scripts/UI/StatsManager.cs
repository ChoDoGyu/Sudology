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

    private string statsFilePath => Application.persistentDataPath + "/stats.json"; // JSON ���� ���

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
        // ���õ� ���̵� ���ڿ�
        string difficulty = difficultyDropdown.options[difficultyDropdown.value].text;

        // JSON ���Ͽ��� �ش� ���̵��� ��� �ҷ�����
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

    // JSON���� ��� ������ �ҷ�����
    public StatsData LoadStats(string difficulty)
    {
        StatsData stats = new StatsData();

        if (File.Exists(statsFilePath))
        {
            // JSON ������ �а� StatsData ��ü�� ��ȯ
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

    // JSON�� ��� ������ ����
    public void SaveStats(string difficulty, StatsData stats)
    {
        Dictionary<string, StatsData> allStats = new Dictionary<string, StatsData>();

        // ������ �ִٸ� ���� ������ �б�
        if (File.Exists(statsFilePath))
        {
            string json = File.ReadAllText(statsFilePath);
            allStats = JsonUtility.FromJson<Wrapper<Dictionary<string, StatsData>>>(json).data;
        }

        // ������ ����
        allStats[difficulty] = stats;

        // JSON �������� ��ȯ�Ͽ� ���Ͽ� ����
        string jsonData = JsonUtility.ToJson(new Wrapper<Dictionary<string, StatsData>> { data = allStats }, true);
        File.WriteAllText(statsFilePath, jsonData);
    }
}
