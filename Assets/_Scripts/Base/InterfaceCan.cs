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
    float GetHungryValue(); // ����ֵ�仯
    void SetHungryValue(float value); // ����ֵ�仯
}

public interface ICan_FleshForgeAltar
{
}

#region ��Ϊ+���ݽӿ�

/// <summary>
/// �����ƶ���Ϊ�Ľӿ� ICan_Move
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
 /*   IFunction_Move MoveFunction { get; set; } // ��ȡ�ƶ����ܽӿ�*/
    float GetEntitySpeed(); // ��ȡʵ����ƶ��ٶ�
}
/// <summary>
/// ���幥����Ϊ�Ľӿ� ICan_TriggerAttack
/// </summary>
public interface ICan_TriggerAttack
{
    float GetEntityAttackSpeed(); // ��ȡ���չ����ٶ�
}
/// <summary>
/// ��������˺��Ľӿ� ICan_ReceivesDamage
/// </summary>
public interface ICan_ReceivesDamage
{
    void GetRowDamage(float damage); // �ܵ��˺�����������ֵ
}

/// <summary>
/// ���巢���˺��Ľӿ� ICan_SendDamage
/// </summary>
public interface ICan_SendDamage
{
    float GetWeaponDamageValue(); // ��ȡ�������˺�ֵ
}

/// <summary>
/// ������ҿ��ƵĽӿ� ICan_BePlayerControl
/// </summary>
public interface ICan_BePlayerControl
{
    Organ GetEntityOrgan(string ObjectName);
}

/// <summary>
/// �����������Ľӿ� ICan_FaceMouse
/// </summary>
public interface ICan_FaceMouse
{
    void SetFacePosition(Vector3 mousePosition,float speed = 0); // ��ȡ���λ��
}



#endregion


 