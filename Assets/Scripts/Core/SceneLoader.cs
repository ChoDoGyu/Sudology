using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // StartScene�� �ε��ϴ� ���� �޼���
    public void LoadStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
