using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ChunkManager : SingletonMono<ChunkManager>
{
    #region �ֶ�

    // �������
    [Range(0, 1)]
    public float blocksLoadSpeed;

    // ��Ҷ������ڻ�ȡ���λ��
    public List<ChunkLoader> loadingTargets;

    // Ԥ�Ƶ��������
    public GameObject chunkPrefab;

    // Ԥ�Ƶ��������
    public Chunk chunkPrefabComponent;

    // ���Ƶ�����Ϣ����
    public bool isDebugging = true;

    // ��Inspector����ק��Ӧ��Map����
    public GameObject map_parent;

    // ��Inspector����ק��Ӧ��Item����
    public GameObject item_parent;

    // �ȴ����ص��������������б�
    public List<string> WaitLodeChunkDataName_List;

    // ��ͼ���ݹ�����
    public TheWorld_ChunkDataManager _Map
    {
        get { return TheWorld.Instance.map_Dic; }
    }

    #endregion

    #region ����

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

    #region Unity����

    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener("���µ�ͼ", ForceUpdateChunks);
        chunkPrefabComponent = chunkPrefab.GetComponent<Chunk>();
        chunkPrefabComponent.chunkSize = TheWorld.Instance.worldDataJson.ChunkSize;
        chunkPrefabComponent.blocksPerFrame = (int)(_Map.ChunkSize.x * _Map.ChunkSize.y * BlocksLoadSpeed);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener("���µ�ͼ", ForceUpdateChunks);
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
                Debug.LogWarning($"_Map.unloadedChunksData_Dic Ϊ null���޷��������顣");
            }
        }
    }

    #endregion

    #region ��������

    /// <summary>
    /// ��ȡ����Ŀ��
    /// </summary>
    /// <param name="target">����Ŀ�����</param>
    public void GetLoadingTargets(ChunkLoader target)
    {
        if (!loadingTargets.Contains(target))
        {
            loadingTargets.Add(target);
        }
    }

    /// <summary>
    /// ǿ��ˢ�µ�ͼ����
    /// </summary>
    [Button("ǿ��ˢ�µ�ͼ����")]
    public void ForceUpdateChunks()
    {
        WaitLodeChunkDataName_List = GetChunksNameInLoadDistance(loadingTargets);
        UnloadChunks(WaitLodeChunkDataName_List, _Map.loadedChunks_Dic);
        LoadChunks(WaitLodeChunkDataName_List, _Map.unloadedChunksData_Dic);
        List<string> chunksToUnload = _Map.loadedChunks_Dic.Keys.Except(WaitLodeChunkDataName_List).ToList();
        UnloadChunks(chunksToUnload, _Map.loadedChunks_Dic);
    }

    /// <summary>
    /// �����Ӷ���
    /// </summary>
    [Button("�����Ӷ���")]
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
            Debug.Log("û���Ӷ����������١�");
        }
    }

    #endregion

    #region ˽�з���

    /// <summary>
    /// ���ݼ�������ȡ��Χ����,Ȼ����غ�ж���������ط���
    /// </summary>
    /// <param name="WaitLodeChunkDataName">�ȴ����ص��������������б�</param>
    /// <param name="loadingTargets">����Ŀ���б�</param>
    private void UpdateTargetsAroundChunks(List<string> WaitLodeChunkDataName, List<ChunkLoader> loadingTargets)
    {
        LoadChunks(WaitLodeChunkDataName, _Map.unloadedChunksData_Dic);
        List<string> chunksToUnload = _Map.loadedChunks_Dic.Keys.Except(WaitLodeChunkDataName).ToList();
        UnloadChunks(chunksToUnload, _Map.loadedChunks_Dic);
    }

    /// <summary>
    /// ��ȡ���ط�Χ�ڵ���������
    /// </summary>
    /// <param name="loadingTargets">����Ŀ���б�</param>
    /// <returns>���ط�Χ�ڵ����������б�</returns>
    private List<string> GetChunksNameInLoadDistance(List<ChunkLoader> loadingTargets)
    {
        HashSet<string> chunksInRange = new HashSet<string>();

        if (_Map.ChunkSize.x == 0 || _Map.ChunkSize.y == 0)
        {
            Debug.LogWarning("��ͼ�ߴ�δ���ã��޷�������ط�Χ��");
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
    /// ж������
    /// </summary>
    /// <param name="chunkNames">���������б�</param>
    /// <param name="ChunkData_Dic">���������ֵ�</param>
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
                    Debug.Log($"��������: {chunkName}");
                }
            }
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="chunkNames">���������б�</param>
    /// <param name="ChunkData_Dic">���������ֵ�</param>
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
                        Debug.Log($"�Ѽ��������ѱ�����: {chunkName}");
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
                    Debug.Log($"ʵ��������������: {chunkName}");
                }
            }
        }
    }

    #endregion
}