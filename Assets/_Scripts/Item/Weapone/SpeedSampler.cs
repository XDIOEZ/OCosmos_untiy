using System.Collections.Generic;
using UnityEngine;

public class SpeedSampler : MonoBehaviour
{
    private Vector2 previousPosition;
    private float timeSinceLastUpdate;
    public List<float> speedSamples = new List<float>(); // �洢�ٶ��������б�
    public List<float> avgSpeedSamples = new List<float>(); // ���б����ڱ���1000���������ڼ���ƽ���ٶ�
    public float maxSpeed = 0f; // �����⵽������ٶ�
    public float averageSpeed = 0f; // ���ֶΣ����ڱ���������ƽ���ٶ�

    private const float SampleInterval = 0.1f; // �������ʱ��
    private const int MaxSamples = 15; // �����������
    private const int MaxAvgSamples = 1000; // �µ�����������������ڼ���ƽ���ٶ�

    private void Start()
    {
        previousPosition = transform.position;
        timeSinceLastUpdate = Time.time;

        // ��ʼ���ڲ����ٶ�
        InvokeRepeating("SampleSpeed", SampleInterval, SampleInterval);
    }

    private void SampleSpeed()
    {
        float deltaTime = Time.time - timeSinceLastUpdate;
        Vector2 currentPosition = transform.position;

        // ����λ�ñ仯����
        Vector2 positionDelta = currentPosition - previousPosition;

        // �����ۺ��ٶȣ�λ�ñ仯�����ĳ��ȳ���ʱ��
        float sampleSpeed = positionDelta.magnitude / deltaTime;

        // ��������ٶȣ������ǰ�����ٶȸ���
        if (sampleSpeed > maxSpeed)
        {
            maxSpeed = sampleSpeed;
        }

        // ����б��������Ƴ���ɵ����ݵ�
        if (speedSamples.Count >= MaxSamples)
        {
            speedSamples.RemoveAt(0);
        }

        // ���ٶ�������ӵ��б���
        speedSamples.Add(sampleSpeed);

        // ͬʱ����ƽ���ٶ������б�
        if (avgSpeedSamples.Count >= MaxAvgSamples)
        {
            avgSpeedSamples.RemoveAt(0);
        }
        avgSpeedSamples.Add(sampleSpeed);

        // ����ƽ���ٶ�
        averageSpeed = 0f;
        foreach (var speed in avgSpeedSamples)
        {
            averageSpeed += speed;
        }
        averageSpeed /= avgSpeedSamples.Count;

        previousPosition = currentPosition;
        timeSinceLastUpdate = Time.time;
    }
}