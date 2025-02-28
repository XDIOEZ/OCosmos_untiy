using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map_Chunks", menuName = "Map_Chunks/MapData", order = 1)]
public class Map_SO : ScriptableObject
{
    public Vector2Int mapSize;
    public Vector2Int chunkSize;
    public List<ChunkData> chunkDataList = new List<ChunkData>();

    public Map_SO GetTheMap()
    {
        return this;
    }

}
