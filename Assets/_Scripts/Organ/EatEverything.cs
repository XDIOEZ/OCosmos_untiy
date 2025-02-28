using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatEverything : Organ
{
    public Item Eat(Vector3 position)
    {
        // 检查 Camera.main 是否存在
        if (Camera.main == null)
        {
            Debug.LogError("未找到主摄像机！");
            return null;
        }

        // 将传入的位置转为世界坐标（如果需要）
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

        // 获取该位置周围的物体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, 1f);

        // 如果没有找到可吃的物品，返回 null
        return null;
    }
}
