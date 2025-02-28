using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
[MemoryPackable(SerializeLayout.Explicit)]
[System.Serializable]
public partial class ChunkData
{
    [MemoryPackOrder(0)]
    public string chunkName;
    [MemoryPackOrder(1)]
    public Vector2Int chunkPosition;
    [MemoryPackOrder(2)]
    public ChunkType chunkType;
    [MemoryPackOrder(3)]
    public List<BlockData> blockDatas;
    [MemoryPackOrder(4)]
    [ShowInInspector]
    public List<ItemData> itemDatas;

    [MemoryPackConstructor]
    public ChunkData(string chunkName, Vector2Int chunkPosition, ChunkType chunkType, List<BlockData> blockDatas, List<ItemData> itemDatas)
    {
        this.chunkName = chunkName;
        this.chunkPosition = chunkPosition;
        this.chunkType = chunkType;
        this.blockDatas = blockDatas;
        this.itemDatas = itemDatas;
    }

    public void AddItemData(ItemData itemData)
    {
        //�������ItemData��ӵ�itemDatas�б���
        itemDatas.Add(itemData);
    }
}

public enum ChunkType: byte
{
    Grassland,   // �ݵ�
    Desert,      // ɳĮ
    Forest,      // ɭ��
    Mountain,    // ɽ��
    Water,       // ˮ��
    Swamp,       // ����
    Snow,        // ѩ��
    Lava         // ����
}