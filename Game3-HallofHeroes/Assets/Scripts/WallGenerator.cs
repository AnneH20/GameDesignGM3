using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    private static readonly List<Vector2Int> AllDirections = new List<Vector2Int>
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1)
    };

    public static void CreateWalls(HashSet<Vector2Int> floorPos, TilemapVisualizer tilemapVisualizer)
    {
        var wallPos = FindWallsDirection(floorPos, AllDirections);
        foreach (var position in wallPos)
        {
            tilemapVisualizer.PaintSingleWall(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsDirection(HashSet<Vector2Int> floorPos, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPos = new HashSet<Vector2Int>();
        foreach (var position in floorPos)
        {
            foreach (var direction in directionsList)
            {
                var neighborPos = position + direction;
                if (!floorPos.Contains(neighborPos))
                {
                    wallPos.Add(neighborPos);
                }
            }
        }
        return wallPos;
    }
}

