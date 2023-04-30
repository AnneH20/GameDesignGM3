using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalkGenerator : AbstractGenerator
{
    [SerializeField] private RandomWalkData randomWalkData = null;
    protected override void RunProceduralGenInternal()
    {
        HashSet<Vector2Int> floorpos = RunRandomWalker();
        tilemapVisualizer.ClearFloorTiles();
        tilemapVisualizer.PaintFloorTiles(floorpos);
        tilemapVisualizer.ClearWallTiles();
        WallGenerator.createWalls(floorpos, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalker()
    {
        var currentpos = startpos;
        HashSet<Vector2Int> floorpos = new HashSet<Vector2Int>();
        for (int i = 0; i < randomWalkData.iterations; i++)
        {
            var path = ProceduralGenerator.RandomWalker(currentpos, randomWalkData.walklen);
            floorpos.UnionWith(path);
            if (randomWalkData.useRandomSeed)
            {   
               currentpos = floorpos.ElementAt(UnityEngine.Random.Range(0, floorpos.Count));
            }
        }
        return floorpos;
    }
}
