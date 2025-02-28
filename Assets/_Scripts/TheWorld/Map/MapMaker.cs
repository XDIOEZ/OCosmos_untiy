using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
[System.Serializable]
public class MapMaker
{
    #region 变量
    public Dictionary<string, ChunkData> _Map; // 存储地图数据


    #endregion
    public Dictionary<string, ChunkData> GenerateMap(Map_Data map_Data)
    {
        _Map = GenerateRandomMapData(map_Data.MapSize, map_Data.chunkSize, map_Data); // 生成随机地图字典
        return map_Data.AllChunksData_Dic = _Map; // 序列化地图数据
    }
    #region 地图生成函数

    public Dictionary<string, ChunkData> GenerateRandomMapData(Vector2Int mapSize, Vector2Int chunkSize, Map_Data map_Data)
    {
        Dictionary<string, ChunkData> mapChunkData = new Dictionary<string, ChunkData>();
        int startX = map_Data.MapCenter.x - mapSize.x / 2;
        int endX = map_Data.MapCenter.x + mapSize.x / 2;
        int startY = map_Data.MapCenter.y - mapSize.y / 2;
        int endY = map_Data.MapCenter.y + mapSize.y / 2;
        ChunkType chunkType = ChunkType.Water; // 默认区块类型


        for (int x = startX; x < endX; x += chunkSize.x)
        {
            for (int y = startY; y < endY; y += chunkSize.y)
            {
                string chunkName = $"Chunk_{x}_{y}";

                List<BlockData> blockDataList = SetBlockDataList(chunkType, chunkSize.x, chunkSize.y);

                ChunkData chunkData = new ChunkData(chunkName, new Vector2Int(x, y),chunkType, blockDataList,new List<ItemData>());

                mapChunkData.Add(chunkName, chunkData);
            }
        }

        return mapChunkData;
    } 

    private List<BlockData> SetBlockDataList(ChunkType chunkType, int width, int height)
    {
        List<BlockData> blockDataList = new List<BlockData>(); // 存储生成的方块数据

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                 
                //blockDataList.Add(new BlockData("air"));
            }
        }

        return blockDataList;
    }
}


#endregion





