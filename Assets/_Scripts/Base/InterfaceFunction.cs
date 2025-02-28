using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class InterfaceFunction : MonoBehaviour
{

}
public interface IStamina
{
}
public interface IFun_FleshForgeAltar
{
    void PushSacrifice(Item item); // 祭品增加

    public bool Composite(); // 合成

}
public interface IFun_Inventory
{
    void AddItem(Item item); // 添加道具
    void RemoveItem(Item item); // 移除道具
    void UseItem(Item item); // 使用道具
}
public interface IFunction_ChangeHungryValue
{
    void ChangeHungryValue(float value); // 饥饿值变化
}
#region 小型方法接口
public interface IFunction_TurnBody
{
    void TurnBodyToDirection(Vector2 direction); // 转身
}
/// <summary>
/// 定义攻击功能接口 IFunction_TriggerAttack
/// </summary>
public interface IFunction_TriggerAttack
{


    UltEvent OnStartAttack { get; set; }
    UltEvent OnStayAttack { get; set; }
    UltEvent OnEndAttack { get; set; }

    void TriggerAttack(KeyState keyState, Vector3 Target); // 执行攻击

    public WeaponData Weapon_Data { get; set; }
}

/// <summary>
/// 定义移动功能接口 IFunction_Move
/// </summary>
public interface IFunction_Move
{
    void Move(Vector2 moveInput); // 执行移动
}

public interface IFunction_Forward
{
    void Forward(Vector2 targetPosition, float rotationSpeed); // 持续前进
}
public interface IFunction_Attack
{
    void StartAttack();

    void StayAttack();

    void StopAttack();
}
public interface IFunction_FaceToMouse
{
    public void FaceToMouse(Vector3 mousePos);
}
public interface IFunction_ReceivesDamage
{

}
public interface IDamageSender
{
    public bool IsDamageModeEnabled { get;set; }
    public  Damage DamageValue  { get; set; }

    public void OnTriggerEnter2D(Collider2D other);


}

#endregion
