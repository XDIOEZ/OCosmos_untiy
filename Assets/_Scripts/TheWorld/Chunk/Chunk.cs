using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

// Chunk 类用于管理区块的相关操作，包括方块和物品的实例化、保存等 
public class Chunk : MonoBehaviour
{
    #region 字段 (按复杂度升序排列)
    // 调试模式开关 
    public bool isDebug = true;
    // 每帧实例化上限 
    public int blocksPerFrame;
    // 实例化间隔时间 
    public float instantiateDelay = 0.05f;
    // 区块尺寸配置 
    public Vector2Int chunkSize = new Vector2Int(25, 25);
    // 核心区块数据 
    public ChunkData chunkData;
    // 方块实例容器 
    public List<GameObject> Blocks_GameObject = new List<GameObject>();
    // 物品实例容器 
    public List<Item> Items_GameObject = new List<Item>();
    // 方块父节点 
    public Transform BlockParent;
    // 物品父节点 
    public Transform ItemParent;
    //方块字典
    [ShowInInspector]
    public Dictionary<Vector3, BlockData> blockDict = new Dictionary<Vector3, BlockData>();
    #endregion 

    #region Unity 生命周期方法 
    // 当 Chunk 对象禁用时调用，用于保存物品数据到区块数据 
    private void OnDisable()
    {
        SaveChunkItemToData();
    }
    #endregion

    #region 公共方法 

    //更新区块中所有方块的位置
    public void UpdateBlockPosition(Vector3 position)
    {
        
    }
    /// <summary> 
    /// 保存区块中的物品数据到区块数据 
    /// </summary> 
    public void SaveChunkItemToData()
    {
        // 遍历 Items_GameObject 将其中的为空的物体对应的索引在 chunkData.itemDatas  对应的相同索引处删除 
        Debug.Log("保存物品数据");
        for (int i = Items_GameObject.Count - 1; i >= 0; i--)
        {
            if (Items_GameObject[i] == null)
            {
                Items_GameObject.RemoveAt(i);
                chunkData.itemDatas[i] = null;
                chunkData.itemDatas.RemoveAt(i);
            }
        }

        // 遍历 Items_GameObject 将其保存到 chunkData.itemDatas  
        foreach (Item item in Items_GameObject)
        {
            SaveItemTochunkData(item, chunkData);
        }
    }
    /// <summary> 
    /// 向物品列表中添加物品，并设置其父物体 
    /// </summary> 
    /// <param name="item_add">要添加的物品</param> 
    public void AddItem(Item item_add)
    {
        Items_GameObject.Add(item_add);
        item_add.transform.SetParent(ItemParent);
    }

    /// <summary> 
    /// 初始化 Chunk，设置位置并启动方块实例化协程，加载物品 
    /// </summary> 
    /// <param name="chunkData">区块数据</param> 
    public void Initialize(ChunkData chunkData)
    {
        this.chunkData = chunkData;

        // 设置 Chunk 的位置为 chunkData 的 ChunkSize.x 和 ChunkSize.y 值 
        transform.localPosition = new Vector3(chunkData.chunkPosition.x, chunkData.chunkPosition.y, transform.position.z);  // 使用 Z 轴的当前值 

        // 启动协程实例化方块 
        StartCoroutine(InstantiateBlocksCoroutine());
        LoadChunks_Item(chunkData);
    }

    /// <summary> 
    /// 将物品保存到 Chunk 的物品列表中，并设置其父物体 
    /// </summary> 
    /// <param name="item">要保存的物品</param> 
    public void SaveItemTOChunk(Item item)
    {
        Items_GameObject.Add(item);
        item.transform.SetParent(ItemParent);
    }

    /// <summary> 
    /// 将物品数据保存到区块数据的物品数据列表中 
    /// </summary> 
    /// <param name="itemData">要保存的物品数据</param> 
    public void SaveItemDataTOChunkData(ItemData itemData)
    {
        chunkData.itemDatas.Add(itemData);
    }
    #endregion

