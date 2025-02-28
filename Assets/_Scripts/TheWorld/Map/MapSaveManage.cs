using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MapSaveManager : MonoBehaviour
{
    // 挂接 Map_So 引用
    [SerializeField] private Map_SO mapData;


    // Json 文件输出路径
    [SerializeField] private string jsonSavePath = "Assets/Resources/MapSave/";

    // 保存 Map_So 到 Json 文件
    public void SaveMapToJson()
    {
        if (mapData == null)
        {
            Debug.LogError("Map_So 数据未挂接！");
            return;
        }

        // 将 Map_So 转换为 Json 格式
        string jsonData = JsonUtility.ToJson(mapData, false);

        // 将 Json 数据保存到指定路径
        File.WriteAllText(jsonSavePath, jsonData);
        Debug.Log("地图数据已保存到 " + jsonSavePath);
        //输出到控制台
        Debug.Log(jsonData);
    }

    // 从 Json 文件加载数据到 Map_So
    public void LoadMapFromJson()
    {
        if (!File.Exists(jsonSavePath))
        {
            Debug.LogError("Json 文件未找到：" + jsonSavePath);
            return;
        }

        // 从文件读取 Json 数据
        string jsonData = File.ReadAllText(jsonSavePath);

        // 将 Json 数据反序列化为 Map_So
        JsonUtility.FromJsonOverwrite(jsonData, mapData);
        Debug.Log("地图数据已从 " + jsonSavePath + " 加载");
    }
}
//创建一个继承editor的类 使上述两个方法在编辑器中可运行
#if UNITY_EDITOR
[CustomEditor(typeof(MapSaveManager))]
public class MapSaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 显示 MapSaveManager 原始的 Inspector 内容
        DrawDefaultInspector();

        // 获取 MapSaveManager 脚本的引用
        MapSaveManager mapSaveManager = (MapSaveManager)target;

        // 添加按钮并关联到 SaveMapToTheWorld 方法
        if (GUILayout.Button("保存地图数据到 JSON"))
        {
            mapSaveManager.SaveMapToJson();
        }

        // 添加按钮并关联到 LoadMapFromJson 方法
        if (GUILayout.Button("从 JSON 加载地图数据"))
        {
            mapSaveManager.LoadMapFromJson();
        }
    }
}
#endif
