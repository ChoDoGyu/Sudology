using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzleGenerator
{
    void Generate(Transform parent, int gridSize, Difficulty difficulty);
    void GenerateFromState(Transform parent, int gridSize, Difficulty difficulty, int[,] values, bool[,] fixeds, int[,] corrects);
}