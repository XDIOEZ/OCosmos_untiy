using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceCan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public interface ICan_ChangeHungryValue
{
    float GetHungryValue(); // 饥饿值变化
    void SetHungryValue(float value); // 饥饿值变化
}

public interface ICan_FleshForgeAltar
{
}

#region 行为+数据接口

/// <summary>
/// 定义移动行为的接口 ICan_Move
/// </summary>
/// 
public interface ICan_Eat
{
}
public interface ICan_TurnBody
{
}
public interface ICan_Move
{
 /*   IFunction_Move MoveFunction { get; set; } // 获取移动功能接口*/
    float GetEntitySpeed(); // 获取实体的移动速度
}
/// <summary>
/// 定义攻击行为的接口 ICan_TriggerAttack
/// </summary>
public interface ICan_TriggerAttack
{
    float GetEntityAttackSpeed(); // 获取最终攻击速度
}
/// <summary>
/// 定义接受伤害的接口 ICan_ReceivesDamage
/// </summary>
public interface ICan_ReceivesDamage
{
    void GetRowDamage(float damage); // 受到伤害，减少生命值
}

/// <summary>
/// 定义发送伤害的接口 ICan_SendDamage
/// </summary>
public interface ICan_SendDamage
{
    float GetWeaponDamageValue(); // 获取武器的伤害值
}

/// <summary>
/// 定义玩家控制的接口 ICan_BePlayerControl
/// </summary>
public interface ICan_BePlayerControl
{
    Organ GetEntityOrgan(string ObjectName);
}

/// <summary>
/// 定义面向鼠标的接口 ICan_FaceMouse
/// </summary>
public interface ICan_FaceMouse
{
    void SetFacePosition(Vector3 mousePosition,float speed = 0); // 获取鼠标位置
}



#endregion


 