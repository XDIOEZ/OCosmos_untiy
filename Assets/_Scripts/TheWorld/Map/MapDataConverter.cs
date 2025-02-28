using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//编辑器模式下运行
public class MapDataConverter : MonoBehaviour
{
    public Map_SO _Map_SO;
    public TheWorld_ChunkDataManager _Map_Dic; // 存储地图数据

    public void ConvertToList()
    {
        // 清空现有列表
        _Map_SO.chunkDataList.Clear();
        // 假设 _Map_Dic.unloadedChunksData_Dic 是一个 Dictionary<TKey, ChunkData> 类型的字典
        foreach (var entry in _Map_Dic.unloadedChunksData_Dic)
        {
            _Map_SO.chunkDataList.Add(entry.Value); // 将字典中的每个 ChunkData 添加到列表中
        }

        // 现在 chunkDataList 包含了字典中的所有 ChunkData
        Debug.Log("Converted dictionary to list with " + _Map_SO.chunkDataList.Count + " entries.");
    }
    public void ConvertToDictionary()
    {
        // 清空现有字典
        _Map_Dic.unloadedChunksData_Dic.Clear();

        // 假设每个 ChunkData 都有一个唯一的 name 字段作为键
        foreach (var chunkData in _Map_SO.chunkDataList)
        {
            // 使用 chunkData.name 作为字典的键
            _Map_Dic.unloadedChunksData_Dic["Chunk_" + chunkData.chunkPosition.x + "_" + chunkData.chunkPosition.y] = chunkData; // 将 ChunkData 添加到字典中
        }

        // 现在 unloadedChunksData_Dic 包含了列表中的所有 ChunkData
        Debug.Log("Converted list to dictionary with " + _Map_Dic.unloadedChunksData_Dic.Count + " entries.");
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(MapDataConverter))]
public class MapDataConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 获取 MapDataConverter 脚本的引用
        MapDataConverter mapDataConverter = (MapDataConverter)target;

        // 绘制默认的 Inspector
        DrawDefaultInspector();

        // 添加按钮，点击时触发 ConvertToList 方法
        if (GUILayout.Button("Convert To List"))
        {
            mapDataConverter.ConvertToList();
        }

        // 添加按钮，点击时触发 ConvertToDictionary 方法
        if (GUILayout.Button("Convert To Dictionary"))
        {
            mapDataConverter.ConvertToDictionary();
        }
    }
}
#endif
