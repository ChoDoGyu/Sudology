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
        public int[] values;   // ���� 81��, 0 = ��ĭ
        public bool[] fixeds;  // ���� �� ���� 81��
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

    // ����: values, fixeds �迭�� JSON���� ����ȭ
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


    // �ҷ�����: ����� JSON�� ������ null ��ȯ
    public BoardState LoadState()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return null;
        string json = PlayerPrefs.GetString(SaveKey);
        return JsonUtility.FromJson<BoardState>(json);
    }

    // �ʱ�ȭ
    public void ClearState()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }
}
