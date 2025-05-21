using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // StartScene�� �ε��ϴ� ���� �޼���
    public void LoadStartScene()
    {
        // 1. ���� ���� ���� (����, ��Ʈ, Ÿ�̸� ��� ����)
        if (InputManager.Instance != null)
        {
            int cellCount =
                GameObject.Find("PuzzleBoard")?.GetComponentsInChildren<PuzzleCell>().Length ?? -1;
            //Debug.Log($"���� ���� ���� �� ����: {cellCount}");
            InputManager.Instance.SaveCurrentState();
        }

        // Ÿ�̸� ���� (�� �κ��� ���ܵ� ����. ��, PlayerPrefs ������ �ʿ����)
        if (GameManager.Instance != null && GameManager.Instance.gameTimer != null)
        {
            float time = GameManager.Instance.gameTimer.StopTimer();
            //Debug.Log($"���� ���� �� Ÿ�̸� ������: {time}��");
        }

        //Debug.Log("���� ���� �� StartScene���� �̵��մϴ�.");
        SceneManager.LoadScene("StartScene");
    }
}
