using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MemoryPackable(SerializeLayout.Explicit)]
[System.Serializable]
public partial class Map_Data
{
    [MemoryPackOrder(0)]
    public string MapName;//��ͼ����

    [MemoryPackOrder(1)]
    public string CreateMapDate;//������ͼ����

    [MemoryPackOrder(2)]
    public string LastEditDate;//���༭����

    [MemoryPackOrder(3)]
    public Vector2Int MapCenter = new Vector2Int(0, 0);//��ͼ���ĵ�

    [MemoryPackOrder(4)]
    [ShowInInspector]
    public Dictionary<string, ChunkData> AllChunksData_Dic = new Dictionary<string, ChunkData>(); // �洢��ͼ�е�����δ��������
   
    [MemoryPackOrder(5)]
    [ShowInInspector]
    public int ChunkDataCount
    {
        get
        {
            if (AllChunksData_Dic == null)
            {
                return 0;
            }
            return  AllChunksData_Dic.Count;
        }
    }
  
    [MemoryPackOrder(6)]
    [FoldoutGroup ("��ͼĬ������"), ReadOnly]
    public Vector2Int chunkSize = new Vector2Int(25, 25);//ÿ������Ĵ�С
  
    [MemoryPackOrder(7)]
    [FoldoutGroup("��ͼĬ������"), ReadOnly]
    public Vector2Int MapSize = new Vector2Int(500, 500);//��ͼ��С

    [MemoryPackConstructor]
    /// <summary>
    /// ��ʼ����ͼ����
    /// </summary>
    /// <param name="mapName">����</param>
    /// <param name="chunkSize">�����С</param>
    /// <param name="mapCenter">��ͼ���ĵ�</param>
    /// <param name="mapSize">��ͼ���</param>
    public Map_Data(string mapName, Vector2Int chunkSize , Vector2Int mapCenter, Vector2Int mapSize)
    {
        MapName = mapName;
        this.chunkSize = chunkSize;
        MapCenter = mapCenter;
        MapSize = mapSize;
        CreateMapDate = System.DateTime.Now.ToString();
    }


}
