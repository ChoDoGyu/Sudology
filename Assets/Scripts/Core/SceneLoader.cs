using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // StartScene을 로드하는 공개 메서드
    public void LoadStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
