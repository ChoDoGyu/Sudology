using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzleGenerator
{
    // ���� ũ��(GridSize), �θ� Transform�� �޾� ������ ����
    void Generate(Transform parent, int gridSize);
}
