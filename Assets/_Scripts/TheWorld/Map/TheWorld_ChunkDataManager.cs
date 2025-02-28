using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 世界地图块数据管理类
/// </summary>
[System.Serializable]
public class TheWorld_ChunkDataManager
{
    #region 字段
    private const string CHUNK_NAME_FORMAT = "Chunk_{0}_{1}";
    /// <summary>
    /// 偶数大小偏移除数
    /// </summary>
    private const float EVEN_SIZE_OFFSET_DIVIDER = 2f;

    /// <summary>
    /// 奇数大小偏移除数
    /// </summary>
    private const float ODD_SIZE_OFFSET_DIVIDER = 2f;
    [ShowInInspector]
    public Dictionary<Vector2Int, Map_Data> ReadLoadMapData = new Dictionary<Vector2Int, Map_Data>(); // 所有地图数据字典
    [ShowInInspector]
    public Dictionary<string, ChunkData> unloadedChunksData_Dic = new Dictionary<string, ChunkData>(); // 未加载的区块数据字典
    public Dictionary<string, Chunk> loadedChunks_Dic = new Dictionary<string, Chunk>(); // 已加载的区块字典
    #endregion

    #region 属性
    /// <summary>
    /// 获取区块大小
    /// </summary>
    public Vector2Int ChunkSize
    {
        get => TheWorld.Instance.WorldDataJson.ChunkSize;
    }
    #endregion

    #region 公共方法
    [Button("获取区块数据")]
    public ChunkData GetChunkData_ByBlockPosition(Vector3 blockPosition)
    {
        string chunkDataName = GetChunkDataNameByBlockPosition(blockPosition);
        Debug.Log("GetChunkData_ByBlockPosition chunkDataName:" + chunkDataName);
        return GetChunkDataByName(chunkDataName);
    }
    [Button("获取区块")]
    public Chunk GetChunk_ByBlockPosition(Vector3 blockPosition)
    {
        string chunkDataName = GetChunkDataNameByBlockPosition(blockPosition);
        Debug.Log("GetChunk_ByBlockPosition chunkDataName:" + chunkDataName);
        return loadedChunks_Dic[chunkDataName];
    }
    public string GetChunkDataNameByBlockPosition(Vector3 position)
    {
        var (chunkX, chunkY) = CalculateChunkCoordinates(position); // 计算区块坐标
        return string.Format(CHUNK_NAME_FORMAT, chunkX * ChunkSize.x, chunkY * ChunkSize.y); // 返回区块名称
    }
    private (int, int) CalculateChunkCoordinates(Vector3 position)
    {
        var chunkSize = ChunkSize; // 获取区块大小
        bool isEvenSize = chunkSize.x % 2 == 0 && chunkSize.y % 2 == 0; // 判断区块大小是否为偶数

        float offsetX = isEvenSize ? chunkSize.x / EVEN_SIZE_OFFSET_DIVIDER : (chunkSize.x / ODD_SIZE_OFFSET_DIVIDER) - 1; // 计算X轴偏移
        float offsetY = isEvenSize ? chunkSize.y / EVEN_SIZE_OFFSET_DIVIDER : (chunkSize.y / ODD_SIZE_OFFSET_DIVIDER) - 1; // 计算Y轴偏移

        float adjustedX = position.x + (position.x < 0 ? -offsetX : offsetX); // 调整X轴位置
        float adjustedY = position.y + (position.y < 0 ? -offsetY : offsetY); // 调整Y轴位置

        return (
            Mathf.FloorToInt(adjustedX / chunkSize.x), // 计算区块X坐标
            Mathf.FloorToInt(adjustedY / chunkSize.y)  // 计算区块Y坐标
        );
    }
    [Button("清空地图数据")]
    public void Clear()
    {
        unloadedChunksData_Dic.Clear();
        ReadLoadMapData.Clear();
    }

    public void Clear_ReadLoadMapData()
    {
        ReadLoadMapData.Clear();
    }

    public void Clear_UnloadedChunksData_Dic()
    {
        unloadedChunksData_Dic.Clear();
    }

    public Vector2Int AddMapDataToUnLoadDic()
    {
        foreach (var mapData in ReadLoadMapData.Values)
        {
            PushAllChunkDataToUnloadedChunksData_Dic(mapData);
        }
        return Vector2Int.zero;
    }

    public void PushAllChunkDataToUnloadedChunksData_Dic(Map_Data mapData)
    {
        foreach (var chunkData in mapData.AllChunksData_Dic)
        {
            if (!unloadedChunksData_Dic.ContainsKey(chunkData.Key))
            {
                unloadedChunksData_Dic[chunkData.Key] = chunkData.Value;
            }
        }
    }

    public ChunkData GetChunkDataByName(string chunkName)
    {
        return unloadedChunksData_Dic.ContainsKey(chunkName) ? unloadedChunksData_Dic[chunkName] : null;
    }

    public void AddMapData(Map_Data mapData)
    {
        if (mapData == null)
        {
            Debug.LogError("要添加的地图数据为空");
            return;
        }
        if (ReadLoadMapData == null)
        {
            Debug.LogError("ReadLoadMapData 为空");
            return;
        }
        ReadLoadMapData[mapData.MapCenter] = mapData;
        Debug.Log($"添加地图数据到 ReadLoadMapData，中心点：{mapData.MapCenter}");
    }

    public void SaveMapToTheWorld(bool Gzip_Use)
    {
        foreach (var mapData in ReadLoadMapData.Values)
        {
            TheWorld.Instance.WorldDataJson.Save_OneMapData_To_TheWorldData(mapData, Gzip_Use);
        }
        Debug.Log($"保存了 {ReadLoadMapData.Count} 张地图块数据到Json文件");
    }
    #endregion

    #region 区块加载相关方法
    public void AddLoadedChunk(string chunkName, Chunk chunk)
    {
        if (!loadedChunks_Dic.ContainsKey(chunkName))
        {
            loadedChunks_Dic.Add(chunkName, chunk);
        }
    }

    public void RemoveLoadedChunk(string chunkName)
    {
        loadedChunks_Dic.Remove(chunkName);
    }
    #endregion
}
