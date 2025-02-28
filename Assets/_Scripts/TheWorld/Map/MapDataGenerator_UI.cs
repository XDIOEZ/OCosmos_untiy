using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapDataGenerator_UI : MonoBehaviour
{
    public UnityEvent onIntMapDataClick;
    public MapDataGenerator mapDataGenerator;
    public ImportPngToMap importPngToMap;
    public bool UseGzip = false;


    //创建五个public按钮字段
    public Button IntMapData;
    public Button CreateMapData;
    public Button ImportPng;
    public Button UpdateMapDataFromPlayer;
    public Button SendMapToTheWorld;

    void Start()
    {
        // 为每个按钮添加监听事件
        IntMapData.onClick.AddListener(OnIntMapDataClick);
        CreateMapData.onClick.AddListener(OnCreateMapDataClick);
        ImportPng.onClick.AddListener(OnImportPngClick);
        UpdateMapDataFromPlayer.onClick.AddListener(OnUpdateMapDataFromPlayerClick);
        SendMapToTheWorld.onClick.AddListener(OnSendMapToTheWorldClick);
    }

    // 按钮点击事件的回调函数
    void OnIntMapDataClick()
    {
        // 实现初始化地图数据的逻辑
        mapDataGenerator.InitializeMapDataList();
    }

    void OnCreateMapDataClick()
    {
        // 实现创建地图数据的逻辑
        mapDataGenerator.CreateMapsFromData(mapDataGenerator.generatedMaps);
    }

    void OnImportPngClick()
    {
        // 实现导入PNG的逻辑
        importPngToMap.Test();
    }

    void OnUpdateMapDataFromPlayerClick()
    {
        // 实现从玩家更新地图数据的逻辑
        TheWorld.Instance.GetNearbyMapData();
    }

    void OnSendMapToTheWorldClick()
    {
        // 实现发送地图到世界的逻辑
        //遍历generatedMaps
        //将其中的数据保存到TheWorld.Instance.worldDataJson.mapDataDicJson
        foreach (var map in mapDataGenerator.generatedMaps)
        {

        TheWorld.Instance.WorldDataJson.Save_OneMapData_To_TheWorldData(map, UseGzip);

        }
    }
}