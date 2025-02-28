using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UltEvents;

public class SceneItemManager : MonoBehaviour
{
    public List<GameObject> sceneItems_GameObject;
    [ShowInInspector]
    public Dictionary<string, List<Item>> sceneItems_Item_Dictionary; // 按 Item 类型存储

    public Transform WorldItemManager;

    public Chunk[] sceneChunks_List = null;
    [ShowInInspector]
    public Dictionary<ChunkData, Chunk> ChunkDataGetChunk_Dictionary = new Dictionary<ChunkData, Chunk>();

    public UltEvent onSceneItemsSetToWorldItemManager = new UltEvent();

    private void Start()
    {
        onSceneItemsSetToWorldItemManager+= SetSceneItemsToWorldItemManager;
        onSceneItemsSetToWorldItemManager += SetAllSceneItemsTOChunkData;
        onSceneItemsSetToWorldItemManager += SetItemDataToChunkData;
    }

    private void Update()
    {
    }
    #region 添加散落物品到场景物品管理器下

    [Button("设置场景物品到WorldItemManager")]
    public void SetSceneItemsToWorldItemManager() //TODO:输出耗时信息
    {
        if (WorldItemManager == null)
        {
            Debug.LogError("WorldItemManager 未指定！");
            return;
        }

        // 使用缓存避免重复查找
        Transform worldItemTransform = WorldItemManager.transform;

        // 获取所有无父物体的Item（单次遍历优化）
        var targetItems = GetRootItemsNonAlloc();

        // 分帧处理配置
        const int itemsPerFrame = 100; // 根据项目规模调整
        StartCoroutine(ProcessItemsCoroutine(targetItems, worldItemTransform, itemsPerFrame));
    }

    // 优化1：使用非分配方式获取物品
    private List<Item> GetRootItemsNonAlloc()
    {
        List<Item> results = new List<Item>();

#if UNITY_2021_1_OR_NEWER
        // 获取所有类型为 Item 的物体，包括非激活的物体
        Item[] items = FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
    // 获取所有激活的 Item 物体
    Item[] items = GameObject.FindObjectsOfType<Item>();
#endif

        // 遍历并筛选出没有父物体的物品
        foreach (var item in items)
        {
            Transform itemTransform = item.transform;
            if (itemTransform.parent == null && itemTransform != WorldItemManager.transform)
            {
                results.Add(item);
            }
        }

        return results;
    }


    // 优化2：分帧处理协程
    private IEnumerator ProcessItemsCoroutine(List<Item> items, Transform parent, int batchSize)
    {
        int processed = 0;
        int total = items.Count;
        var sw = System.Diagnostics.Stopwatch.StartNew();

        while (processed < total)
        {
            int endIndex = Mathf.Min(processed + batchSize, total);

            // 批量处理区间
            for (int i = processed; i < endIndex; i++)
            {
                Item item = items[i];
                if (item == null) continue;
                item.transform.SetParent(parent);
            }

            processed = endIndex;

            // 每批处理后报告进度，或者按时间间隔强制刷新
            if (sw.ElapsedMilliseconds > 200) // 超过200ms时强制刷新
            {
                Debug.Log($"处理进度: {processed}/{total} ({processed / (float)total:P0})");
                sw.Restart();
                yield return null; // 确保主线程响应
            }

            yield return null;
        }

        Debug.Log($"处理完成，共设置 {total} 个物品");
    }

    // 优化3：编辑器专用性能测试方法
#if UNITY_EDITOR
    [Button("性能测试模式")]
    private void PerformanceTest()
    {
        const int testIterations = 10;
        var sw = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < testIterations; i++)
        {
            SetSceneItemsToWorldItemManager();
        }

        Debug.Log($"平均耗时: {sw.ElapsedMilliseconds / testIterations}ms");
    }
