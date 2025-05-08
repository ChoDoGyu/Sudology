using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    public void SelectEasy()
    {
        GameManager.Instance.SetDifficulty(Difficulty.Easy);
        SceneManager.LoadScene("GameScene");
    }

    public void SelectNormal()
    {
        GameManager.Instance.SetDifficulty(Difficulty.Normal);
        SceneManager.LoadScene("GameScene");
    }

    public void SelectHard()
    {
        GameManager.Instance.SetDifficulty(Difficulty.Hard);
        SceneManager.LoadScene("GameScene");
    }
}
