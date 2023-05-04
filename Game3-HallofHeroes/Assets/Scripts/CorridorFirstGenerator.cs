using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static ProceduralGenerator;
using UnityEngine.Tilemaps;

public class CorridorFirstGenerator : RandomWalkGenerator
{
    [SerializeField] private int corridorLength = 10;
    [SerializeField] private int corridorCount = 10;
    [SerializeField] private int corridorWidth = 2;
    [SerializeField] [Range(0.1f, 1f)] private float roomChance = 0.8f;
    [SerializeField] private GameObject enemyPrefab = null;
    [SerializeField] private GameObject bossPrefab = null;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Tilemap tilemap = null;
    [SerializeField] private float spawnRadius = 1.0f;
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomDict = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    private HashSet<Vector2Int> floorpos, corridorpos;

    private void Start()
    {
        RunProceduralGen();
    }
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
        // Create a room at the starting position
        roompos.Add(startpos);
        var startRoom = RunRandomWalker(randomWalkData, startpos);
        roompos.UnionWith(startRoom);
        // Find the room furthest from the start position
        Vector2Int furthestRoom = roomposList[0];
        float maxDistance = 0f;
        foreach (var pos in roomposList)
        {
            float distance = Vector2Int.Distance(pos, startpos);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestRoom = pos;
            }
        }
        foreach (var pos in roomposList)
        {
            // Create a special tile in the furthest room that loads the next level
            if (pos == furthestRoom)
            {
                // Add the position of the special tile to the room position set
                roompos.Add(pos);
                var farRoom = RunRandomWalker(randomWalkData, pos);
                roompos.UnionWith(farRoom);
                Instantiate(bossPrefab, (Vector3Int)pos, Quaternion.identity);
                // Spawn the special tile
                SpawnNextLevelTile(pos);
                
            }
            // Otherwise, create a regular room
            else
            {
                var room = RunRandomWalker(randomWalkData, pos);
                roompos.UnionWith(room);
                // Spawn enemies in this room
                if (pos != startpos && pos != furthestRoom)
                {
                    foreach (var roomPos in room)
                    {
                        TilemapCollider2D tilemapCollider = tilemap.GetComponent<TilemapCollider2D>();
                        Tilemap tiles = tilemapCollider.GetComponent<Tilemap>();
                        Vector3Int tilePosition = tiles.WorldToCell((Vector3Int)roomPos);
                        TileBase tile = tiles.GetTile(tilePosition);
                        if (tile == null)
                        {
                            if (UnityEngine.Random.value < 0.1f)
                            {
                                Vector3 spawnPos = (Vector3Int)roomPos + new Vector3(0.5f, 0.5f, 0f);
                                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                            }   
                        }
                        else
                        {
                            Debug.Log("Cannot spawn enemy - wall detected.");
                        }  
                    }
                }
                
            }
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
        corridorpos = new HashSet<Vector2Int>(floorpos);
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

    private void SaveRoomData(Vector2Int position, HashSet<Vector2Int> room)
    {
        roomDict[position] = room;
    }
    private void ClearRoomData()
    {
        roomDict.Clear();
    }

    private void SpawnNextLevelTile(Vector2Int pos)
    {
        tilemapVisualizer.PaintNextLevelTile(pos);
    }
    public static void DestroyEnemy()
    {
        List<GameObject> enemies = new List<GameObject>();
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        enemies.Add(GameObject.FindGameObjectWithTag("Boss"));
        foreach (GameObject enemy in enemies)
        {
            DestroyImmediate(enemy);
        }
    }
}
