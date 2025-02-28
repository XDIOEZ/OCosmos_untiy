using UnityEngine;
//���Ϊ���� 
[System.Serializable]
public class BlockNoiseData
{
    public string blockName; // ������������
    public float noiseScale = 0.1f;
    [SerializeField, Range(0, 1.0f), Tooltip("�������ֵ")]
    public float minNoiseThreshold; // �������ֵ
    [SerializeField, Range(0, 1.0f), Tooltip("�������ֵ")]
    public float maxNoiseThreshold; // �������ֵ
}