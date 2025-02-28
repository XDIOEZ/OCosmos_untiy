using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Biome
{
    public string biomeName; // 生物群系名称
    public List<ChunkData> chunkDataList; // 生物群系的区块块数据列表

    [SerializeField, Range(0, 1.0f), Tooltip("最低噪声值")]
    public float minNoiseThreshold; // 最低 噪声值
    [SerializeField, Range(0, 1.0f), Tooltip("最高噪声值")]
    public float maxNoiseThreshold; // 最大 噪声值


}
//生物群系枚举
public enum BiomeType : byte
{
    Savanna,               // 草原     
    TropicalSavanna,       // 热带草原
    Glacier,               // 冰川
    TropicalMonsoonForest, // 热带季雨林
    BorealForest,          // 北方 针叶林
    TemperateDeciduousForest, // 温带 落叶林
    TropicalDesert,        // 热带沙漠
    TemperateRainforest,   // 温带雨林
    Tundra,                // 苔原
    TropicalRainforest,    // 热带雨林
    Wetland,               // 湿地
    ColdDesert,             // 寒漠
    Ocean,                 // 海洋

}
