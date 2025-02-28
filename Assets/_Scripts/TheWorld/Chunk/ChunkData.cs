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
        //将传入的ItemData添加到itemDatas列表中
        itemDatas.Add(itemData);
    }
}

public enum ChunkType: byte
{
    Grassland,   // 草地
    Desert,      // 沙漠
    Forest,      // 森林
    Mountain,    // 山地
    Water,       // 水域
    Swamp,       // 沼泽
    Snow,        // 雪地
    Lava         // 熔岩
}