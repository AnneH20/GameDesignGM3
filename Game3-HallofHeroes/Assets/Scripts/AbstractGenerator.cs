using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGenerator : MonoBehaviour
{
    [SerializeField] protected Vector2Int startpos = Vector2Int.zero;
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;

    public void RunProceduralGen()
    {
        tilemapVisualizer.ClearFloorTiles();
        tilemapVisualizer.ClearWallTiles();
        tilemapVisualizer.ClearNextLevelTiles();
        RunProceduralGenInternal();
    }

    public void ClearFloorTiles()
    {
        tilemapVisualizer.ClearFloorTiles();
        tilemapVisualizer.ClearWallTiles();
        tilemapVisualizer.ClearNextLevelTiles();
    }

    protected abstract void RunProceduralGenInternal();
}
