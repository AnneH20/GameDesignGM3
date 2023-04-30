using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static ProceduralGenerator;

public class CorridorFirstGenerator : RandomWalkGenerator
{
    [SerializeField] private int corridorLength = 10;
    [SerializeField] private int corridorCount = 10;
    [SerializeField] private int corridorWidth = 2;
    [SerializeField] [Range(0.1f, 1f)] private float roomChance = 0.7f;
    protected override void RunProceduralGenInternal()
    {
        CorridorFirstGeneration();
    }
    // Create corridors and rooms
    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorpos = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomposPotential = new HashSet<Vector2Int>();
        List<List<Vector2Int>> corridors = CreateCorridors(floorpos, roomposPotential);
        HashSet<Vector2Int> roompos = CreateRooms(roomposPotential);
        List<Vector2Int> deadends = FindDeadEnds(floorpos);
        CreateDeadendRooms(deadends, roompos);
        floorpos.UnionWith(roompos);
        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = corridors[i];
            corridor = IncreaseCorridorWidth(corridor, corridorWidth);
            floorpos.UnionWith(corridor);
        }
        tilemapVisualizer.PaintFloorTiles(floorpos);
        WallGenerator.CreateWalls(floorpos, tilemapVisualizer);  
    }

    // Create rooms at the end of dead ends
    private void CreateDeadendRooms(List<Vector2Int> deadends, HashSet<Vector2Int> roomfloorpos)
    {
        foreach (var deadend in deadends)
        {
            var room = RunRandomWalker(randomWalkData, deadend);
            roomfloorpos.UnionWith(room);
        }
    }
    // Find dead ends
    private List<Vector2Int> FindDeadEnds(HashSet<Vector2Int> floorpos)
    {
        List<Vector2Int> deadends = new List<Vector2Int>();
        foreach (var position in floorpos)
        {
            int wallCount = 0;
            foreach (var direction in Direction2D.CardDirections)
            {
                var neighborpos = position + direction;
                if (!floorpos.Contains(neighborpos))
                {
                    wallCount++;
                }
            }
            if (wallCount >= 3)
            {
                deadends.Add(position);
            }
        }
        return deadends;
    }
    // Create rooms at the end of corridors
    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> roomposPotential)
    {
        HashSet<Vector2Int> roompos = new HashSet<Vector2Int>();
        int roomCount = Mathf.RoundToInt(roomposPotential.Count * roomChance);
        List<Vector2Int> roomposList = roomposPotential.OrderBy(x => Guid.NewGuid()).Take(roomCount).ToList();
        roomposList.Add(startpos); // Add a room at the start position
        foreach (var pos in roomposList)
        {
            var room = RunRandomWalker(randomWalkData, pos);
            roompos.UnionWith(room);
        }
        return roompos;
    }
    // Create corridors first, then create rooms at the end of corridors
    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorpos, HashSet<Vector2Int> roomposPotential)
    {
        var currentpos = startpos;
        roomposPotential.Add(currentpos);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
        for (int i = 0; i < corridorCount; i++)
        {
            var path = ProceduralGenerator.RandomWalkCorridor(currentpos, corridorLength);
            corridors.Add(path);
            currentpos = path[path.Count - 1];
            roomposPotential.Add(currentpos);
            floorpos.UnionWith(path);
        }
        return corridors;
    }
    // Increase the width of the corridor by adding neighbors to each position in the corridor
    // Set width to 2 to get both vertical and horizontal neighbors
    public List<Vector2Int> IncreaseCorridorWidth(List<Vector2Int> corridor, int width)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        foreach (var position in corridor)
        {
            newCorridor.Add(position);
            for (int i = 0; i < width; i++)
            {
                var direction = Direction2D.CardDirections[i];
                var neighborpos = position + direction;
                newCorridor.Add(neighborpos);
            }
        }
        return newCorridor;
    }
}