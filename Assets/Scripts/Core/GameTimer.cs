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

    // 게임 시작 시 호출
    public void StartTimer()
    {
        startTime = Time.time;    // 게임 시작 시간 기록
        isGameRunning = true;
    }

    // 게임 종료 시 호출
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
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void ResumeFrom(float seconds)
    {
        elapsedTime = seconds;
        isGameRunning = true;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
