using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    #region ��Ա����������
    [ShowInInspector]
    public DynamicLoadingTargetData loadingTargetData; // ��̬����Ŀ������

    public bool showGizmo = true; // ���ڿ��� Gizmo ����ʾ
#endregion

    #region �������ڷ���
    private void Start()
    {
        SendLoadingData();
    }
#endregion

    #region ��������
    [Button("���ͼ���������")]
    public void SendLoadingData()
    { 
        // ���ͼ���������
        //Debug.Log("���ͼ���������");
        ChunkManager.Instance.GetLoadingTargets(this);
    }
#endregion

    #region Gizmos����
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.green;
            // ���Ƽ�������
            Gizmos.DrawWireCube(transform.position, new Vector3(loadingTargetData.loadingAreaWidth, loadingTargetData.loadingAreaHeight, 0));
        }
        if (showGizmo)
        {
            Gizmos.color = Color.green;
            // ������չ��ļ�������
            Gizmos.DrawWireCube(transform.position, new Vector3(loadingTargetData.loadingAreaWidth + TheWorld.Instance.WorldDataJson.ChunkSize.x*2, loadingTargetData.loadingAreaHeight + TheWorld.Instance.WorldDataJson.ChunkSize.y*2, 0));
        }
    }
#endregion
}

[System.Serializable]
public class DynamicLoadingTargetData
{
#region ��Ա����
    public bool IsWorking = true; // �Ƿ�Ϊ����Ŀ��
    public float loadingAreaWidth; // ����������
    public float loadingAreaHeight; // ��������߶�
#endregion
}
