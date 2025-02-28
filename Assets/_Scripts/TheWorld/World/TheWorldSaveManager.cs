using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class TheWorldSaveManager : MonoBehaviour
{
    #region 字段和属性

    public TheWorld theWorld;

    [SerializeField, ListDrawerSettings(DraggableItems = true, ShowItemCount = true)]
    public string ReadSaveFolderPath = "Assets/_Data";

    [SerializeField]
    public bool IsGZip = false; // 是否使用 GZip 压缩

    public string SaveFilePath = "Assets/_Data/Maps/";

    public TheWorldData theWorldData
    {
        get => theWorld?.WorldDataJson;
        set
        {
            if (theWorld != null)
            {
                theWorld.WorldDataJson = value;
            }
        }
    }

    #endregion

    #region 保存和延时读取

    public void Start()
    {
        // 启动一个 Coroutine 来延时调用 LoadWorldFromDiskSave
        StartCoroutine(DelayedLoadWorld());
        EventCenter.Instance.AddEventListener("更新地图", SaveWorldToDisk);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener("更新地图", SaveWorldToDisk);
    }

    // Coroutine 方法：延时加载世界数据
    private IEnumerator DelayedLoadWorld()
    {
        // 等待 1 秒
        yield return new WaitForSeconds(1f);

        // 延时 1 秒后执行 LoadWorldFromDiskSave 方法
        LoadWorldFromDiskSave();
    }

    #endregion
    #region 保存和加载世界数据

    [Button("保存世界数据到磁盘")]
    public void SaveWorldToDisk()
    {
        // 检查 TheWorld 和 TheWorldData 是否有效
        if (theWorld == null || theWorldData == null)
        {
            Debug.LogError("TheWorld 数据未挂接！");
            return;
        }

        // 获取世界名称并进行验证
        string worldName = theWorldData.TheWorldName;
        if (string.IsNullOrEmpty(worldName))
        {
            Debug.LogError("Worldname 为空，无法保存数据！");
            return;
        }

        // 移除文件名中的非法字符
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
        {
            worldName = worldName.Replace(c.ToString(), "");
        }

        // 拼接完整的保存路径
        string fullPath = System.IO.Path.Combine(SaveFilePath, $"{worldName}.bin");

        // 检查并创建保存目录
        if (!System.IO.Directory.Exists(SaveFilePath))
        {
            System.IO.Directory.CreateDirectory(SaveFilePath);
            Debug.Log($"目标存储文件夹不存在，已创建：{SaveFilePath}");
        }

        // 保存数据到磁盘
        MapDataUtility.SaveToFile(theWorldData, fullPath, IsGZip);

        // 更新保存时间戳
        /*lastSaveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log($"数据已保存！路径：{fullPath}，时间：{lastSaveTimestamp}");*/
    }


    [Button("从磁盘加载数据到内存")]
    public void LoadWorldFromDiskSave()
    {
        // 从磁盘加载数据
        var loadedData = MapDataUtility.LoadFromFile<TheWorldData>(ReadSaveFolderPath, IsGZip);

        if (loadedData != null)
        {
            theWorldData = loadedData;
        }
        else
        {
            Debug.LogError("加载世界数据失败，请检查文件路径和格式！");
        }
    }

    #endregion

}
