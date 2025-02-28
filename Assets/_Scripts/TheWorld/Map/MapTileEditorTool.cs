using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapTileEditorTool : MonoBehaviour
#region 字段和属性
{
    /// <summary>
    /// 区块名称格式字符串
    /// </summary>
    private const string CHUNK_NAME_FORMAT = "Chunk_{0}_{1}";

    /// <summary>
    /// 偶数大小偏移除数
    /// </summary>
    private const float EVEN_SIZE_OFFSET_DIVIDER = 2f;

    /// <summary>
    /// 奇数大小偏移除数
    /// </summary>
    private const float ODD_SIZE_OFFSET_DIVIDER = 2f;

    [Header("场景引用")]
    /// <summary>
    /// 方块父对象
    /// </summary>
    public Transform BlockParent;

    /// <summary>
    /// 世界物品对象
    /// </summary>
    public Transform WorldItem;

    /// <summary>
    /// 场景物品管理器
    /// </summary>
    public SceneItemManager sceneItemManager;

    [Header("运行时数据")]
    /// <summary>
    /// 方块列表
    /// </summary>
    [ShowInInspector] private List<Block> blockList = new List<Block>();

    /// <summary>
    /// 地图数据管理器
    /// </summary>
    private TheWorld_ChunkDataManager MapDic => TheWorld.Instance.map_Dic;
    #endregion

    #region 编辑器操作
    /// <summary>
    /// 保存地图修改并将地图数据保存到磁盘文件
    /// </summary>
    [Button("保存地图修改并将地图数据保存到磁盘文件")]
    public void SaveMapData()
    {
        if (!ValidateBlockParent()) return; // 验证BlockParent是否存在

        GetBlockDataToParentGameObject(); // 获取方块数据
        ClearWorldItems(); // 清理世界物品
        sceneItemManager.onSceneItemsSetToWorldItemManager.Invoke(); // 通知场景物品管理器

        MapDic.SaveMapToTheWorld(TheWorld.Instance.worldDataJson.IsZip); // 保存地图数据
        EventCenter.Instance.EventTrigger("更新地图"); // 触发地图更新事件
        Debug.Log("地图已更新");
    }

    /// <summary>
    /// 获取方块数据
    /// </summary>
    [Button("获取方块数据")]
    public void GetBlockData() => GetBlockDataToParentGameObject();

    /// <summary>
    /// 清理内存
    /// </summary>
    [Button("清理内存")]
    public void ClearMemory()
    {
        long memoryBefore = GC.GetTotalMemory(false); // 获取清理前的内存使用情况
        GC.Collect(); // 强制进行垃圾回收
        GC.WaitForPendingFinalizers(); // 等待所有终结器完成
        GC.Collect(); // 再次进行垃圾回收
        Debug.Log($"内存清理完成\n清理前: {memoryBefore / 1024} KB\n清理后: {GC.GetTotalMemory(true) / 1024} KB"); // 输出内存清理前后的情况
    }
    #endregion

    #region 核心逻辑
    /// <summary>
    /// 从父游戏对象获取方块数据
    /// </summary>
    private void GetBlockDataToParentGameObject()
    {
        if (BlockParent.childCount == 0) return; // 如果没有子对象则返回

        var childrenToDestroy = new List<Transform>(); // 需要销毁的子对象列表
        foreach (Transform child in BlockParent)
        {
            if (child.TryGetComponent<Block>(out var block))
            {
                blockList.Add(block); // 添加方块到列表
                childrenToDestroy.Add(child); // 添加子对象到销毁列表
                ProcessBlockData(block); // 处理方块数据
            }
        }

        CleanupChildren(childrenToDestroy); // 清理子对象
        Debug.Log($"成功添加 {blockList.Count} 个方块数据"); // 输出添加的方块数量
        blockList.Clear(); // 清空方块列表
    }

    /// <summary>
    /// 处理方块数据
    /// </summary>
    /// <param name="block">方块对象</param>
    private void ProcessBlockData(Block block)
    {
        var chunkData = GetChunkDataByBlockPosition(block.BlockData.position); // 获取区块数据
        if (chunkData != null)
        {
            chunkData.blockDatas.Add(block.BlockData); // 添加方块数据到区块
            Debug.Log($"添加到区块: {chunkData.chunkName}"); // 输出添加到的区块名称
        }
    }
    #endregion

    #region 区块计算
    /// <summary>
    /// 根据方块位置获取区块数据名称
    /// </summary>
    /// <param name="position">方块位置</param>
    /// <returns>区块数据名称</returns>
    public string GetChunkDataNameByBlockPosition(Vector3 position)
    {
        var (chunkX, chunkY) = CalculateChunkCoordinates(position); // 计算区块坐标
        return string.Format(CHUNK_NAME_FORMAT, chunkX * MapDic.ChunkSize.x, chunkY * MapDic.ChunkSize.y); // 返回区块名称
    }

    /// <summary>
    /// 计算区块坐标
    /// </summary>
    /// <param name="position">方块位置</param>
    /// <returns>区块坐标 (X, Y)</returns>
    private (int, int) CalculateChunkCoordinates(Vector3 position)
    {
        var chunkSize = MapDic.ChunkSize; // 获取区块大小
        bool isEvenSize = chunkSize.x % 2 == 0 && chunkSize.y % 2 == 0; // 判断区块大小是否为偶数

        float offsetX = isEvenSize ? chunkSize.x / EVEN_SIZE_OFFSET_DIVIDER : (chunkSize.x / ODD_SIZE_OFFSET_DIVIDER) - 1; // 计算X轴偏移
        float offsetY = isEvenSize ? chunkSize.y / EVEN_SIZE_OFFSET_DIVIDER : (chunkSize.y / ODD_SIZE_OFFSET_DIVIDER) - 1; // 计算Y轴偏移

        float adjustedX = position.x + (position.x < 0 ? -offsetX : offsetX); // 调整X轴位置
        float adjustedY = position.y + (position.y < 0 ? -offsetY : offsetY); // 调整Y轴位置

        return (
            Mathf.FloorToInt(adjustedX / chunkSize.x), // 计算区块X坐标
            Mathf.FloorToInt(adjustedY / chunkSize.y)  // 计算区块Y坐标
        );
    }
    #endregion

    #region 工具方法
    [Button("根据位置获取区块数据")]
    /// <summary>
    /// 根据位置获取区块数据
    /// </summary>
    /// <param name="position">方块位置</param>
    /// <returns>区块数据</returns>
    private ChunkData GetChunkDataByBlockPosition(Vector3 position) =>
        MapDic.GetChunkDataByName(GetChunkDataNameByBlockPosition(position));

    /// <summary>
    /// 验证BlockParent是否存在
    /// </summary>
    /// <returns>BlockParent是否存在</returns>
    private bool ValidateBlockParent()
    {
        if (BlockParent != null) return true; // 验证BlockParent是否存在
        Debug.LogError("未找到 BlockParent 对象"); // 输出错误信息
        return false;
    }

    /// <summary>
    /// 清理子对象
    /// </summary>
    /// <param name="children">需要销毁的子对象列表</param>
    private void CleanupChildren(List<Transform> children)
    {
        foreach (var child in children)
            DestroyImmediate(child.gameObject); // 立即销毁子对象
    }

    /// <summary>
    /// 清理世界物品
    /// </summary>
    private void ClearWorldItems()
    {
        //检查是否为空
        if (WorldItem == null) return;
        foreach (Transform child in WorldItem)
            Destroy(child.gameObject); // 销毁世界物品
    }
    #endregion
}
