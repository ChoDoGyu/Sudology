using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzleGenerator
{
    void Generate(Transform parent, int gridSize, Difficulty difficulty);
}