using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ChunkNoiseData", menuName = "����/��������", order = 1)]
public class ChunkNoiseData : ScriptableObject
{
    public ChunkType Type; // ������������
    public  float noiseScale = 0.1f;
    [SerializeField, Range(0, 1.0f), Tooltip("�������ֵ")]
    public float minNoiseThreshold; // �������ֵ
    [SerializeField, Range(0, 1.0f), Tooltip("�������ֵ")]
    public float maxNoiseThreshold; // �������ֵ

    public List<BlockNoiseData> blockNoiseData = new List<BlockNoiseData>(); // ������������


}
