using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private const string SaveKey = "Sudoku_Save";

    [Serializable]
    private class BoardState
    {
        public int[] values;      // ���� 81�� (0 = ��ĭ)
        public bool[] fixeds;     // ���� �� ���� 81��
        public int[] corrects;    // ���䰪 81��
        public int hintCount;     // ��� ��Ʈ Ƚ��
        public float elapsedTime; // ��� �ð�(��)
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ����: 2���� �迭(9x9)�� ���� ����(hint, timer)�� 1���� BoardState�� ����
    public void SaveState(int[,] values, bool[,] fixeds, int[,] corrects, int hintCount, float elapsedTime)
    {
        var state = new BoardState
        {
            values = new int[81],
            fixeds = new bool[81],
            corrects = new int[81],
            hintCount = hintCount,
            elapsedTime = elapsedTime
        };
        for (int i = 0; i < 81; i++)
        {
            int r = i / 9, c = i % 9;
            state.values[i] = values[r, c];
            state.fixeds[i] = fixeds[r, c];
            state.corrects[i] = corrects[r, c];
        }
        string json = JsonUtility.ToJson(state);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        //Debug.Log("[SaveManager] SaveState �Ϸ� (" + SaveKey + ")");
    }


    // �ҷ�����: BoardState ��ü�� ����, ���� ���ų� �߸��� ��� false ��ȯ
    public bool LoadState(out int[,] values, out bool[,] fixeds, out int[,] corrects, out int hintCount, out float elapsedTime)
    {
        string json = PlayerPrefs.GetString(SaveKey, null);
        if (string.IsNullOrEmpty(json))
        {
            values = null; fixeds = null; corrects = null; hintCount = 0; elapsedTime = 0;
            Debug.LogWarning("[SaveManager] ���� �����Ͱ� ���� (" + SaveKey + ")");
            return false;
        }
        try
        {
            BoardState state = JsonUtility.FromJson<BoardState>(json);
            // ������ ��ȿ�� �˻�
            if (state.values == null || state.fixeds == null || state.corrects == null ||
                state.values.Length != 81 || state.fixeds.Length != 81 || state.corrects.Length != 81)
            {
                throw new Exception("BoardState �迭 ũ�� ����");
            }
            values = new int[9, 9];
            fixeds = new bool[9, 9];
            corrects = new int[9, 9];
            for (int i = 0; i < 81; i++)
            {
                int r = i / 9, c = i % 9;
                values[r, c] = state.values[i];
                fixeds[r, c] = state.fixeds[i];
                corrects[r, c] = state.corrects[i];
            }
            hintCount = state.hintCount;
            elapsedTime = state.elapsedTime;
            //Debug.Log("[SaveManager] LoadState ���� (" + SaveKey + ")");
            return true;
        }
        catch (Exception e)
        {
            values = null; fixeds = null; corrects = null; hintCount = 0; elapsedTime = 0;
            //Debug.LogError("[SaveManager] ���� ������ ���� ����: " + e.Message);
            return false;
        }
    }


    // ����� ���� ������ ���� ����
    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    // ���� ������ ���� (�� ���� ��)
    public void ClearState()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        //Debug.Log("[SaveManager] ���� ������ ���� (" + SaveKey + ")");
    }
}
