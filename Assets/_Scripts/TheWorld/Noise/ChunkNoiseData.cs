using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ChunkNoiseData", menuName = "噪声/区块噪声", order = 1)]
public class ChunkNoiseData : ScriptableObject
{
    public ChunkType Type; // 区块类型名称
    public  float noiseScale = 0.1f;
    [SerializeField, Range(0, 1.0f), Tooltip("最低噪声值")]
    public float minNoiseThreshold; // 最低噪声值
    [SerializeField, Range(0, 1.0f), Tooltip("最高噪声值")]
    public float maxNoiseThreshold; // 最大噪声值

    public List<BlockNoiseData> blockNoiseData = new List<BlockNoiseData>(); // 区块噪声数据


}
