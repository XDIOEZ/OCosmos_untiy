using System.Collections.Generic;
using UnityEngine;

public class SpeedSampler : MonoBehaviour
{
    private Vector2 previousPosition;
    private float timeSinceLastUpdate;
    public List<float> speedSamples = new List<float>(); // 存储速度样本的列表
    public List<float> avgSpeedSamples = new List<float>(); // 新列表，用于保存1000个样本用于计算平均速度
    public float maxSpeed = 0f; // 保存检测到的最高速度
    public float averageSpeed = 0f; // 新字段，用于保存计算出的平均速度

    private const float SampleInterval = 0.1f; // 采样间隔时间
    private const int MaxSamples = 15; // 最大样本数量
    private const int MaxAvgSamples = 1000; // 新的最大样本数量，用于计算平均速度

    private void Start()
    {
        previousPosition = transform.position;
        timeSinceLastUpdate = Time.time;

        // 开始定期采样速度
        InvokeRepeating("SampleSpeed", SampleInterval, SampleInterval);
    }

    private void SampleSpeed()
    {
        float deltaTime = Time.time - timeSinceLastUpdate;
        Vector2 currentPosition = transform.position;

        // 计算位置变化向量
        Vector2 positionDelta = currentPosition - previousPosition;

        // 计算综合速度（位置变化向量的长度除以时间差）
        float sampleSpeed = positionDelta.magnitude / deltaTime;

        // 更新最高速度，如果当前样本速度更高
        if (sampleSpeed > maxSpeed)
        {
            maxSpeed = sampleSpeed;
        }

        // 如果列表已满，移除最旧的数据点
        if (speedSamples.Count >= MaxSamples)
        {
            speedSamples.RemoveAt(0);
        }

        // 将速度样本添加到列表中
        speedSamples.Add(sampleSpeed);

        // 同时更新平均速度样本列表
        if (avgSpeedSamples.Count >= MaxAvgSamples)
        {
            avgSpeedSamples.RemoveAt(0);
        }
        avgSpeedSamples.Add(sampleSpeed);

        // 计算平均速度
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