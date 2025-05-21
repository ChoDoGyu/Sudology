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
        public int[] values;      // 셀값 81개 (0 = 빈칸)
        public bool[] fixeds;     // 고정 셀 여부 81개
        public int[] corrects;    // 정답값 81개
        public int hintCount;     // 사용 힌트 횟수
        public float elapsedTime; // 경과 시간(초)
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

    // 저장: 2차원 배열(9x9)과 보조 정보(hint, timer)를 1차원 BoardState에 저장
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
        //Debug.Log("[SaveManager] SaveState 완료 (" + SaveKey + ")");
    }


    // 불러오기: BoardState 전체를 복원, 값이 없거나 잘못된 경우 false 반환
    public bool LoadState(out int[,] values, out bool[,] fixeds, out int[,] corrects, out int hintCount, out float elapsedTime)
    {
        string json = PlayerPrefs.GetString(SaveKey, null);
        if (string.IsNullOrEmpty(json))
        {
            values = null; fixeds = null; corrects = null; hintCount = 0; elapsedTime = 0;
            Debug.LogWarning("[SaveManager] 저장 데이터가 없음 (" + SaveKey + ")");
            return false;
        }
        try
        {
            BoardState state = JsonUtility.FromJson<BoardState>(json);
            // 데이터 유효성 검사
            if (state.values == null || state.fixeds == null || state.corrects == null ||
                state.values.Length != 81 || state.fixeds.Length != 81 || state.corrects.Length != 81)
            {
                throw new Exception("BoardState 배열 크기 오류");
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
            //Debug.Log("[SaveManager] LoadState 성공 (" + SaveKey + ")");
            return true;
        }
        catch (Exception e)
        {
            values = null; fixeds = null; corrects = null; hintCount = 0; elapsedTime = 0;
            //Debug.LogError("[SaveManager] 저장 데이터 복원 실패: " + e.Message);
            return false;
        }
    }


    // 저장된 퍼즐 데이터 존재 여부
    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    // 저장 데이터 삭제 (새 게임 등)
    public void ClearState()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        //Debug.Log("[SaveManager] 저장 데이터 삭제 (" + SaveKey + ")");
    }
}
