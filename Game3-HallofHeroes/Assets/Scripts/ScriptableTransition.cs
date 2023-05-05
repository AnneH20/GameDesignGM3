using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScriptableTransition : ScriptableObject
{
    public List<SavedTile> savedTiles = new List<SavedTile>();
}

public class SavedTile
{
    public Vector3Int position;
    public TileBase tile;
    public SavedTile(Vector3Int position, TileBase tile)
    {
        this.position = position;
        this.tile = tile;
    }
}
