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
    #region �ļ�·�����ļ��в���

    /// <summary>
    /// ȷ��ָ�����ļ��д���
    /// </summary>
    /// <param name="folderPath">�ļ���·��</param>
    public static void EnsureDirectoryExists(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            UnityEngine.Debug.Log($"�ļ��в����ڣ��Ѵ�����{folderPath}");
        }
    }

    /// <summary>
    /// ��ȡ�������ļ�·����ȷ���ļ��д���
    /// </summary>
    /// <param name="folderPath">�ļ���·��</param>
    /// <param name="fileName">�ļ���</param>
    /// <returns>�������ļ�·��</returns>
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

    #region ѹ���ͽ�ѹ��

    /// <summary>
    /// ѹ���ֽ�����
    /// </summary>
    /// <param name="input">�����ֽ�����</param>
    /// <returns>ѹ������ֽ�����</returns>
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
    /// ��ѹ���ֽ�����
    /// </summary>
    /// <param name="input">�����ѹ���ֽ�����</param>
    /// <returns>��ѹ������ֽ�����</returns>
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

    #region �����ͼ����

    public static void SaveToFile<T>(T data, string filePath, bool isGZip = false,bool CanParallel = true)
    {
        byte[] dataBytes = MemoryPackSerializer.Serialize(data);
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {

            // ����ļ��Ƿ���ڲ�������
            
            //����bool���ж��Ƿ��⸲��
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
            UnityEngine.Debug.Log($"�����ѱ��浽 {filePath}. ����ʱ��: {stopwatch.ElapsedMilliseconds} ����");
            // ˢ��AssetDatabase
#if UNITY_EDITOR
            // AssetDatabase.Refresh();
#endif
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"��������ʱ��������{ex.Message}");
        }
    }

    // ��ȡΨһ���ļ�·������ֹ�ļ�����
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

    #region ���ص�ͼ����

    /// <summary>
    /// ���ļ���������
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="filePath">�ļ�·��</param>
    /// <param name="isGZip">�Ƿ�ʹ�� GZip ѹ��</param>
    /// <returns>���ص�����</returns>
    public static T LoadFromFile<T>(string filePath, bool isGZip = false)
    {
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogError($"�ļ�δ�ҵ���{filePath}");
            return default;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            byte[] dataBytes = isGZip ? Decompress(File.ReadAllBytes(filePath)) : File.ReadAllBytes(filePath);
            T data = MemoryPackSerializer.Deserialize<T>(dataBytes);

            stopwatch.Stop();
            UnityEngine.Debug.Log($"�����Ѵ� {filePath} ����. ��ȡʱ��: {stopwatch.ElapsedMilliseconds} ����");
            return data;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"��������ʱ��������{ex.Message}");
            return default;
        }
    }
    #endregion

    #region ���߳�

    /// <summary>
    /// ���߳����л���ͼ����
    /// </summary>
    /// <param name="mapData">��ͼ����</param>
    /// <returns>ѹ������ֽ�����</returns>
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
            UnityEngine.Debug.LogError($"ѹ������ʱ��������{ex.Message}");
            return null;
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log($"���л���ʱ��{stopwatch.ElapsedMilliseconds} ����");

        return compressedFinalData;
    }
    #endregion
}
