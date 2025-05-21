using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // StartScene을 로드하는 공개 메서드
    public void LoadStartScene()
    {
        // 1. 퍼즐 상태 저장 (퍼즐, 힌트, 타이머 모두 포함)
        if (InputManager.Instance != null)
        {
            int cellCount =
                GameObject.Find("PuzzleBoard")?.GetComponentsInChildren<PuzzleCell>().Length ?? -1;
            //Debug.Log($"저장 직전 퍼즐 셀 개수: {cellCount}");
            InputManager.Instance.SaveCurrentState();
        }

        // 타이머 멈춤 (이 부분은 남겨도 무방. 단, PlayerPrefs 저장은 필요없음)
        if (GameManager.Instance != null && GameManager.Instance.gameTimer != null)
        {
            float time = GameManager.Instance.gameTimer.StopTimer();
            //Debug.Log($"게임 저장 후 타이머 정지됨: {time}초");
        }

        //Debug.Log("게임 저장 후 StartScene으로 이동합니다.");
        SceneManager.LoadScene("StartScene");
    }
}
