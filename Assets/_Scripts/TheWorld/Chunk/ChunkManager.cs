using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ChunkManager : SingletonMono<ChunkManager>
{
    #region 字段

    // 滑块控制
    [Range(0, 1)]
    public float blocksLoadSpeed;

    // 玩家对象，用于获取玩家位置
    public List<ChunkLoader> loadingTargets;

    // 预制的区块对象
    public GameObject chunkPrefab;

    // 预制的区块组件
    public Chunk chunkPrefabComponent;

    // 控制调试信息开关
    public bool isDebugging = true;

    // 在Inspector中拖拽对应的Map对象
    public GameObject map_parent;

    // 在Inspector中拖拽对应的Item对象
    public GameObject item_parent;

    // 等待加载的区块数据名称列表
    public List<string> WaitLodeChunkDataName_List;

    // 地图数据管理器
    public TheWorld_ChunkDataManager _Map
    {
        get { return TheWorld.Instance.map_Dic; }
    }

    #endregion

    #region 属性

    [ShowInInspector]
    public float BlocksLoadSpeed
    {
        get
        {
            chunkPrefabComponent.blocksPerFrame = (int)(_Map.ChunkSize.x * _Map.ChunkSize.y * blocksLoadSpeed);
            return blocksLoadSpeed;
        }
        set
        {
            chunkPrefabComponent.blocksPerFrame = (int)(_Map.ChunkSize.x * _Map.ChunkSize.y * value);
            blocksLoadSpeed = value;
        }
    }

    #endregion

    #region Unity方法

    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener("更新地图", ForceUpdateChunks);
        chunkPrefabComponent = chunkPrefab.GetComponent<Chunk>();
        chunkPrefabComponent.chunkSize = TheWorld.Instance.worldDataJson.ChunkSize;
        chunkPrefabComponent.blocksPerFrame = (int)(_Map.ChunkSize.x * _Map.ChunkSize.y * BlocksLoadSpeed);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener("更新地图", ForceUpdateChunks);
    }

    private void FixedUpdate()
    {
        if (_Map != null)
        {
            WaitLodeChunkDataName_List = GetChunksNameInLoadDistance(loadingTargets);
            UpdateTargetsAroundChunks(WaitLodeChunkDataName_List, loadingTargets);
        }
        else
        {
            if (isDebugging)
            {
                Debug.LogWarning($"_Map.unloadedChunksData_Dic 为 null，无法更新区块。");
            }
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 获取加载目标
    /// </summary>
    /// <param name="target">加载目标对象</param>
    public void GetLoadingTargets(ChunkLoader target)
    {
        if (!loadingTargets.Contains(target))
        {
            loadingTargets.Add(target);
        }
    }

    /// <summary>
    /// 强制刷新地图区块
    /// </summary>
    [Button("强制刷新地图区块")]
    public void ForceUpdateChunks()
    {
        WaitLodeChunkDataName_List = GetChunksNameInLoadDistance(loadingTargets);
        UnloadChunks(WaitLodeChunkDataName_List, _Map.loadedChunks_Dic);
        LoadChunks(WaitLodeChunkDataName_List, _Map.unloadedChunksData_Dic);
        List<string> chunksToUnload = _Map.loadedChunks_Dic.Keys.Except(WaitLodeChunkDataName_List).ToList();
        UnloadChunks(chunksToUnload, _Map.loadedChunks_Dic);
    }

    /// <summary>
    /// 销毁子对象
    /// </summary>
    [Button("销毁子对象")]
    public void DestroyChildren()
    {
        if (map_parent.transform.childCount > 0)
        {
            foreach (Transform child in map_parent.transform)
            {
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }
        else
        {
            Debug.Log("没有子对象，跳过销毁。");
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 根据加载器获取周围方块,然后加载和卸载区块的相关方法
    /// </summary>
    /// <param name="WaitLodeChunkDataName">等待加载的区块数据名称列表</param>
    /// <param name="loadingTargets">加载目标列表</param>
    private void UpdateTargetsAroundChunks(List<string> WaitLodeChunkDataName, List<ChunkLoader> loadingTargets)
    {
        LoadChunks(WaitLodeChunkDataName, _Map.unloadedChunksData_Dic);
        List<string> chunksToUnload = _Map.loadedChunks_Dic.Keys.Except(WaitLodeChunkDataName).ToList();
        UnloadChunks(chunksToUnload, _Map.loadedChunks_Dic);
    }

    /// <summary>
    /// 获取加载范围内的区块名称
    /// </summary>
    /// <param name="loadingTargets">加载目标列表</param>
    /// <returns>加载范围内的区块名称列表</returns>
    private List<string> GetChunksNameInLoadDistance(List<ChunkLoader> loadingTargets)
    {
        HashSet<string> chunksInRange = new HashSet<string>();

        if (_Map.ChunkSize.x == 0 || _Map.ChunkSize.y == 0)
        {
            Debug.LogWarning("地图尺寸未设置，无法计算加载范围。");
            return new List<string>();
        }

        foreach (var loadingTarget in loadingTargets)
        {
            Vector3 position = loadingTarget.transform.position;
            float width = loadingTarget.loadingTargetData.loadingAreaWidth;
            float height = loadingTarget.loadingTargetData.loadingAreaHeight;

            if (Mathf.Abs(position.x) <= loadingTarget.loadingTargetData.loadingAreaWidth / 2)
            {
                position -= new Vector3(_Map.ChunkSize.x / 2, 0, 0);
                width = loadingTarget.loadingTargetData.loadingAreaWidth + _Map.ChunkSize.x;
            }
            else if (position.x <= -loadingTarget.loadingTargetData.loadingAreaWidth / 2)
            {
                position -= new Vector3(_Map.ChunkSize.x, 0, 0);
                width = loadingTarget.loadingTargetData.loadingAreaWidth;
            }

            if (Mathf.Abs(position.y) <= loadingTarget.loadingTargetData.loadingAreaHeight / 2)
            {
                position -= new Vector3(0, _Map.ChunkSize.y / 2, 0);
                height = loadingTarget.loadingTargetData.loadingAreaHeight + _Map.ChunkSize.y;
            }
            else if (position.y <= -loadingTarget.loadingTargetData.loadingAreaHeight / 2)
            {
                position -= new Vector3(0, _Map.ChunkSize.y, 0);
                height = loadingTarget.loadingTargetData.loadingAreaHeight;
            }

            float leftBoundary = position.x - width / 2;
            float rightBoundary = position.x + width / 2;
            float bottomBoundary = position.y - height / 2;
            float topBoundary = position.y + height / 2;

            int minX = (int)(leftBoundary / _Map.ChunkSize.x) * _Map.ChunkSize.x;
            int maxX = (int)(rightBoundary / _Map.ChunkSize.x) * _Map.ChunkSize.x;
            int minY = (int)(bottomBoundary / _Map.ChunkSize.y) * _Map.ChunkSize.y;
            int maxY = (int)(topBoundary / _Map.ChunkSize.y) * _Map.ChunkSize.y;

            for (int x = minX; x <= maxX; x += _Map.ChunkSize.x)
            {
                for (int y = minY; y <= maxY; y += _Map.ChunkSize.y)
                {
                    chunksInRange.Add($"Chunk_{x}_{y}");
                }
            }
        }

        return new List<string>(chunksInRange);
    }

    /// <summary>
    /// 卸载区块
    /// </summary>
    /// <param name="chunkNames">区块名称列表</param>
    /// <param name="ChunkData_Dic">区块数据字典</param>
    private void UnloadChunks(List<string> chunkNames, Dictionary<string, Chunk> ChunkData_Dic)
    {
        foreach (string chunkName in chunkNames)
        {
            if (ChunkData_Dic.ContainsKey(chunkName))
            {
                Chunk chunk = _Map.loadedChunks_Dic[chunkName];
#if UNITY_EDITOR
                DestroyImmediate(chunk.gameObject);
#else
                Destroy(chunk.gameObject);
#endif
                ChunkData_Dic.Remove(chunkName);
                if (isDebugging)
                {
                    Debug.Log($"销毁区块: {chunkName}");
                }
            }
        }
    }

    /// <summary>
    /// 加载区块
    /// </summary>
    /// <param name="chunkNames">区块名称列表</param>
    /// <param name="ChunkData_Dic">区块数据字典</param>
    private void LoadChunks(List<string> chunkNames, Dictionary<string, ChunkData> ChunkData_Dic)
    {
        foreach (string chunkName in chunkNames)
        {
            if (_Map.loadedChunks_Dic.ContainsKey(chunkName))
            {
                if (_Map.loadedChunks_Dic[chunkName] == null)
                {
                    _Map.loadedChunks_Dic.Remove(chunkName);
                    if (isDebugging)
                    {
                        Debug.Log($"已加载区块已被销毁: {chunkName}");
                    }
                    continue;
                }
                continue;
            }

            if (ChunkData_Dic.ContainsKey(chunkName))
            {
                GameObject instantiatedChunk = Instantiate(chunkPrefab);

                instantiatedChunk.name = chunkName;

                Chunk chunkComponent = instantiatedChunk.GetComponent<Chunk>();

                instantiatedChunk.transform.SetParent(map_parent.transform);

                chunkComponent.Initialize(ChunkData_Dic[chunkName]);

                _Map.loadedChunks_Dic.Add(chunkName, chunkComponent);

                instantiatedChunk.SetActive(true);
                
                if (isDebugging)
                {
                    Debug.Log($"实例化并加载区块: {chunkName}");
                }
            }
        }
    }

    #endregion
}