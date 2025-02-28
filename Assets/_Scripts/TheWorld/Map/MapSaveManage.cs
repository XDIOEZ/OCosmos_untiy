using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MapSaveManager : MonoBehaviour
{
    // �ҽ� Map_So ����
    [SerializeField] private Map_SO mapData;


    // Json �ļ����·��
    [SerializeField] private string jsonSavePath = "Assets/Resources/MapSave/";

    // ���� Map_So �� Json �ļ�
    public void SaveMapToJson()
    {
        if (mapData == null)
        {
            Debug.LogError("Map_So ����δ�ҽӣ�");
            return;
        }

        // �� Map_So ת��Ϊ Json ��ʽ
        string jsonData = JsonUtility.ToJson(mapData, false);

        // �� Json ���ݱ��浽ָ��·��
        File.WriteAllText(jsonSavePath, jsonData);
        Debug.Log("��ͼ�����ѱ��浽 " + jsonSavePath);
        //���������̨
        Debug.Log(jsonData);
    }

    // �� Json �ļ��������ݵ� Map_So
    public void LoadMapFromJson()
    {
        if (!File.Exists(jsonSavePath))
        {
            Debug.LogError("Json �ļ�δ�ҵ���" + jsonSavePath);
            return;
        }

        // ���ļ���ȡ Json ����
        string jsonData = File.ReadAllText(jsonSavePath);

        // �� Json ���ݷ����л�Ϊ Map_So
        JsonUtility.FromJsonOverwrite(jsonData, mapData);
        Debug.Log("��ͼ�����Ѵ� " + jsonSavePath + " ����");
    }
}
//����һ���̳�editor���� ʹ�������������ڱ༭���п�����
#if UNITY_EDITOR
[CustomEditor(typeof(MapSaveManager))]
public class MapSaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ��ʾ MapSaveManager ԭʼ�� Inspector ����
        DrawDefaultInspector();

        // ��ȡ MapSaveManager �ű�������
        MapSaveManager mapSaveManager = (MapSaveManager)target;

        // ��Ӱ�ť�������� SaveMapToTheWorld ����
        if (GUILayout.Button("�����ͼ���ݵ� JSON"))
        {
            mapSaveManager.SaveMapToJson();
        }

        // ��Ӱ�ť�������� LoadMapFromJson ����
        if (GUILayout.Button("�� JSON ���ص�ͼ����"))
        {
            mapSaveManager.LoadMapFromJson();
        }
    }
}
#endif
