using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    public void SelectEasy() => StartNewGame(Difficulty.Easy);
    public void SelectNormal() => StartNewGame(Difficulty.Normal);
    public void SelectHard() => StartNewGame(Difficulty.Hard);

    private void StartNewGame(Difficulty diff)
    {
        //이전 저장 삭제
        SaveManager.Instance.ClearState();

        //새 난이도 설정
        GameManager.Instance.SetDifficulty(diff);

        //GameManager 흐름을 통해 GameScene 진입 + isNewGame 설정
        GameManager.Instance.StartNewGame();
    }
}
