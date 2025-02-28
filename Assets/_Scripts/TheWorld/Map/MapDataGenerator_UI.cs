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


    //�������public��ť�ֶ�
    public Button IntMapData;
    public Button CreateMapData;
    public Button ImportPng;
    public Button UpdateMapDataFromPlayer;
    public Button SendMapToTheWorld;

    void Start()
    {
        // Ϊÿ����ť��Ӽ����¼�
        IntMapData.onClick.AddListener(OnIntMapDataClick);
        CreateMapData.onClick.AddListener(OnCreateMapDataClick);
        ImportPng.onClick.AddListener(OnImportPngClick);
        UpdateMapDataFromPlayer.onClick.AddListener(OnUpdateMapDataFromPlayerClick);
        SendMapToTheWorld.onClick.AddListener(OnSendMapToTheWorldClick);
    }

    // ��ť����¼��Ļص�����
    void OnIntMapDataClick()
    {
        // ʵ�ֳ�ʼ����ͼ���ݵ��߼�
        mapDataGenerator.InitializeMapDataList();
    }

    void OnCreateMapDataClick()
    {
        // ʵ�ִ�����ͼ���ݵ��߼�
        mapDataGenerator.CreateMapsFromData(mapDataGenerator.generatedMaps);
    }

    void OnImportPngClick()
    {
        // ʵ�ֵ���PNG���߼�
        importPngToMap.Test();
    }

    void OnUpdateMapDataFromPlayerClick()
    {
        // ʵ�ִ���Ҹ��µ�ͼ���ݵ��߼�
        TheWorld.Instance.GetNearbyMapData();
    }

    void OnSendMapToTheWorldClick()
    {
        // ʵ�ַ��͵�ͼ��������߼�
        //����generatedMaps
        //�����е����ݱ��浽TheWorld.Instance.worldDataJson.mapDataDicJson
        foreach (var map in mapDataGenerator.generatedMaps)
        {

        TheWorld.Instance.WorldDataJson.Save_OneMapData_To_TheWorldData(map, UseGzip);

        }
    }
}