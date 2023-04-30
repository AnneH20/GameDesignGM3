using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalkGenerator : AbstractGenerator
{
    [SerializeField] protected RandomWalkData randomWalkData = null;

    private void Start()
    {
        RunProceduralGen();
    }
    protected override void RunProceduralGenInternal()
    {
        HashSet<Vector2Int> floorpos = RunRandomWalker(randomWalkData, startpos);
        tilemapVisualizer.ClearFloorTiles();
        tilemapVisualizer.PaintFloorTiles(floorpos);
        tilemapVisualizer.ClearWallTiles();
        WallGenerator.createWalls(floorpos, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalker(RandomWalkData randomWalkData, Vector2Int pos)
    {
        var currentpos = pos;
        HashSet<Vector2Int> floorpos = new HashSet<Vector2Int>();
        for (int i = 0; i < randomWalkData.iterations; i++)
        {
            var floors = ProceduralGenerator.RandomWalker(currentpos, randomWalkData.walklen);
            floorpos.UnionWith(floors);
            if (randomWalkData.useRandomSeed)
            {   
               currentpos = floorpos.ElementAt(UnityEngine.Random.Range(0, floorpos.Count));
            }
        }
        return floorpos;
    }
}
