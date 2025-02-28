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

    public TheWorldData worldDataJson; // ��ͼ����
    public TheWorld_ChunkDataManager map_Dic = new TheWorld_ChunkDataManager(); // ��ͼ�ֵ�
  /*  [Header("��ͼ����������")]*/
    [Title("��ͼ������λ��")]
    public List<Transform> LoaderPosition;
    [ShowInInspector]
    public Transform MapBlockParent; // ��ͼ�鸸����

    #region Unity�������ں���

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

    #region ��������

    /// <summary>
    /// ���Է���
    /// </summary>
    public void GetNearbyMapData()
    {
        // ��ȡ����ҵ�ʵ��λ��
        Vector3 playerPosition = LoaderPosition[0].position;

        // ���������λ�õĵ�ͼ������
        ProcessMapData(playerPosition);

        // ����ƫ���������������������λ��
        int offset = 50;

        // �����ĸ��������λ�ò������ͼ������
        Vector3[] virtualPositions = new Vector3[]
        {
        playerPosition + new Vector3(offset, offset, 0),    // ����
        playerPosition + new Vector3(-offset, offset, 0),   // ����
        playerPosition + new Vector3(offset, -offset, 0),   // ����
        playerPosition + new Vector3(-offset, -offset, 0)   // ����
        };

        foreach (var virtualPosition in virtualPositions)
        {
            ProcessMapData(virtualPosition);
        }
    }

    private void ProcessMapData(Vector3 position)
    {
        // ����λ�ü����ͼ������
        Vector2Int nearCenter = GetNearbyMapKey(position);

        // ����ֵ����Ƿ��Ѵ��ڵ�ͼ����
        if (map_Dic.ReadLoadMapData.TryGetValue(nearCenter, out var mapData))
        {
            if (mapData != null)
            {
                return;
            }
            else
            {
                Debug.Log("�ֵ����иü�����ֵΪ�գ�������Ҫ���¼������ݡ�");
            }
        }

        // ���ز������ͼ����
        Map_Data nearMapData = WorldDataJson.LoadMapDataByCenter(nearCenter);
        map_Dic.AddMapData(nearMapData);
        map_Dic.AddMapDataToUnLoadDic();
    }

    /// <summary>
    /// ��ȡ������������ Map Key
    /// </summary>
    /// <returns>����ĵ�ͼ����</returns>
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
    /// ������ҵĵ�ǰλ�ã����㲢����������ڵ�ͼ����������ꡣ
    /// </summary>
    /// <param name="playerPosition">��ҵ���ά�������ꡣ</param>
    /// <returns>����������ڵ�ͼ�����ĵĶ�ά���ꡣ</returns>
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

    #region ���ڹ���
    private bool IsGzipCompressed(byte[] data)
    {
        return data.Length > 2 && data[0] == 0x1F && data[1] == 0x8B;
    }
    /// <summary>
    /// �������л� Map_Data
    /// </summary>
    /// <param name="mapData">��ͼ����</param>
    /// <returns>���л���ѹ������ֽ�����</returns>
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
            Debug.LogError("ѹ����ͼ����ʧ�ܣ�" + ex.Message);
            return null;  // ����ѹ������ʱ���� null
        }

        float endTime = Time.realtimeSinceStartup;
        Debug.Log("�������л���ѹ����ɺ�ʱ: " + ((endTime - startTime) * 1000) + " ms");

        return compressedMapDataBytes;
    }

    /// <summary>
    /// �� Vector2Int ת��Ϊ�ַ���
    /// </summary>
    /// <param name="vector">����</param>
    /// <returns>�ַ�����ʽ������</returns>
    public static string Vector2IntToString(Vector2Int vector)
    {
        return "(" + vector.x + ", " + vector.y + ")";
    }

    /// <summary>
    /// ���Խ��ַ�������Ϊ Vector2Int
    /// </summary>
    /// <param name="input">�����ַ���</param>
    /// <returns>������� Vector2Int</returns>
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

    #region ������Ϣ
    private void UpdateDebugData()
    {
        StringBuilder stringBuilder = new StringBuilder();
        int totalSize = 0; // ͳ�����ֽڴ�С

        stringBuilder.AppendLine("������Ŀ��: " + WorldDataJson.WorldData.Count);
        foreach (var kvp in WorldDataJson.WorldData)
        {
            int dataSize = kvp.Value.Length;
            totalSize += dataSize;

            if (dataSize >= 1024 * 1024) // ���ڵ���1MB
            {
                double dataSizeMB = dataSize / (1024.0 * 1024.0);
                stringBuilder.AppendLine("�ֿ�����: " + kvp.Key + ", ������ռ�洢��С: " + dataSizeMB.ToString("F2") + " MB");
            }
            else // С��1MB
            {
                double dataSizeKB = dataSize / 1024.0;
                stringBuilder.AppendLine("�ֿ�����: " + kvp.Key + ", ������ռ�洢��С: " + dataSizeKB.ToString("F2") + " KB");
            }
        }

        if (totalSize >= 1024 * 1024) // �ܴ�С���ڵ���1MB
        {
            double totalSizeMB = totalSize / (1024.0 * 1024.0);
            stringBuilder.AppendLine("�ܴ洢��С: " + totalSizeMB.ToString("F2") + " MB");
        }
        else // �ܴ�СС��1MB
        {
            double totalSizeKB = totalSize / 1024.0;
            stringBuilder.AppendLine("�ܴ洢��С: " + totalSizeKB.ToString("F2") + " KB");
        }

        DebugData = stringBuilder.ToString();
    }

    [TextArea(10, 10)]
    public string DebugData; // ��������

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
