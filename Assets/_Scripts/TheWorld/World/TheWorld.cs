using JetBrains.Annotations;
using MemoryPack;
using NaughtyAttributes;
using Sirenix.OdinInspector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TheWorld : SingletonMono<TheWorld>
{

    public TheWorldData worldDataJson; // 地图数据
    public TheWorld_ChunkDataManager map_Dic = new TheWorld_ChunkDataManager(); // 地图字典
  /*  [Header("地图加载器对象")]*/
    [Title("地图加载器位置")]
    public List<Transform> LoaderPosition;
    [ShowInInspector]
    public Transform MapBlockParent; // 地图块父物体

    #region Unity生命周期函数

    public void Start()
    {
    }
    private void Update()
    {
        UpdateDebugData();
    }

    public void FixedUpdate()
    {
        GetNearbyMapData();
    }
    #endregion

    #region 公共方法

    /// <summary>
    /// 测试方法
    /// </summary>
    public void GetNearbyMapData()
    {
        // 获取主玩家的实际位置
        Vector3 playerPosition = LoaderPosition[0].position;

        // 处理主玩家位置的地图块数据
        ProcessMapData(playerPosition);

        // 定义偏移量，用于生成虚拟玩家位置
        int offset = 50;

        // 生成四个虚拟玩家位置并处理地图块数据
        Vector3[] virtualPositions = new Vector3[]
        {
        playerPosition + new Vector3(offset, offset, 0),    // 右上
        playerPosition + new Vector3(-offset, offset, 0),   // 左上
        playerPosition + new Vector3(offset, -offset, 0),   // 右下
        playerPosition + new Vector3(-offset, -offset, 0)   // 左下
        };

        foreach (var virtualPosition in virtualPositions)
        {
            ProcessMapData(virtualPosition);
        }
    }

    private void ProcessMapData(Vector3 position)
    {
        // 根据位置计算地图块中心
        Vector2Int nearCenter = GetNearbyMapKey(position);

        // 检查字典中是否已存在地图数据
        if (map_Dic.ReadLoadMapData.TryGetValue(nearCenter, out var mapData))
        {
            if (mapData != null)
            {
                return;
            }
            else
            {
                Debug.Log("字典中有该键，但值为空，可能需要重新加载数据。");
            }
        }

        // 加载并保存地图数据
        Map_Data nearMapData = WorldDataJson.LoadMapDataByCenter(nearCenter);
        map_Dic.AddMapData(nearMapData);
        map_Dic.AddMapDataToUnLoadDic();
    }

    /// <summary>
    /// 获取距离玩家最近的 Map Key
    /// </summary>
    /// <returns>最近的地图中心</returns>
    public Vector2Int GetNearbyMapKey()
    {
        GameObject player = GameObject.FindWithTag("CanvasBelong_Item");
        Vector3 playerPosition = player.transform.position;

        foreach (var key in WorldDataJson.WorldData.Keys)
        {
            Vector2Int center = TryParseVector2Int(key);
            if (Vector2Int.Distance(center, new Vector2Int(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y))) <= TheWorld.Instance.WorldDataJson.MapTileSize.x)
            {
                return center;
            }
        }
        return Vector2Int.zero;
    }

    /// <summary>
    /// 根据玩家的当前位置，计算并返回玩家所在地图块的中心坐标。
    /// </summary>
    /// <param name="playerPosition">玩家的三维世界坐标。</param>
    /// <returns>返回玩家所在地图块中心的二维坐标。</returns>
    public Vector2Int GetNearbyMapKey(Vector3 playerPosition)
    {
        Vector2Int ppInt = new Vector2Int(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y));
        int XmapSize = TheWorld.Instance.WorldDataJson.MapTileSize.x;
        int YmapSize = TheWorld.Instance.WorldDataJson.MapTileSize.y;

        int xCount = Mathf.FloorToInt(ppInt.x / (float)XmapSize);
        int yCount = Mathf.FloorToInt(ppInt.y / (float)YmapSize);

        Vector2Int NearCenter = new Vector2Int(
            xCount * XmapSize + XmapSize / 2,
            yCount * YmapSize + YmapSize / 2
        );

        return NearCenter;
    }

    #endregion

    #region 类内工具
    private bool IsGzipCompressed(byte[] data)
    {
        return data.Length > 2 && data[0] == 0x1F && data[1] == 0x8B;
    }
    /// <summary>
    /// 并行序列化 Map_Data
    /// </summary>
    /// <param name="mapData">地图数据</param>
    /// <returns>序列化并压缩后的字节数组</returns>
    public static byte[] ParallelSerialize(Map_Data mapData)
    {
        float startTime = Time.realtimeSinceStartup;
        var dictionary = mapData.AllChunksData_Dic;
        ConcurrentDictionary<string, byte[]> serializedChunks = new ConcurrentDictionary<string, byte[]>();

        Parallel.ForEach(dictionary, kvp =>
        {
            string key = kvp.Key;
            byte[] valueBytes = MemoryPackSerializer.Serialize(kvp.Value);
            serializedChunks[key] = valueBytes;
        });

        byte[] mapDataBytes = MemoryPackSerializer.Serialize(serializedChunks);
        byte[] compressedMapDataBytes;
        try
        {
            compressedMapDataBytes = MapDataUtility.Compress(mapDataBytes);
        }
        catch (Exception ex)
        {
            Debug.LogError("压缩地图数据失败：" + ex.Message);
            return null;  // 发生压缩错误时返回 null
        }

        float endTime = Time.realtimeSinceStartup;
        Debug.Log("并行序列化和压缩完成耗时: " + ((endTime - startTime) * 1000) + " ms");

        return compressedMapDataBytes;
    }

    /// <summary>
    /// 将 Vector2Int 转换为字符串
    /// </summary>
    /// <param name="vector">向量</param>
    /// <returns>字符串形式的向量</returns>
    public static string Vector2IntToString(Vector2Int vector)
    {
        return "(" + vector.x + ", " + vector.y + ")";
    }

    /// <summary>
    /// 尝试将字符串解析为 Vector2Int
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>解析后的 Vector2Int</returns>
    private static Vector2Int TryParseVector2Int(string input)
    {
        Vector2Int result;

        if (string.IsNullOrEmpty(input) || !input.StartsWith("(") || !input.EndsWith(")"))
            return Vector2Int.zero;

        string content = input.Trim('(', ')');
        string[] parts = content.Split(',');

        if (parts.Length == 2 &&
            int.TryParse(parts[0].Trim(), out int x) &&
            int.TryParse(parts[1].Trim(), out int y))
        {
            result = new Vector2Int(x, y);
            return result;
        }

        return Vector2Int.zero;
    }
    #endregion

    #region 调试信息
    private void UpdateDebugData()
    {
        StringBuilder stringBuilder = new StringBuilder();
        int totalSize = 0; // 统计总字节大小

        stringBuilder.AppendLine("数据条目数: " + WorldDataJson.WorldData.Count);
        foreach (var kvp in WorldDataJson.WorldData)
        {
            int dataSize = kvp.Value.Length;
            totalSize += dataSize;

            if (dataSize >= 1024 * 1024) // 大于等于1MB
            {
                double dataSizeMB = dataSize / (1024.0 * 1024.0);
                stringBuilder.AppendLine("分块中心: " + kvp.Key + ", 数据所占存储大小: " + dataSizeMB.ToString("F2") + " MB");
            }
            else // 小于1MB
            {
                double dataSizeKB = dataSize / 1024.0;
                stringBuilder.AppendLine("分块中心: " + kvp.Key + ", 数据所占存储大小: " + dataSizeKB.ToString("F2") + " KB");
            }
        }

        if (totalSize >= 1024 * 1024) // 总大小大于等于1MB
        {
            double totalSizeMB = totalSize / (1024.0 * 1024.0);
            stringBuilder.AppendLine("总存储大小: " + totalSizeMB.ToString("F2") + " MB");
        }
        else // 总大小小于1MB
        {
            double totalSizeKB = totalSize / 1024.0;
            stringBuilder.AppendLine("总存储大小: " + totalSizeKB.ToString("F2") + " KB");
        }

        DebugData = stringBuilder.ToString();
    }

    [TextArea(10, 10)]
    public string DebugData; // 调试数据

    public TheWorldData WorldDataJson
    {
        get => worldDataJson; 
        set
        {
           /* XDTool.IsGzipCompressed(value);*/
            worldDataJson = value;
        }
    }
    #endregion
}
