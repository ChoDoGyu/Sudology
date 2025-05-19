using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // StartScene을 로드하는 공개 메서드
    public void LoadStartScene()
    {
        // 1. 퍼즐 상태 저장
        if (InputManager.Instance != null)
        {
            int cellCount = // 퍼즐 셀 개수 로그로 찍기
                GameObject.Find("PuzzleBoard")?.GetComponentsInChildren<PuzzleCell>().Length ?? -1;
            Debug.Log($"저장 직전 퍼즐 셀 개수: {cellCount}");
            InputManager.Instance.SaveCurrentState();
        }

        if (GameManager.Instance != null && GameManager.Instance.gameTimer != null)
        {
            // 반드시 타이머 멈춤 → 실제로 정지해야 씬 전환 후에도 지속되지 않음
            float time = GameManager.Instance.gameTimer.StopTimer();
            PlayerPrefs.SetFloat("LastElapsedTime", time);
            PlayerPrefs.SetInt("HintCount", GameManager.Instance.currentHintCount);
            Debug.Log($"게임 저장 후 타이머 정지됨: {time}초");
        }

        PlayerPrefs.Save();

        Debug.Log("게임 저장 후 StartScene으로 이동합니다.");

        SceneManager.LoadScene("StartScene");
    }
}
