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
        //���� ���� ����
        SaveManager.Instance.ClearState();

        //�� ���̵� ����
        GameManager.Instance.SetDifficulty(diff);

        //GameManager �帧�� ���� GameScene ���� + isNewGame ����
        GameManager.Instance.StartNewGame();
    }
}
