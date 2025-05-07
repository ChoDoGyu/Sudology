using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzleGenerator
{
    // 보드 크기(GridSize), 부모 Transform을 받아 퍼즐을 생성
    void Generate(Transform parent, int gridSize);
}
