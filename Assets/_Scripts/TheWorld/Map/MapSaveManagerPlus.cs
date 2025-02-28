using UnityEngine;

public class MapSaveManagerPlus : MonoBehaviour
{
/*    [SerializeField] private TheWorld_ChunkDataManager ReadLoadMapData;
    public int MapIndex;
    [SerializeField] private string ReadSaveFolderPath = "Assets/Resources/MapSave/";
    [SerializeField] private string customFileName = "mapData.json";
    public bool IsGZip = false;*/
/*
    public void SaveMapToTheWorld()
    {
        if (ReadLoadMapData == null || ReadLoadMapData.mapData == null)
        {
            UnityEngine.Debug.LogError("Map_Data Êý¾ÝÎ´¹Ò½Ó£¡");
            return;
        }

        string fullPath = MapDataUtility.GetFullFileName(ReadSaveFolderPath, customFileName);
        MapDataUtility.SaveToJson(ReadLoadMapData.mapData, fullPath, IsGZip);
    }
    public void LoadMapFromJson()
    {
        string fullPath = MapDataUtility.GetFullFileName(ReadSaveFolderPath, customFileName);

        var mapData = MapDataUtility.LoadFromJson<Map_Data>(fullPath, IsGZip);
            ReadLoadMapData.mapData = mapData;
    }
*/

}
