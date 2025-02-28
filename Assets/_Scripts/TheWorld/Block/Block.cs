using Gaskellgames;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    public BlockData blockData;
    public BlockData BlockData
    {
        get
        {
            return blockData;
        }

        set
        {
            blockData = value;
        }
    }

    public void SyncPosition()
    {
        blockData.SetTransform(transform);
       // Debug.Log("同步位置");
    }
    public void Start()
    {
        SyncPosition();
    }
    public void OnDestroy()
    {
      
    }
   [Button("拆除方块")]
public void DigBlock_End()
{
    #region 输出位置信息
    // 输出方块数据中的逻辑位置
    Debug.Log("方块逻辑位置: " + blockData.position);
    // 输出物体在世界空间中的坐标
    Debug.Log("世界坐标: " + transform.position);
    // 输出物体在父节点中的本地坐标（新增）
    Debug.Log("本地坐标: " + transform.localPosition);
    #endregion

    #region 获取所在区块
    // 通过世界坐标获取所属区块
    Chunk chunk = TheWorld.Instance.map_Dic.GetChunk_ByBlockPosition(transform.position);
    // 输出区块名称时附带坐标信息
    Debug.Log("目标区块 [" + chunk.name + "] 中的方块");
    #endregion

    #region 修改区块数据
    // 清空区块字典中对应位置的方块名称
    chunk.blockDict[blockData.position].name = "";
    // 销毁当前游戏物体
    Destroy(gameObject);
    #endregion
}


}



