using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;
using UnityEditor;
using UnityEngine;

public static class MapDataUtility
{
    #region 文件路径和文件夹操作

    /// <summary>
    /// 确保指定的文件夹存在
    /// </summary>
    /// <param name="folderPath">文件夹路径</param>
    public static void EnsureDirectoryExists(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            UnityEngine.Debug.Log($"文件夹不存在，已创建：{folderPath}");
        }
    }

    /// <summary>
    /// 获取完整的文件路径并确保文件夹存在
    /// </summary>
    /// <param name="folderPath">文件夹路径</param>
    /// <param name="fileName">文件名</param>
    /// <returns>完整的文件路径</returns>
    public static string GetFullFileName(string folderPath, string fileName)
    {
        if (!fileName.EndsWith(".bin"))
        {
            fileName += ".bin";
        }

        EnsureDirectoryExists(folderPath);
        return Path.Combine(folderPath, fileName);
    }
    #endregion

    #region 压缩和解压缩

    /// <summary>
    /// 压缩字节数组
    /// </summary>
    /// <param name="input">输入字节数组</param>
    /// <returns>压缩后的字节数组</returns>
    public static byte[] Compress(byte[] input)
    {
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzipStream.Write(input, 0, input.Length);
            }

            return outputStream.ToArray();
        }
    }

    /// <summary>
    /// 解压缩字节数组
    /// </summary>
    /// <param name="input">输入的压缩字节数组</param>
    /// <returns>解压缩后的字节数组</returns>
    public static byte[] Decompress(byte[] input)
    {
        using (var inputStream = new MemoryStream(input))
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(outputStream);
            }
            return outputStream.ToArray();
        }
    }
    #endregion

    #region 保存地图数据

    public static void SaveToFile<T>(T data, string filePath, bool isGZip = false,bool CanParallel = true)
    {
        byte[] dataBytes = MemoryPackSerializer.Serialize(data);
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {

            // 检测文件是否存在并重命名
            
            //根据bool来判断是否检测覆盖
            if (!CanParallel)
            {
   filePath = GetUniqueFilePath(filePath);
            }
         

            if (isGZip)
            {
                var compressedData = Compress(dataBytes);
                File.WriteAllBytes(filePath, compressedData);
            }
            else
            {
                File.WriteAllBytes(filePath, dataBytes);
            }

            stopwatch.Stop();
            UnityEngine.Debug.Log($"数据已保存到 {filePath}. 保存时间: {stopwatch.ElapsedMilliseconds} 毫秒");
            // 刷新AssetDatabase
#if UNITY_EDITOR
            // AssetDatabase.Refresh();
#endif
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"保存数据时发生错误：{ex.Message}");
        }
    }

    // 获取唯一的文件路径，防止文件覆盖
    private static string GetUniqueFilePath(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);

        int counter = 1;
        string newFilePath = filePath;

        while (File.Exists(newFilePath))
        {
            newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}_{counter}{extension}");
            counter++;
        }

        return newFilePath;
    }

    #endregion

    #region 加载地图数据

    /// <summary>
    /// 从文件加载数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="filePath">文件路径</param>
    /// <param name="isGZip">是否使用 GZip 压缩</param>
    /// <returns>加载的数据</returns>
    public static T LoadFromFile<T>(string filePath, bool isGZip = false)
    {
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogError($"文件未找到：{filePath}");
            return default;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            byte[] dataBytes = isGZip ? Decompress(File.ReadAllBytes(filePath)) : File.ReadAllBytes(filePath);
            T data = MemoryPackSerializer.Deserialize<T>(dataBytes);

            stopwatch.Stop();
            UnityEngine.Debug.Log($"数据已从 {filePath} 加载. 读取时间: {stopwatch.ElapsedMilliseconds} 毫秒");
            return data;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"加载数据时发生错误：{ex.Message}");
            return default;
        }
    }
    #endregion

    #region 多线程

    /// <summary>
    /// 多线程序列化地图数据
    /// </summary>
    /// <param name="mapData">地图数据</param>
    /// <returns>压缩后的字节数组</returns>
    public static byte[] ParallelSerialize(Map_Data mapData)
    {
        var stopwatch = Stopwatch.StartNew();
        var dictionary = mapData.AllChunksData_Dic;
        var finalDataList = new List<byte[]>();

        Parallel.ForEach(dictionary, kvp =>
        {
            byte[] keyBytes = MemoryPackSerializer.Serialize(kvp.Key);
            byte[] valueBytes = MemoryPackSerializer.Serialize(kvp.Value);

            lock (finalDataList)
            {
                finalDataList.Add(keyBytes);
                finalDataList.Add(valueBytes);
            }
        });

        byte[] finalData = MemoryPackSerializer.Serialize(finalDataList);
        byte[] compressedFinalData;

        try
        {
            compressedFinalData = Compress(finalData);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"压缩数据时发生错误：{ex.Message}");
            return null;
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log($"序列化耗时：{stopwatch.ElapsedMilliseconds} 毫秒");

        return compressedFinalData;
    }
    #endregion
}
