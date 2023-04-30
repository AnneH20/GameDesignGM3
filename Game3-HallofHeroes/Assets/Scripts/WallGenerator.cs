using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProceduralGenerator;

public static class WallGenerator
{
    public static void createWalls(HashSet<Vector2Int> floorpos, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPos = FindWallsDirection(floorpos, Direction2D.CardDirections);
        foreach (var position in basicWallPos)
        {
            tilemapVisualizer.PaintSingleWall(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsDirection(HashSet<Vector2Int> floorpos, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallpos = new HashSet<Vector2Int>();
        foreach (var position in floorpos)
        {
            foreach (var direction in directionsList)
            {
                var neighborpos = position + direction;
                if (!floorpos.Contains(neighborpos))
                {
                    wallpos.Add(neighborpos);
                }
            }
        }
        return wallpos;
    }
}
