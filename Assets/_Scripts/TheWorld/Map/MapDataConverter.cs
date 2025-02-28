using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//�༭��ģʽ������
public class MapDataConverter : MonoBehaviour
{
    public Map_SO _Map_SO;
    public TheWorld_ChunkDataManager _Map_Dic; // �洢��ͼ����

    public void ConvertToList()
    {
        // ��������б�
        _Map_SO.chunkDataList.Clear();
        // ���� _Map_Dic.unloadedChunksData_Dic ��һ�� Dictionary<TKey, ChunkData> ���͵��ֵ�
        foreach (var entry in _Map_Dic.unloadedChunksData_Dic)
        {
            _Map_SO.chunkDataList.Add(entry.Value); // ���ֵ��е�ÿ�� ChunkData ��ӵ��б���
        }

        // ���� chunkDataList �������ֵ��е����� ChunkData
        Debug.Log("Converted dictionary to list with " + _Map_SO.chunkDataList.Count + " entries.");
    }
    public void ConvertToDictionary()
    {
        // ��������ֵ�
        _Map_Dic.unloadedChunksData_Dic.Clear();

        // ����ÿ�� ChunkData ����һ��Ψһ�� name �ֶ���Ϊ��
        foreach (var chunkData in _Map_SO.chunkDataList)
        {
            // ʹ�� chunkData.name ��Ϊ�ֵ�ļ�
            _Map_Dic.unloadedChunksData_Dic["Chunk_" + chunkData.chunkPosition.x + "_" + chunkData.chunkPosition.y] = chunkData; // �� ChunkData ��ӵ��ֵ���
        }

        // ���� unloadedChunksData_Dic �������б��е����� ChunkData
        Debug.Log("Converted list to dictionary with " + _Map_Dic.unloadedChunksData_Dic.Count + " entries.");
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(MapDataConverter))]
public class MapDataConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ��ȡ MapDataConverter �ű�������
        MapDataConverter mapDataConverter = (MapDataConverter)target;

        // ����Ĭ�ϵ� Inspector
        DrawDefaultInspector();

        // ��Ӱ�ť�����ʱ���� ConvertToList ����
        if (GUILayout.Button("Convert To List"))
        {
            mapDataConverter.ConvertToList();
        }

        // ��Ӱ�ť�����ʱ���� ConvertToDictionary ����
        if (GUILayout.Button("Convert To Dictionary"))
        {
            mapDataConverter.ConvertToDictionary();
        }
    }
}
#endif
