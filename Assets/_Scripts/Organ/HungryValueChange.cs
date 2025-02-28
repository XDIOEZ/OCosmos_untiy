using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HungryValueChange : MonoBehaviour, IFunction_ChangeHungryValue
{
    public float CurrentHungryValue = 100;
    public float MaxHungryValue = 100;
    public float GetMaxHungryValue()
    {
        return MaxHungryValue;
    }
    public float BaseHungryRate; // 基础饥饿消耗率
    private float adjustedHungryRate; // 调整后的饥饿消耗率
    public float OnMoveHungryRate; // 移动时饥饿消耗率的加成

    public void ChangeHungryValue(float deltaTime)
    {
        if (CanMoveItem != null)
        {
            
            // 根据调整后的饥饿消耗率减少饥饿值
            CurrentHungryValue -= deltaTime * adjustedHungryRate;

            // 确保饥饿值不会低于0
            if (CurrentHungryValue < 0)
            {
                CurrentHungryValue = 0;
            }
        }
    }

    public void Update()
    {
        // 调整饥饿消耗率
        AdjustHungryRateBasedOnSpeed();

        // 按调整后的消耗率改变饥饿值
        ChangeHungryValue(Time.deltaTime);
    }

    private void AdjustHungryRateBasedOnSpeed()
    {
        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
        Debug.Log(rb);
        if (rb != null)
        {
            Vector2 velocity = rb.velocity;
            float speed = velocity.magnitude;

            // 假设速度每增加1单位，饥饿消耗率增加0.1单位
            adjustedHungryRate = BaseHungryRate + (speed * OnMoveHungryRate);
        }
        else
        {
            adjustedHungryRate = BaseHungryRate;
        }
    }

    private ICan_ChangeHungryValue canChangeHungryValue;

    public ICan_ChangeHungryValue CanMoveItem
    {
        get
        {
            if (canChangeHungryValue == null)
            {
                canChangeHungryValue = GetComponentInParent<ICan_ChangeHungryValue>();
            }
            return canChangeHungryValue;
        }
        set => canChangeHungryValue = value;
    }

    void OnGUI()
    {
        // 计算血条的背景宽度和高度
        float healthBarHeight = 20f; // 血条的高度
        Rect healthBarBackground = new Rect(0, Screen.height - healthBarHeight, Screen.width, healthBarHeight);

        // 绘制血条的背景，这里使用默认的灰色
        GUI.Box(healthBarBackground, "", GUI.skin.box);

        // 根据当前饥饿值计算血条的宽度
        float healthBarWidth = healthBarBackground.width * (CurrentHungryValue / GetMaxHungryValue());

        // 创建血条的矩形区域
        Rect healthBar = new Rect(healthBarBackground.x, healthBarBackground.y, healthBarWidth, healthBarBackground.height);

        // 设置GUI颜色为黄色
        GUI.color = Color.yellow;

        // 绘制血条
        GUI.Box(healthBar, "");

        // 重置颜色，避免影响后续的GUI调用
        GUI.color = Color.white;
    }

}
