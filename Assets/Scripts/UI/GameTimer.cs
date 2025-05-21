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

    // SaveManager���� �ҷ��� ������ �ʱ�ȭ
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
        startTime = Time.time - elapsedTime; // �ҷ��� �ð��� ������ �׸�ŭ ����
        isGameRunning = true;
    }

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
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
        startTime = Time.time;
    }

}
