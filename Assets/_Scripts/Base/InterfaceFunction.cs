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
    void PushSacrifice(Item item); // ��Ʒ����

    public bool Composite(); // �ϳ�

}
public interface IFun_Inventory
{
    void AddItem(Item item); // ��ӵ���
    void RemoveItem(Item item); // �Ƴ�����
    void UseItem(Item item); // ʹ�õ���
}
public interface IFunction_ChangeHungryValue
{
    void ChangeHungryValue(float value); // ����ֵ�仯
}
#region С�ͷ����ӿ�
public interface IFunction_TurnBody
{
    void TurnBodyToDirection(Vector2 direction); // ת��
}
/// <summary>
/// ���幥�����ܽӿ� IFunction_TriggerAttack
/// </summary>
public interface IFunction_TriggerAttack
{


    UltEvent OnStartAttack { get; set; }
    UltEvent OnStayAttack { get; set; }
    UltEvent OnEndAttack { get; set; }

    void TriggerAttack(KeyState keyState, Vector3 Target); // ִ�й���

    public WeaponData Weapon_Data { get; set; }
}

/// <summary>
/// �����ƶ����ܽӿ� IFunction_Move
/// </summary>
public interface IFunction_Move
{
    void Move(Vector2 moveInput); // ִ���ƶ�
}

public interface IFunction_Forward
{
    void Forward(Vector2 targetPosition, float rotationSpeed); // ����ǰ��
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
