using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// ���̵��� Ŭ���� ���, ���/�ְ� ����� �����ϴ� �̱��� �Ŵ���
/// (�� �̵����� ��Ƴ����� DontDestroyOnLoad ����)
/// </summary>
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
            DontDestroyOnLoad(gameObject);  // �� �̵����� ��� �Ŵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    private void Start()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.onValueChanged.AddListener(_ => {
                if (panel != null && panel.activeSelf)
                    UpdateStatsUI();
            });
        }
    }

    public void TogglePanel()
    {
        if (panel == null) return;

        panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
            UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (difficultyDropdown == null) return;
        string difficulty = difficultyDropdown.options[difficultyDropdown.value].text;
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
            string json = File.ReadAllText(statsFilePath);
            Dictionary<string, StatsData> allStats = JsonUtility.FromJson<Wrapper<Dictionary<string, StatsData>>>(json).data;

            if (allStats != null && allStats.ContainsKey(difficulty))
                stats = allStats[difficulty];
        }

        return stats;
    }

    // JSON�� ��� ������ ����
    public void SaveStats(string difficulty, StatsData stats)
    {
        Dictionary<string, StatsData> allStats = new Dictionary<string, StatsData>();

        if (File.Exists(statsFilePath))
        {
            string json = File.ReadAllText(statsFilePath);
            allStats = JsonUtility.FromJson<Wrapper<Dictionary<string, StatsData>>>(json).data;
        }

        allStats[difficulty] = stats;

        string jsonData = JsonUtility.ToJson(new Wrapper<Dictionary<string, StatsData>> { data = allStats }, true);
        File.WriteAllText(statsFilePath, jsonData);
    }

    /// <summary>
    /// GameManager ��� ȣ�� (���̵�, �ð�, ��Ʈ ��� Ƚ��)
    /// </summary>
    public void UpdateStats(Difficulty difficulty, float clearTime, int hintCount)
    {
        string diffStr = difficulty.ToString();

        // 1. ���� ��� �ҷ�����
        StatsData stats = LoadStats(diffStr);

        // 2. Ŭ���� Ƚ�� ����
        stats.clearCount++;

        // 3. �ְ� ���(�ּ� �ð�) ����
        if (stats.bestTime <= 0f || (clearTime > 0 && clearTime < stats.bestTime))
            stats.bestTime = clearTime;

        // 4. ��� �ð� ���� (������)
        if (stats.clearCount == 1)
            stats.avgTime = clearTime;
        else
            stats.avgTime = ((stats.avgTime * (stats.clearCount - 1)) + clearTime) / stats.clearCount;

        // 5. ��� ��Ʈ ��� Ƚ�� ����
        if (stats.clearCount == 1)
            stats.avgHintUsage = hintCount;
        else
            stats.avgHintUsage = ((stats.avgHintUsage * (stats.clearCount - 1)) + hintCount) / stats.clearCount;

        // 6. ����
        SaveStats(diffStr, stats);

        // 7. �ʿ�� UI ����
        UpdateStatsUI();
    }
}
