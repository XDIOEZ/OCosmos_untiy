using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MemoryPackUnion(0, typeof(GunData))]
[MemoryPackUnion(1, typeof(PickaxeToolData))]
[MemoryPackUnion(2, typeof(AxeToolData))]
[MemoryPackable]
[System.Serializable]
public abstract partial class WeaponData : ItemData
{
    #region ������ֵ����
    [Header("---������ֵ---")]
    [LabelText("�����˺�(��ʱ����)")]
    public Damage _damage; // �����˺�
    [LabelText("����ÿ�뾫������(���ڹ���״̬ʱÿ�����ĵľ���)")]
    public float StaminaCost = 10f; // ����ÿ�뾫������
    [LabelText("��󹥻�����(����״̬ʱ����������ƶ�����)")]
    public float MaxAttackDistance = 1f; // ��󹥻�����
    [LabelText("��Ʒ�����ٶ�")]
    public float AttackSpeed = 20;
    [LabelText("��Ʒ�����ٶ�")]
    public float ReturnSpeed = 5f; // ��ǰ��Ʒ�����ٶ�
    [LabelText("����˺����(��λ����)")]
    public float MaxDamageInterval = 0.5f; // ����˺����
   


    #endregion

    //��дToString����������鿴����
    public override string ToString()
    {
        return
               "�����˺���" + _damage + "\n" +
               "����ÿ�뾫�����ģ�" + StaminaCost + "\n" +
               "��󹥻����룺" + MaxAttackDistance + "\n" +
               //   "Ĭ����󹥻�ʱ�䣺" + DefaultMaxAttackTime + "\n" +
               //  "�ɸı���󹥻�ʱ�䣺" + MaxAttackTime + "\n" +
               "�����ٶȣ�" + AttackSpeed + "\n" +
               "��Ʒ�����ٶȣ�" + ReturnSpeed + "\n";
          /*     "��ǰ����ʱ�䣺" + AttackTime + "\n" +
               "��ǰ��Ʒ����ʱ�䣺" + ItemBackTime;*/
    }
}
