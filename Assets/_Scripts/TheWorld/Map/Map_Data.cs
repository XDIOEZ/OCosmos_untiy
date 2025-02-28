using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MemoryPackable(SerializeLayout.Explicit)]
[System.Serializable]
public partial class Map_Data
{
    [MemoryPackOrder(0)]
    public string MapName;//地图名称

    [MemoryPackOrder(1)]
    public string CreateMapDate;//创建地图日期

    [MemoryPackOrder(2)]
    public string LastEditDate;//最后编辑日期

    [MemoryPackOrder(3)]
    public Vector2Int MapCenter = new Vector2Int(0, 0);//地图中心点

    [MemoryPackOrder(4)]
    [ShowInInspector]
    public Dictionary<string, ChunkData> AllChunksData_Dic = new Dictionary<string, ChunkData>(); // 存储地图中的所有未加载区块
   
    [MemoryPackOrder(5)]
    [ShowInInspector]
    public int ChunkDataCount
    {
        get
        {
            if (AllChunksData_Dic == null)
            {
                return 0;
            }
            return  AllChunksData_Dic.Count;
        }
    }
  
    [MemoryPackOrder(6)]
    [FoldoutGroup ("地图默认数据"), ReadOnly]
    public Vector2Int chunkSize = new Vector2Int(25, 25);//每个区块的大小
  
    [MemoryPackOrder(7)]
    [FoldoutGroup("地图默认数据"), ReadOnly]
    public Vector2Int MapSize = new Vector2Int(500, 500);//地图大小

    [MemoryPackConstructor]
    /// <summary>
    /// 初始化地图数据
    /// </summary>
    /// <param name="mapName">名字</param>
    /// <param name="chunkSize">区块大小</param>
    /// <param name="mapCenter">地图中心点</param>
    /// <param name="mapSize">地图宽高</param>
    public Map_Data(string mapName, Vector2Int chunkSize , Vector2Int mapCenter, Vector2Int mapSize)
    {
        MapName = mapName;
        this.chunkSize = chunkSize;
        MapCenter = mapCenter;
        MapSize = mapSize;
        CreateMapDate = System.DateTime.Now.ToString();
    }


}
