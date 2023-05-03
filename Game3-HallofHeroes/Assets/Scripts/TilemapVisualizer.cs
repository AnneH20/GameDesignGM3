using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap, nextLevelTilemap;
    [SerializeField] private TileBase floorTile, wallTile, nextLevelTile;

    public void PaintFloorTiles (IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(position, tilemap, tile);
        }
    }

    internal void PaintSingleWall(Vector2Int position)
    {
        PaintSingleTile(position, wallTilemap, wallTile);
    }

    private void PaintSingleTile(Vector2Int position, Tilemap tilemap, TileBase tile)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void ClearFloorTiles()
    {
        floorTilemap.ClearAllTiles();
    }

    public void ClearWallTiles()
    {
        wallTilemap.ClearAllTiles();
    }

    public void ClearNextLevelTiles()
    {
        nextLevelTilemap.ClearAllTiles();
    }
    public void PaintNextLevelTile(Vector2Int position)
    {
        PaintSingleTile(position, nextLevelTilemap, nextLevelTile);
    }

    public bool IsWall(Vector2Int position)
    {
        return wallTilemap.HasTile((Vector3Int)position);
    }
}
