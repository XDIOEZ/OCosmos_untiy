using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图数据生成器类，用于生成和管理地图数据
/// </summary>
public class MapDataGenerator : MonoBehaviour
{
    #region 地图生成设置

    [Header("地图生成设置")]
    [Title("世界名字")]
    public string worldName = "TheWorld";

    [Title("地图起始中心位置")]
    public Vector2Int mapDataStartCenterPosition = new Vector2Int(200, 200);

    [Title("地图终止中心位置")]
    public Vector2Int mapDataEndCenterPosition = new Vector2Int(600, 600);

    [Title("地图块的大小")]
    public Vector2Int MapTileSize = new Vector2Int(400, 400);

    [Title("区块的大小")]
    public Vector2Int chunkSize = new Vector2Int(25, 25);
[Title("是否压缩世界存档")]
    public bool useGzip = false;

    TheWorldData NewCreateWorldData;



    #endregion

    #region 地图数据

    [Title("存储生成的地图数据")]
    public List<Map_Data> generatedMaps = new List<Map_Data>();

    [Title("生成器设置列表")]
    public List<MapMaker> mapMakers = new List<MapMaker>();

    #endregion

    private void Start()
    {
        // 初始化地图生成器列表
        mapMakers.Add(new MapMaker());
    }

    #region 地图生成函数

    /// <summary>
    /// 初始化地图数据列表
    /// </summary>
    public void InitializeMapDataList()
    {
        generatedMaps = GenerateMapDataList(MapTileSize, chunkSize, mapDataStartCenterPosition, mapDataEndCenterPosition);
    }

    /// <summary>
    /// 生成空的地图数据列表
    /// </summary>
    /// <param name="mapTileSize_">地图尺寸</param>
    /// <param name="chunkSize">区块尺寸</param>
    /// <param name="startPosition">起始中心位置</param>
    /// <param name="endPosition">结束中心位置</param>
    /// <returns>返回生成的地图数据列表</returns>
    public List<Map_Data> GenerateMapDataList(Vector2Int mapTileSize_, Vector2Int chunkSize, Vector2Int startPosition, Vector2Int endPosition)
    {
        List<Map_Data> empty_MapData_List = new List<Map_Data>();

        startPosition += new Vector2Int(mapTileSize_.x/2,mapTileSize_.y/2);
        endPosition -= new Vector2Int(mapTileSize_.x / 2, mapTileSize_.y / 2);


        // 计算世界尺寸
        Vector2Int worldSize = (endPosition + mapTileSize_ / 2) - (startPosition - mapTileSize_ / 2);

        // 计算地图数据的数量
        int MapDataCountX = Mathf.CeilToInt((float)worldSize.x / mapTileSize_.x);
        int MapDataCountY = Mathf.CeilToInt((float)worldSize.y / mapTileSize_.y);

        // 遍历生成地图数据
        for (int y = (startPosition.y + (mapTileSize_.y / 2)) / mapTileSize_.y; y < MapDataCountY + (startPosition.y + (mapTileSize_.y / 2)) / mapTileSize_.y; y++)
        {
            for (int x = (startPosition.x + (mapTileSize_.x / 2)) / mapTileSize_.x; x < MapDataCountX + (startPosition.x + (mapTileSize_.x / 2)) / mapTileSize_.x; x++)
            {
                Vector2Int mapCenter = new Vector2Int(
                    startPosition.x + (x - (startPosition.x + (mapTileSize_.x / 2)) / mapTileSize_.x) * mapTileSize_.x,
                    startPosition.y + (y - (startPosition.y + (mapTileSize_.y / 2)) / mapTileSize_.y) * mapTileSize_.y);

                char yChar = (char)('A' + (TheWorld.Instance.WorldDataJson.MapDataCount.y - y));
                Map_Data mapData = new Map_Data($"{yChar}{x}", chunkSize, mapCenter, mapTileSize_);
                empty_MapData_List.Add(mapData);
            }
        }
        return empty_MapData_List;
    }

    /// <summary>
    /// 根据地图数据列表通过地图生成器创建地图
    /// </summary>
    /// <param name="mapDataList">地图数据列表</param>
    public void CreateMapsFromData(List<Map_Data> mapDataList)
    {
        foreach (var mapData in mapDataList)
        {
            mapMakers[0].GenerateMap(mapData);
        }
    }

    #endregion

    #region 地图数据传输

    //发送地图数据到 世界数据 覆盖原有数据
    [Button("创建世界(覆盖)")]
    public void CreateWorld()
    {
        InitializeMapDataList();

        CreateMapsFromData(generatedMaps);



        foreach (var mapData in generatedMaps)
        {
            NewCreateWorldData.Save_OneMapData_To_TheWorldData(mapData, useGzip);
        }

        NewCreateWorldData.SetTheWorldDataName(worldName);

        NewCreateWorldData.MapTileSize = MapTileSize;

        NewCreateWorldData.WorldSize = (mapDataEndCenterPosition + MapTileSize / 2) - (mapDataStartCenterPosition - MapTileSize / 2);

        NewCreateWorldData.IsZip = useGzip;

        NewCreateWorldData.ChunkSize = chunkSize;

        TheWorld.Instance.WorldDataJson = NewCreateWorldData;//更新世界数据为
    }
    [Button]
    public void FixWorld()
    {

        InitializeMapDataList();

        CreateMapsFromData(generatedMaps);

        foreach (var mapData in generatedMaps)
        {
            TheWorld.Instance.WorldDataJson.Save_OneMapData_To_TheWorldData(mapData, TheWorld.Instance.WorldDataJson.IsZip);
        }
    }
    #endregion
}
