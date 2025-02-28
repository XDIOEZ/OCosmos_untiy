using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapTileEditorTool : MonoBehaviour
#region �ֶκ�����
{
    /// <summary>
    /// �������Ƹ�ʽ�ַ���
    /// </summary>
    private const string CHUNK_NAME_FORMAT = "Chunk_{0}_{1}";

    /// <summary>
    /// ż����Сƫ�Ƴ���
    /// </summary>
    private const float EVEN_SIZE_OFFSET_DIVIDER = 2f;

    /// <summary>
    /// ������Сƫ�Ƴ���
    /// </summary>
    private const float ODD_SIZE_OFFSET_DIVIDER = 2f;

    [Header("��������")]
    /// <summary>
    /// ���鸸����
    /// </summary>
    public Transform BlockParent;

    /// <summary>
    /// ������Ʒ����
    /// </summary>
    public Transform WorldItem;

    /// <summary>
    /// ������Ʒ������
    /// </summary>
    public SceneItemManager sceneItemManager;

    [Header("����ʱ����")]
    /// <summary>
    /// �����б�
    /// </summary>
    [ShowInInspector] private List<Block> blockList = new List<Block>();

    /// <summary>
    /// ��ͼ���ݹ�����
    /// </summary>
    private TheWorld_ChunkDataManager MapDic => TheWorld.Instance.map_Dic;
    #endregion

    #region �༭������
    /// <summary>
    /// �����ͼ�޸Ĳ�����ͼ���ݱ��浽�����ļ�
    /// </summary>
    [Button("�����ͼ�޸Ĳ�����ͼ���ݱ��浽�����ļ�")]
    public void SaveMapData()
    {
        if (!ValidateBlockParent()) return; // ��֤BlockParent�Ƿ����

        GetBlockDataToParentGameObject(); // ��ȡ��������
        ClearWorldItems(); // ����������Ʒ
        sceneItemManager.onSceneItemsSetToWorldItemManager.Invoke(); // ֪ͨ������Ʒ������

        MapDic.SaveMapToTheWorld(TheWorld.Instance.worldDataJson.IsZip); // �����ͼ����
        EventCenter.Instance.EventTrigger("���µ�ͼ"); // ������ͼ�����¼�
        Debug.Log("��ͼ�Ѹ���");
    }

    /// <summary>
    /// ��ȡ��������
    /// </summary>
    [Button("��ȡ��������")]
    public void GetBlockData() => GetBlockDataToParentGameObject();

    /// <summary>
    /// �����ڴ�
    /// </summary>
    [Button("�����ڴ�")]
    public void ClearMemory()
    {
        long memoryBefore = GC.GetTotalMemory(false); // ��ȡ����ǰ���ڴ�ʹ�����
        GC.Collect(); // ǿ�ƽ�����������
        GC.WaitForPendingFinalizers(); // �ȴ������ս������
        GC.Collect(); // �ٴν�����������
        Debug.Log($"�ڴ��������\n����ǰ: {memoryBefore / 1024} KB\n�����: {GC.GetTotalMemory(true) / 1024} KB"); // ����ڴ�����ǰ������
    }
    #endregion

    #region �����߼�
    /// <summary>
    /// �Ӹ���Ϸ�����ȡ��������
    /// </summary>
    private void GetBlockDataToParentGameObject()
    {
        if (BlockParent.childCount == 0) return; // ���û���Ӷ����򷵻�

        var childrenToDestroy = new List<Transform>(); // ��Ҫ���ٵ��Ӷ����б�
        foreach (Transform child in BlockParent)
        {
            if (child.TryGetComponent<Block>(out var block))
            {
                blockList.Add(block); // ��ӷ��鵽�б�
                childrenToDestroy.Add(child); // ����Ӷ��������б�
                ProcessBlockData(block); // ����������
            }
        }

        CleanupChildren(childrenToDestroy); // �����Ӷ���
        Debug.Log($"�ɹ���� {blockList.Count} ����������"); // �����ӵķ�������
        blockList.Clear(); // ��շ����б�
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="block">�������</param>
    private void ProcessBlockData(Block block)
    {
        var chunkData = GetChunkDataByBlockPosition(block.BlockData.position); // ��ȡ��������
        if (chunkData != null)
        {
            chunkData.blockDatas.Add(block.BlockData); // ��ӷ������ݵ�����
            Debug.Log($"��ӵ�����: {chunkData.chunkName}"); // �����ӵ�����������
        }
    }
    #endregion

    #region �������
    /// <summary>
    /// ���ݷ���λ�û�ȡ������������
    /// </summary>
    /// <param name="position">����λ��</param>
    /// <returns>������������</returns>
    public string GetChunkDataNameByBlockPosition(Vector3 position)
    {
        var (chunkX, chunkY) = CalculateChunkCoordinates(position); // ������������
        return string.Format(CHUNK_NAME_FORMAT, chunkX * MapDic.ChunkSize.x, chunkY * MapDic.ChunkSize.y); // ������������
    }

    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="position">����λ��</param>
    /// <returns>�������� (X, Y)</returns>
    private (int, int) CalculateChunkCoordinates(Vector3 position)
    {
        var chunkSize = MapDic.ChunkSize; // ��ȡ�����С
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
    #endregion

    #region ���߷���
    [Button("����λ�û�ȡ��������")]
    /// <summary>
    /// ����λ�û�ȡ��������
    /// </summary>
    /// <param name="position">����λ��</param>
    /// <returns>��������</returns>
    private ChunkData GetChunkDataByBlockPosition(Vector3 position) =>
        MapDic.GetChunkDataByName(GetChunkDataNameByBlockPosition(position));

    /// <summary>
    /// ��֤BlockParent�Ƿ����
    /// </summary>
    /// <returns>BlockParent�Ƿ����</returns>
    private bool ValidateBlockParent()
    {
        if (BlockParent != null) return true; // ��֤BlockParent�Ƿ����
        Debug.LogError("δ�ҵ� BlockParent ����"); // ���������Ϣ
        return false;
    }

    /// <summary>
    /// �����Ӷ���
    /// </summary>
    /// <param name="children">��Ҫ���ٵ��Ӷ����б�</param>
    private void CleanupChildren(List<Transform> children)
    {
        foreach (var child in children)
            DestroyImmediate(child.gameObject); // ���������Ӷ���
    }

    /// <summary>
    /// ����������Ʒ
    /// </summary>
    private void ClearWorldItems()
    {
        //����Ƿ�Ϊ��
        if (WorldItem == null) return;
        foreach (Transform child in WorldItem)
            Destroy(child.gameObject); // ����������Ʒ
    }
    #endregion
}
