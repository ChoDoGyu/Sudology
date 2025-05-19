using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;  // �ð� ǥ���� UI �ؽ�Ʈ
    private float startTime;                      // ���� ���� �ð�
    private float elapsedTime;                    // ��� �ð�
    private bool isGameRunning = false;           // ���� ���� ����

    // ���� ���� �� ȣ��
    public void StartTimer()
    {
        startTime = Time.time;    // ���� ���� �ð� ���
        isGameRunning = true;
    }

    // ���� ���� �� ȣ��
    public float StopTimer()
    {
        isGameRunning = false;
        return elapsedTime;
    }

    // �� �����Ӹ��� �ð� ����
    private void Update()
    {
        if (isGameRunning)
        {
            elapsedTime = Time.time - startTime;
            UpdateTimerUI(elapsedTime);
        }
    }

    // UI�� �ð� ������Ʈ
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
