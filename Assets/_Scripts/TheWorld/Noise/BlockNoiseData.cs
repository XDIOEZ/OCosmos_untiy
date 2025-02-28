using UnityEngine;
//标记为弃用 
[System.Serializable]
public class BlockNoiseData
{
    public string blockName; // 方块类型名称
    public float noiseScale = 0.1f;
    [SerializeField, Range(0, 1.0f), Tooltip("最低噪声值")]
    public float minNoiseThreshold; // 最低噪声值
    [SerializeField, Range(0, 1.0f), Tooltip("最高噪声值")]
    public float maxNoiseThreshold; // 最大噪声值
}