#endif

    private string GetTypeDistributionReport()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var kvp in sceneItems_Item_Dictionary)
        {
            sb.AppendLine($"• {kvp.Key}: {kvp.Value.Count} 个");
        }
        return sb.ToString();
    }
    #endregion

    [Button("更新Chunk")]
    public void SetAllSceneItemsTOChunkData()
    {
        //获取子对象下的Chunk组件
        sceneChunks_List = GetComponentsInChildren<Chunk>(true);
        //遍历Chunk组件
        foreach (Chunk chunk in sceneChunks_List)
        {
            // 保存ChunkData到Chunk字典中
            ChunkDataGetChunk_Dictionary[chunk.chunkData] = chunk;
            // 保存ChunkItem数据到ChunkData中
            chunk.SaveChunkItemToData();
        }
    }
    [Button("获取sceneItems_Transform设置到对应的ChunkData中")]
    public void SetItemDataToChunkData()
    {
        // 初始化列表和字典
        if (sceneItems_GameObject == null)
            sceneItems_GameObject = new List<GameObject>();
        else
            sceneItems_GameObject.Clear();

        if (sceneItems_Item_Dictionary == null)
            sceneItems_Item_Dictionary = new Dictionary<string, List<Item>>();
        else
            sceneItems_Item_Dictionary.Clear();

        // 获取所有在 WorldItemManager 子对象下的 Item 组件（包括非激活的）
        Item[] itemsInScene = WorldItemManager.GetComponentsInChildren<Item>(true);

        foreach (Item item in itemsInScene)
        {
            GameObject obj = item.gameObject;
            string key = item.GetType().Name; // 用类型名称作为字典的 key

            // 添加到列表
            sceneItems_GameObject.Add(obj);

            // 如果字典中没有该类型，则创建新的 List<Item>
            if (!sceneItems_Item_Dictionary.ContainsKey(key))
            {
                sceneItems_Item_Dictionary[key] = new List<Item>();
            }
            sceneItems_Item_Dictionary[key].Add(item);
        }
        // 阶段1：预处理 - 清空所有相关chunkData的旧数据
        // -----------------------------------------------
        HashSet<ChunkData> processedChunks = new HashSet<ChunkData>();

        // 第一次遍历：收集所有涉及的chunkData并清空列表
        foreach (var kvp in sceneItems_Item_Dictionary)
        {
            foreach (var item in kvp.Value)
            {
                ChunkData chunData_ = TheWorld.Instance.map_Dic.GetChunkData_ByBlockPosition(item.transform.localPosition);
                // 确保每个chunk只清空一次
                if (processedChunks.Add(chunData_))
                {
                    // 安全清除（避免null引用）
                    chunData_?.itemDatas?.Clear();
                }
            }
        }

        // 阶段2：数据写入 - 重新填充chunkData
        // -----------------------------------------------
        foreach (var kvp in sceneItems_Item_Dictionary)
        {
            foreach (var item in kvp.Value)
            {
                // 有效性检查
                if (item == null || item.Item_Data == null) continue;

                ChunkData chunkData_ = TheWorld.Instance.map_Dic.GetChunkData_ByBlockPosition(item.transform.localPosition);
                if (chunkData_ == null )
                {
                    Debug.LogError($"物品 {item.name}  未找到有效区块!");
                    continue;
                }

                try
                {
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

                    // 设置物品旋转


                    // 防止重复添加的二次验证
                    if (!chunkData_.itemDatas.Any(x => x.Guid == item.Item_Data.Guid))
                    {
                        chunkData_.itemDatas.Add(item.Item_Data);

                        Chunk itemBelong = ChunkDataGetChunk_Dictionary[chunkData_];

                        itemBelong.AddItem(item);

                        Debug.Log($"添加 {item.name}  到 {chunkData_.chunkName}  (GUID:{item.Item_Data.Guid})");
                    }
                    else
                    {
                        Debug.LogWarning($"重复物品 {item.name}  已跳过");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"处理物品 {item.name}  时发生错误: {e.Message}");    
                }
            }
        }

        // 阶段3：后期验证
        // -----------------------------------------------
        Debug.Log($"处理完成，共更新 {processedChunks.Count} 个区块");
        foreach (var chunk in processedChunks)
        {
            if (chunk != null)
            {
                Debug.Log($"{chunk.chunkName} 当前物品数: {chunk.itemDatas.Count}");
            }
        }
    }
}
