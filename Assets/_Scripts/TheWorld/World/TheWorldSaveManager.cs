using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class TheWorldSaveManager : MonoBehaviour
{
    #region �ֶκ�����

    public TheWorld theWorld;

    [SerializeField, ListDrawerSettings(DraggableItems = true, ShowItemCount = true)]
    public string ReadSaveFolderPath = "Assets/_Data";

    [SerializeField]
    public bool IsGZip = false; // �Ƿ�ʹ�� GZip ѹ��

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

    #region �������ʱ��ȡ

    public void Start()
    {
        // ����һ�� Coroutine ����ʱ���� LoadWorldFromDiskSave
        StartCoroutine(DelayedLoadWorld());
        EventCenter.Instance.AddEventListener("���µ�ͼ", SaveWorldToDisk);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener("���µ�ͼ", SaveWorldToDisk);
    }

    // Coroutine ��������ʱ������������
    private IEnumerator DelayedLoadWorld()
    {
        // �ȴ� 1 ��
        yield return new WaitForSeconds(1f);

        // ��ʱ 1 ���ִ�� LoadWorldFromDiskSave ����
        LoadWorldFromDiskSave();
    }

    #endregion
    #region ����ͼ�����������

    [Button("�����������ݵ�����")]
    public void SaveWorldToDisk()
    {
        // ��� TheWorld �� TheWorldData �Ƿ���Ч
        if (theWorld == null || theWorldData == null)
        {
            Debug.LogError("TheWorld ����δ�ҽӣ�");
            return;
        }

        // ��ȡ�������Ʋ�������֤
        string worldName = theWorldData.TheWorldName;
        if (string.IsNullOrEmpty(worldName))
        {
            Debug.LogError("Worldname Ϊ�գ��޷��������ݣ�");
            return;
        }

        // �Ƴ��ļ����еķǷ��ַ�
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
        {
            worldName = worldName.Replace(c.ToString(), "");
        }

        // ƴ�������ı���·��
        string fullPath = System.IO.Path.Combine(SaveFilePath, $"{worldName}.bin");

        // ��鲢��������Ŀ¼
        if (!System.IO.Directory.Exists(SaveFilePath))
        {
            System.IO.Directory.CreateDirectory(SaveFilePath);
            Debug.Log($"Ŀ��洢�ļ��в����ڣ��Ѵ�����{SaveFilePath}");
        }

        // �������ݵ�����
        MapDataUtility.SaveToFile(theWorldData, fullPath, IsGZip);

        // ���±���ʱ���
        /*lastSaveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log($"�����ѱ��棡·����{fullPath}��ʱ�䣺{lastSaveTimestamp}");*/
    }


    [Button("�Ӵ��̼������ݵ��ڴ�")]
    public void LoadWorldFromDiskSave()
    {
        // �Ӵ��̼�������
        var loadedData = MapDataUtility.LoadFromFile<TheWorldData>(ReadSaveFolderPath, IsGZip);

        if (loadedData != null)
        {
            theWorldData = loadedData;
        }
        else
        {
            Debug.LogError("������������ʧ�ܣ������ļ�·���͸�ʽ��");
        }
    }

    #endregion

}
