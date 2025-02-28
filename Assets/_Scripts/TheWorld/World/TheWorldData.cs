using MemoryPack;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class TheWorldData
{
#region �ֶ�

    public string TheWorldName = ""; // �����ͼ����
    public Vector2Int WorldSize = new Vector2Int(8000, 4000); // �����ͼ��С
    public Vector2Int MapTileSize = new Vector2Int(400, 400); // ���ŵ�ͼ��С
    public Vector2Int ChunkSize = new Vector2Int(25, 25); // ��ͼ���С
    public bool IsZip = false; // �Ƿ�ʹ����Gzipѹ����ͼ����

    [ShowInInspector]
    private Dictionary<string, byte[]> worldData = new Dictionary<string, byte[]>(); // �洢�����ͼ���ݵ��ֵ�

    public byte[][,] WorldData_Array = new byte[2][,]; // �洢�����ͼ���ݵĶ�ά����

    #endregion

    #region ����

    /// <summary>
    /// ��ȡ�����ͼ�����ĵ�ͼ����
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

    #region ��ͼ���ݼ����뱣�淽��

    //���ݵĶ�ȡ����
    public Map_Data LoadMapDataByCenter(Vector2Int center)
    {
        //TODO :����Զ�����Ƿ���Ҫ��ѹ�Ĺ���,ͬʱ���û���������Ч��
        // ���ɷֿ���������ļ�ֵ
        string centerKey = TheWorld.Vector2IntToString(center);

        // ����ͼ�����ֵ����Ƿ���ڸü�ֵ
        if (!WorldData.ContainsKey(centerKey))
        {
            Debug.Log("��ͼ���ݲ�����);");
            return null;
        }

        try
        {
            byte[] mapDataBytes;
            byte[] compressedMapDataBytes = WorldData[centerKey];

            // �����Ƿ�ʹ��Gzip��ѹ����ͼ����
            if (IsZip)
            {
                mapDataBytes = MapDataUtility.Decompress(compressedMapDataBytes);
               // Debug.Log("��ͼ�����ѽ�ѹ");
            }
            else
            {
                mapDataBytes = compressedMapDataBytes;
                //Debug.Log("��ͼ����δѹ��");
            }

            // �����л���ͼ����
            Map_Data mapData = MemoryPackSerializer.Deserialize<Map_Data>(mapDataBytes);
            Debug.Log("��ͼ����: " + mapData.MapName);
            return mapData;
        }
        catch (Exception ex)
        {
            Debug.LogError("��ͼ���ݽ���ʧ�ܣ��ֿ�����: " + centerKey + ", ����: " + ex.Message);
            return null;
        }
    }



    //���ݵ�д�뷽��
    public bool Save_OneMapData_To_TheWorldData(Map_Data mapData, bool useGzip)
    {
        // ����ͼ�����Ƿ�Ϊ��
        if (mapData == null)
        {
            Debug.LogError("����ĵ�ͼ����Ϊ�գ�");
            return false;
        }

        // ���л���ͼ����
        byte[] mapDataBytes = MemoryPackSerializer.Serialize(mapData);

        // ������л����
        if (mapDataBytes == null)
        {
            return false;
        }

        // ���ɷֿ���������ļ�ֵ
        string centerKey = TheWorld.Vector2IntToString(mapData.MapCenter);

        // �����Ƿ�ʹ��Gzipѹ����ͼ����
        if (useGzip)
        {
            try
            {
                mapDataBytes = MapDataUtility.Compress(mapDataBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError("ѹ��ʧ�ܣ�" + ex.Message);
                return false;
            }
        }

        // ���»���ӵ�ͼ���ݵ��ֵ�
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
