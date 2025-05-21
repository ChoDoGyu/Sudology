using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;  // 시간 표시할 UI 텍스트
    private float startTime;                      // 게임 시작 시간
    private float elapsedTime;                    // 경과 시간
    private bool isGameRunning = false;           // 게임 실행 여부

    // SaveManager에서 불러온 값으로 초기화
    public void SetElapsedTime(float time)
    {
        elapsedTime = time;
    }
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void StartTimer()
    {
        startTime = Time.time - elapsedTime; // 불러온 시간이 있으면 그만큼 보정
        isGameRunning = true;
    }

    public float StopTimer()
    {
        isGameRunning = false;
        return elapsedTime;
    }

    // 매 프레임마다 시간 갱신
    private void Update()
    {
        if (isGameRunning)
        {
            elapsedTime = Time.time - startTime;
            UpdateTimerUI(elapsedTime);
        }
    }

    // UI에 시간 업데이트
    private void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
        startTime = Time.time;
    }

}
