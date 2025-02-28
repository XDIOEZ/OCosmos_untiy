using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    #region 成员变量和属性
    [ShowInInspector]
    public DynamicLoadingTargetData loadingTargetData; // 动态加载目标数据

    public bool showGizmo = true; // 用于控制 Gizmo 的显示
#endregion

    #region 生命周期方法
    private void Start()
    {
        SendLoadingData();
    }
#endregion

    #region 公共方法
    [Button("发送加载器数据")]
    public void SendLoadingData()
    { 
        // 发送加载器数据
        //Debug.Log("发送加载器数据");
        ChunkManager.Instance.GetLoadingTargets(this);
    }
#endregion

    #region Gizmos绘制
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.green;
            // 绘制加载区域
            Gizmos.DrawWireCube(transform.position, new Vector3(loadingTargetData.loadingAreaWidth, loadingTargetData.loadingAreaHeight, 0));
        }
        if (showGizmo)
        {
            Gizmos.color = Color.green;
            // 绘制扩展后的加载区域
            Gizmos.DrawWireCube(transform.position, new Vector3(loadingTargetData.loadingAreaWidth + TheWorld.Instance.WorldDataJson.ChunkSize.x*2, loadingTargetData.loadingAreaHeight + TheWorld.Instance.WorldDataJson.ChunkSize.y*2, 0));
        }
    }
#endregion
}

[System.Serializable]
public class DynamicLoadingTargetData
{
#region 成员变量
    public bool IsWorking = true; // 是否为加载目标
    public float loadingAreaWidth; // 加载区域宽度
    public float loadingAreaHeight; // 加载区域高度
#endregion
}
