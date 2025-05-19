using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // StartScene�� �ε��ϴ� ���� �޼���
    public void LoadStartScene()
    {
        // 1. ���� ���� ����
        if (InputManager.Instance != null)
        {
            int cellCount = // ���� �� ���� �α׷� ���
                GameObject.Find("PuzzleBoard")?.GetComponentsInChildren<PuzzleCell>().Length ?? -1;
            Debug.Log($"���� ���� ���� �� ����: {cellCount}");
            InputManager.Instance.SaveCurrentState();
        }

        if (GameManager.Instance != null && GameManager.Instance.gameTimer != null)
        {
            // �ݵ�� Ÿ�̸� ���� �� ������ �����ؾ� �� ��ȯ �Ŀ��� ���ӵ��� ����
            float time = GameManager.Instance.gameTimer.StopTimer();
            PlayerPrefs.SetFloat("LastElapsedTime", time);
            PlayerPrefs.SetInt("HintCount", GameManager.Instance.currentHintCount);
            Debug.Log($"���� ���� �� Ÿ�̸� ������: {time}��");
        }

        PlayerPrefs.Save();

        Debug.Log("���� ���� �� StartScene���� �̵��մϴ�.");

        SceneManager.LoadScene("StartScene");
    }
}
