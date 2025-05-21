using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 난이도별 클리어 통계, 평균/최고 기록을 관리하는 싱글턴 매니저
/// (씬 이동에도 살아남도록 DontDestroyOnLoad 적용)
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

    private string statsFilePath => Application.persistentDataPath + "/stats.json"; // JSON 파일 경로

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 이동에도 통계 매니저 유지
        }
        else
        {
            Destroy(gameObject); // 중복 방지
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

    // JSON에서 통계 데이터 불러오기
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

    // JSON에 통계 데이터 저장
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
    /// GameManager 등에서 호출 (난이도, 시간, 힌트 사용 횟수)
    /// </summary>
    public void UpdateStats(Difficulty difficulty, float clearTime, int hintCount)
    {
        string diffStr = difficulty.ToString();

        // 1. 기존 통계 불러오기
        StatsData stats = LoadStats(diffStr);

        // 2. 클리어 횟수 증가
        stats.clearCount++;

        // 3. 최고 기록(최소 시간) 갱신
        if (stats.bestTime <= 0f || (clearTime > 0 && clearTime < stats.bestTime))
            stats.bestTime = clearTime;

        // 4. 평균 시간 갱신 (산술평균)
        if (stats.clearCount == 1)
            stats.avgTime = clearTime;
        else
            stats.avgTime = ((stats.avgTime * (stats.clearCount - 1)) + clearTime) / stats.clearCount;

        // 5. 평균 힌트 사용 횟수 갱신
        if (stats.clearCount == 1)
            stats.avgHintUsage = hintCount;
        else
            stats.avgHintUsage = ((stats.avgHintUsage * (stats.clearCount - 1)) + hintCount) / stats.clearCount;

        // 6. 저장
        SaveStats(diffStr, stats);

        // 7. 필요시 UI 갱신
        UpdateStatsUI();
    }
}
