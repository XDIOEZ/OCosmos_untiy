using MemoryPack;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class TheWorldData
{
#region 字段

    public string TheWorldName = ""; // 世界地图名称
    public Vector2Int WorldSize = new Vector2Int(8000, 4000); // 世界地图大小
    public Vector2Int MapTileSize = new Vector2Int(400, 400); // 单张地图大小
    public Vector2Int ChunkSize = new Vector2Int(25, 25); // 地图块大小
    public bool IsZip = false; // 是否使用了Gzip压缩地图数据

    [ShowInInspector]
    private Dictionary<string, byte[]> worldData = new Dictionary<string, byte[]>(); // 存储世界地图数据的字典

    public byte[][,] WorldData_Array = new byte[2][,]; // 存储世界地图数据的二维数组

    #endregion

    #region 属性

    /// <summary>
    /// 获取世界地图包含的地图数量
    /// </summary>
    public Vector2Int MapDataCount
    {
        get
        {
            return new Vector2Int(WorldSize.x / MapTileSize.x, WorldSize.y / MapTileSize.y);
        }
    }

    public Dictionary<string, byte[]> WorldData { get => worldData; set => worldData = value; }

    #endregion

    #region 地图数据加载与保存方法

    //数据的读取方法
    public Map_Data LoadMapDataByCenter(Vector2Int center)
    {
        //TODO :添加自动检测是否需要解压的功能,同时利用缓存机制提高效率
        // 生成分块中心坐标的键值
        string centerKey = TheWorld.Vector2IntToString(center);

        // 检查地图数据字典中是否存在该键值
        if (!WorldData.ContainsKey(centerKey))
        {
            Debug.Log("地图数据不存在);");
            return null;
        }

        try
        {
            byte[] mapDataBytes;
            byte[] compressedMapDataBytes = WorldData[centerKey];

            // 根据是否使用Gzip解压缩地图数据
            if (IsZip)
            {
                mapDataBytes = MapDataUtility.Decompress(compressedMapDataBytes);
               // Debug.Log("地图数据已解压");
            }
            else
            {
                mapDataBytes = compressedMapDataBytes;
                //Debug.Log("地图数据未压缩");
            }

            // 反序列化地图数据
            Map_Data mapData = MemoryPackSerializer.Deserialize<Map_Data>(mapDataBytes);
            Debug.Log("地图名称: " + mapData.MapName);
            return mapData;
        }
        catch (Exception ex)
        {
            Debug.LogError("地图数据解析失败！分块中心: " + centerKey + ", 错误: " + ex.Message);
            return null;
        }
    }



    //数据的写入方法
    public bool Save_OneMapData_To_TheWorldData(Map_Data mapData, bool useGzip)
    {
        // 检查地图数据是否为空
        if (mapData == null)
        {
            Debug.LogError("保存的地图数据为空！");
            return false;
        }

        // 序列化地图数据
        byte[] mapDataBytes = MemoryPackSerializer.Serialize(mapData);

        // 检查序列化结果
        if (mapDataBytes == null)
        {
            return false;
        }

        // 生成分块中心坐标的键值
        string centerKey = TheWorld.Vector2IntToString(mapData.MapCenter);

        // 根据是否使用Gzip压缩地图数据
        if (useGzip)
        {
            try
            {
                mapDataBytes = MapDataUtility.Compress(mapDataBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError("压缩失败：" + ex.Message);
                return false;
            }
        }

        // 更新或添加地图数据到字典
        if (WorldData.ContainsKey(centerKey))
        {
            WorldData[centerKey] = mapDataBytes;
        }
        else
        {
            WorldData.Add(centerKey, mapDataBytes);
        }

        return true;
    }

    public void SetTheWorldDataName(string name)
    {
        TheWorldName = name;
    }

#endregion
}