    #region 私有方法 
    /// <summary> 
    /// 协程实例化方块，控制每帧实例化的方块数量 
    /// </summary> 
    /// <returns></returns> 
    private IEnumerator InstantiateBlocksCoroutine()
    {
        if (isDebug) Debug.Log($"开始实例化方块，共有 {chunkData.blockDatas.Count}  个方块需要处理。");

        // 偏移量，用于确保方块以中心对齐 
        float offsetX = (chunkSize.x % 2 == 0) ? chunkSize.x * 0.5f - 0.5f : chunkSize.x * 0.5f;
        float offsetY = (chunkSize.y % 2 == 0) ? chunkSize.y * 0.5f - 0.5f : chunkSize.y * 0.5f;

        int blocksInstantiatedThisFrame = 0; // 用来追踪当前帧实例化的方块数量 

        for (int i = 0; i < chunkData.blockDatas.Count; i++)
        {
            BlockData block = chunkData.blockDatas[i];

            blockDict[block.position] = chunkData.blockDatas[i];

            // 使用 chunkData 的 ChunkSize.x 和 ChunkSize.y 来计算方块的位置，并添加偏移量 
            if (block.name == "air"||block.name == "")
            {
                chunkData.blockDatas.RemoveAt(i);
                continue;
            }
            Vector3 blockPosition;

            #region 位置处理

            if (block.position.x == 0 && block.position.y == 0)
            {
                blockPosition = new Vector3(
               0 + (i % chunkSize.x) * 1 - offsetX,
               0 + (i / chunkSize.x) * 1 - offsetY,
               10
              );
                block.position.Set((int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z);
            }
            else
            {
                blockPosition = block.position;
            }
            #endregion


            string prefabName = block.name;

            if (isDebug)
            {
                Debug.Log($"正在处理第 {i + 1} 个方块，名称: {prefabName}, 位置: {blockPosition}");
            }

            //实例化方块
            if (GameResManager.Instance.AllPrefabs[prefabName] is GameObject blockPrefab)
            {
                // 实例化方块 
                GameObject blockObject = Instantiate(blockPrefab);

                blockObject.transform.parent = BlockParent; // 将方块设置为 Chunk 的子对象 

                blockObject.transform.position = blockPosition;

                Blocks_GameObject.Add(blockObject);

                if (isDebug) Debug.Log($"方块 {prefabName} 实例化成功，位置: {blockPosition}");
            }
            else
            {
                if (isDebug) Debug.LogWarning($"未在资源中找到方块预制体: {prefabName}");
            }

            blocksInstantiatedThisFrame++;

            // 每帧实例化 blocksPerFrame 个方块 
            if (blocksInstantiatedThisFrame >= blocksPerFrame)
            {
                blocksInstantiatedThisFrame = 0; // 重置计数器 
                yield return null; // 暂停协程，返回 Unity 引擎控制权，等待下一帧 
            }
        }

        if (isDebug) Debug.Log($"方块实例化完成，总共实例化了 {Blocks_GameObject.Count} 个方块。");
    }

    /// <summary> 
    /// 加载区块中的物品 
    /// </summary> 
    /// <param name="chunkData_">区块数据</param> 
    private void LoadChunks_Item(ChunkData chunkData_)
    {
        // 检测 chunkData_.itemDatas 是否为空 
        if (chunkData_.itemDatas == null)
        {
            Debug.LogWarning("chunkData_.itemDatas 为空");
            chunkData_.itemDatas = new List<ItemData>();
            return;
        }

        // 遍历 chunkData_.itemDatas 
        foreach (ItemData itemData in chunkData_.itemDatas)
        {
          //  Debug.Log(itemData.ToString());
            XDTool.InstantiateAddressableAsync(
                itemData.PrefabPath,
                itemData.Position,
                itemData.Rotation,
                (item_GameObject) =>
                {
                    item_GameObject.transform.SetParent(ItemParent);
                    item_GameObject.GetComponent<Item>().Item_Data = itemData;  
                    Items_GameObject.Add(item_GameObject.GetComponent<Item>());
                    item_GameObject.name = itemData.Name;
                    item_GameObject.transform.localScale = itemData.Scale;
                    item_GameObject.SetActive(true);
                },
                (error) =>
                {
                    Debug.LogError($"实例化失败: {itemData.PrefabPath}, 错误信息: {error.Message}");
                }
            );
        }
    }



    /// <summary> 
    /// 将物品保存到区块数据中 
    /// </summary> 
    /// <param name="item">要保存的物品</param> 
    /// <param name="chunkdata_">区块数据</param> 
    private void SaveItemTochunkData(Item item, ChunkData chunkdata_)
    {
        try
        {
            if (item == null)
            {
                return;
            }

            Quaternion validatedRotation = item.transform.rotation;
            // 设置物品缩放 
            Vector3 validatedScale = item.transform.localScale;
            // 设置物品坐标（考虑坐标系转换） 
            Vector3 validatedPosition = new Vector3(
                Mathf.Round(item.transform.position.x * 100) / 100f,
                Mathf.Round(item.transform.position.y * 100) / 100f,
                Mathf.Round(item.transform.position.z * 100) / 100f
            );
            item.Item_Data.SetTransformValue(validatedPosition, validatedRotation, validatedScale);

            // 防止重复添加的二次验证 
            if (!chunkdata_.itemDatas.Any(x => x.Guid == item.Item_Data.Guid))
            {
                chunkdata_.itemDatas.Add(item.Item_Data);
                Debug.Log($"添加 {item.name}   到 {chunkdata_.chunkName}  (GUID:{item.Item_Data.Guid})");
            }
            else
            {
                Debug.LogWarning($"重复物品 {item.name}   已跳过");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"处理物品 {item.name}   时发生错误: {e.Message}");
        }
    }
    #endregion
}