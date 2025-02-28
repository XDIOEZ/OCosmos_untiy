using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Biome
{
    public string biomeName; // ����Ⱥϵ����
    public List<ChunkData> chunkDataList; // ����Ⱥϵ������������б�

    [SerializeField, Range(0, 1.0f), Tooltip("�������ֵ")]
    public float minNoiseThreshold; // ��� ����ֵ
    [SerializeField, Range(0, 1.0f), Tooltip("�������ֵ")]
    public float maxNoiseThreshold; // ��� ����ֵ


}
//����Ⱥϵö��
public enum BiomeType : byte
{
    Savanna,               // ��ԭ     
    TropicalSavanna,       // �ȴ���ԭ
    Glacier,               // ����
    TropicalMonsoonForest, // �ȴ�������
    BorealForest,          // ���� ��Ҷ��
    TemperateDeciduousForest, // �´� ��Ҷ��
    TropicalDesert,        // �ȴ�ɳĮ
    TemperateRainforest,   // �´�����
    Tundra,                // ̦ԭ
    TropicalRainforest,    // �ȴ�����
    Wetland,               // ʪ��
    ColdDesert,             // ��Į
    Ocean,                 // ����

}
