using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ͼ�����������࣬�������ɺ͹����ͼ����
/// </summary>
public class MapDataGenerator : MonoBehaviour
{
    #region ��ͼ��������

    [Header("��ͼ��������")]
    [Title("��������")]
    public string worldName = "TheWorld";

    [Title("��ͼ��ʼ����λ��")]
    public Vector2Int mapDataStartCenterPosition = new Vector2Int(200, 200);

    [Title("��ͼ��ֹ����λ��")]
    public Vector2Int mapDataEndCenterPosition = new Vector2Int(600, 600);

    [Title("��ͼ��Ĵ�С")]
    public Vector2Int MapTileSize = new Vector2Int(400, 400);

    [Title("����Ĵ�С")]
    public Vector2Int chunkSize = new Vector2Int(25, 25);
[Title("�Ƿ�ѹ������浵")]
    public bool useGzip = false;

    TheWorldData NewCreateWorldData;



    #endregion

    #region ��ͼ����

    [Title("�洢���ɵĵ�ͼ����")]
    public List<Map_Data> generatedMaps = new List<Map_Data>();

    [Title("�����������б�")]
    public List<MapMaker> mapMakers = new List<MapMaker>();

    #endregion

    private void Start()
    {
        // ��ʼ����ͼ�������б�
        mapMakers.Add(new MapMaker());
    }

    #region ��ͼ���ɺ���

    /// <summary>
    /// ��ʼ����ͼ�����б�
    /// </summary>
    public void InitializeMapDataList()
    {
        generatedMaps = GenerateMapDataList(MapTileSize, chunkSize, mapDataStartCenterPosition, mapDataEndCenterPosition);
    }

    /// <summary>
    /// ���ɿյĵ�ͼ�����б�
    /// </summary>
    /// <param name="mapTileSize_">��ͼ�ߴ�</param>
    /// <param name="chunkSize">����ߴ�</param>
    /// <param name="startPosition">��ʼ����λ��</param>
    /// <param name="endPosition">��������λ��</param>
    /// <returns>�������ɵĵ�ͼ�����б�</returns>
    public List<Map_Data> GenerateMapDataList(Vector2Int mapTileSize_, Vector2Int chunkSize, Vector2Int startPosition, Vector2Int endPosition)
    {
        List<Map_Data> empty_MapData_List = new List<Map_Data>();

        startPosition += new Vector2Int(mapTileSize_.x/2,mapTileSize_.y/2);
        endPosition -= new Vector2Int(mapTileSize_.x / 2, mapTileSize_.y / 2);


        // ��������ߴ�
        Vector2Int worldSize = (endPosition + mapTileSize_ / 2) - (startPosition - mapTileSize_ / 2);

        // �����ͼ���ݵ�����
        int MapDataCountX = Mathf.CeilToInt((float)worldSize.x / mapTileSize_.x);
        int MapDataCountY = Mathf.CeilToInt((float)worldSize.y / mapTileSize_.y);

        // �������ɵ�ͼ����
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
    /// ���ݵ�ͼ�����б�ͨ����ͼ������������ͼ
    /// </summary>
    /// <param name="mapDataList">��ͼ�����б�</param>
    public void CreateMapsFromData(List<Map_Data> mapDataList)
    {
        foreach (var mapData in mapDataList)
        {
            mapMakers[0].GenerateMap(mapData);
        }
    }

    #endregion

    #region ��ͼ���ݴ���

    //���͵�ͼ���ݵ� �������� ����ԭ������
    [Button("��������(����)")]
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

        TheWorld.Instance.WorldDataJson = NewCreateWorldData;//������������Ϊ
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
