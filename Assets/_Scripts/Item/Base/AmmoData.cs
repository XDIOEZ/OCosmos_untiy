using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MemoryPackable]
[System.Serializable]
public partial class AmmoData:ItemData
{
    [Title("�ӵ�����")]
    public float speed;//�ٶ�
    public Damage damage;//�˺�
    public float range;//���
    public float Fired;//�Ƿ��Ѿ�����
    public float MinDamageInterval = 0.5f;//��С�˺����
}
