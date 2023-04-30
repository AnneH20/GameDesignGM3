using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerator
{
    public static HashSet<Vector2Int> RandomWalker (Vector2Int startpos, int walklen)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(startpos);
        var prevpos = startpos;
        for (int i = 0; i < walklen; i++)
        {
            var newpos = prevpos + Direction2D.RandomDirection();
            path.Add(newpos);
            prevpos = newpos;
        }
        return path;
    }

    public static class Direction2D
    {
        public static List<Vector2Int> CardDirections = new List<Vector2Int> 
        { 
            new Vector2Int(0, 1), // up
            new Vector2Int(1, 0), // right
            new Vector2Int(0, -1), // down
            new Vector2Int(-1, 0) // left
        };

        public static Vector2Int RandomDirection()
        {
            return CardDirections[Random.Range(0, CardDirections.Count)];
        }
    }
}
