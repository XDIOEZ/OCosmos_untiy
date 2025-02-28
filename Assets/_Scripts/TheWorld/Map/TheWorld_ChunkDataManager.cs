using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �����ͼ�����ݹ�����
/// </summary>
[System.Serializable]
public class TheWorld_ChunkDataManager
{
    #region �ֶ�
    private const string CHUNK_NAME_FORMAT = "Chunk_{0}_{1}";
    /// <summary>
    /// ż����Сƫ�Ƴ���
    /// </summary>
    private const float EVEN_SIZE_OFFSET_DIVIDER = 2f;

    /// <summary>
    /// ������Сƫ�Ƴ���
    /// </summary>
    private const float ODD_SIZE_OFFSET_DIVIDER = 2f;
    [ShowInInspector]
    public Dictionary<Vector2Int, Map_Data> ReadLoadMapData = new Dictionary<Vector2Int, Map_Data>(); // ���е�ͼ�����ֵ�
    [ShowInInspector]
    public Dictionary<string, ChunkData> unloadedChunksData_Dic = new Dictionary<string, ChunkData>(); // δ���ص����������ֵ�
    public Dictionary<string, Chunk> loadedChunks_Dic = new Dictionary<string, Chunk>(); // �Ѽ��ص������ֵ�
    #endregion

    #region ����
    /// <summary>
    /// ��ȡ�����С
    /// </summary>
    public Vector2Int ChunkSize
    {
        get => TheWorld.Instance.WorldDataJson.ChunkSize;
    }
    #endregion

    #region ��������
    [Button("��ȡ��������")]
    public ChunkData GetChunkData_ByBlockPosition(Vector3 blockPosition)
    {
        string chunkDataName = GetChunkDataNameByBlockPosition(blockPosition);
        Debug.Log("GetChunkData_ByBlockPosition chunkDataName:" + chunkDataName);
        return GetChunkDataByName(chunkDataName);
    }
    [Button("��ȡ����")]
    public Chunk GetChunk_ByBlockPosition(Vector3 blockPosition)
    {
        string chunkDataName = GetChunkDataNameByBlockPosition(blockPosition);
        Debug.Log("GetChunk_ByBlockPosition chunkDataName:" + chunkDataName);
        return loadedChunks_Dic[chunkDataName];
    }
    public string GetChunkDataNameByBlockPosition(Vector3 position)
    {
        var (chunkX, chunkY) = CalculateChunkCoordinates(position); // ������������
        return string.Format(CHUNK_NAME_FORMAT, chunkX * ChunkSize.x, chunkY * ChunkSize.y); // ������������
    }
    private (int, int) CalculateChunkCoordinates(Vector3 position)
    {
        var chunkSize = ChunkSize; // ��ȡ�����С
        bool isEvenSize = chunkSize.x % 2 == 0 && chunkSize.y % 2 == 0; // �ж������С�Ƿ�Ϊż��

        float offsetX = isEvenSize ? chunkSize.x / EVEN_SIZE_OFFSET_DIVIDER : (chunkSize.x / ODD_SIZE_OFFSET_DIVIDER) - 1; // ����X��ƫ��
        float offsetY = isEvenSize ? chunkSize.y / EVEN_SIZE_OFFSET_DIVIDER : (chunkSize.y / ODD_SIZE_OFFSET_DIVIDER) - 1; // ����Y��ƫ��

        float adjustedX = position.x + (position.x < 0 ? -offsetX : offsetX); // ����X��λ��
        float adjustedY = position.y + (position.y < 0 ? -offsetY : offsetY); // ����Y��λ��

        return (
            Mathf.FloorToInt(adjustedX / chunkSize.x), // ��������X����
            Mathf.FloorToInt(adjustedY / chunkSize.y)  // ��������Y����
        );
    }
    [Button("��յ�ͼ����")]
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
            Debug.LogError("Ҫ��ӵĵ�ͼ����Ϊ��");
            return;
        }
        if (ReadLoadMapData == null)
        {
            Debug.LogError("ReadLoadMapData Ϊ��");
            return;
        }
        ReadLoadMapData[mapData.MapCenter] = mapData;
        Debug.Log($"��ӵ�ͼ���ݵ� ReadLoadMapData�����ĵ㣺{mapData.MapCenter}");
    }

    public void SaveMapToTheWorld(bool Gzip_Use)
    {
        foreach (var mapData in ReadLoadMapData.Values)
        {
            TheWorld.Instance.WorldDataJson.Save_OneMapData_To_TheWorldData(mapData, Gzip_Use);
        }
        Debug.Log($"������ {ReadLoadMapData.Count} �ŵ�ͼ�����ݵ�Json�ļ�");
    }
    #endregion

    #region ���������ط���
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
