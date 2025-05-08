using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private const string SaveKey = "Sudoku_Save";

    [Serializable]
    public class BoardState
    {
        public int[] values;   // 셀값 81개, 0 = 빈칸
        public bool[] fixeds;  // 고정 셀 여부 81개
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // 저장: values, fixeds 배열을 JSON으로 직렬화
    public void SaveState(int[,] values, bool[,] fixeds)
    {
        var state = new BoardState
        {
            values = new int[values.Length],
            fixeds = new bool[values.Length]
        };
        for (int i = 0; i < values.Length; i++)
        {
            state.values[i] = values[i / 9, i % 9];
            state.fixeds[i] = fixeds[i / 9, i % 9];
        }
        string json = JsonUtility.ToJson(state);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }


    // 불러오기: 저장된 JSON이 없으면 null 반환
    public BoardState LoadState()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return null;
        string json = PlayerPrefs.GetString(SaveKey);
        return JsonUtility.FromJson<BoardState>(json);
    }

    // 초기화
    public void ClearState()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }
}
