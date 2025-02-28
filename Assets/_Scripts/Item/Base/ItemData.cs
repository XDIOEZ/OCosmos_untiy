using MemoryPack;
using NaughtyAttributes.Test;
using Org.BouncyCastle.Asn1.Cmp;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

// ʹ��[System.Serializable]����ʹ������Ա����л����Ա���Unity�༭������ʾ�ͱ༭
[MemoryPackUnion(0, typeof(WeaponData))]//��������
[MemoryPackUnion(1, typeof(ArmorData))]//��������
[MemoryPackUnion(2, typeof(AmmoData))]//��ҩ����
[MemoryPackUnion(3, typeof(AppleTreeData))]//ƻ��������
[MemoryPackUnion(4, typeof(Apple_Red_Data))]//��ƻ������
[MemoryPackUnion(5, typeof(PlayerData))]//�������
[MemoryPackUnion(6, typeof(CoalData))]//ú̿����
[MemoryPackUnion(7, typeof(Com_ItemData))]//ͨ����Ʒ����
[MemoryPackUnion(8, typeof(PickaxeToolData))]//��ʯ�乤������
[MemoryPackUnion(9, typeof(AxeToolData))]//����������
[MemoryPackUnion(10, typeof(WorkerData))]//����������
[MemoryPackable(SerializeLayout.Explicit)]
[System.Serializable]   
public  abstract partial class ItemData
{
    [MemoryPackOrder(0)]
    [Title("---��Ʒ����---")]
    [LabelText("��Ʒ����")]
    // �����ַ�������name�����ڴ洢��Ʒ������
    public string Name;

    [MemoryPackOrder(1)]
    [LabelText("��ƷID")]
    // �������ͱ���id�����ڴ洢��Ʒ��Ψһ��ʶ��
    public int ID;

    [MemoryPackOrder(2)]
    [LabelText("��Ʒ����")]
    // �����ַ�������description�����ڴ洢��Ʒ��������Ϣ
    public string Description = "ʲô��û������";

    [MemoryPackOrder(3)]
    [LabelText("Ԥ����·��")]
    // �����ַ�������PrefabPath�����ڴ洢��Ʒ��Ԥ����·��
    public string PrefabPath = "";

    [MemoryPackOrder(4)]
    [LabelText("��Ʒ���")]
    // ���������ͱ���Volume�����ڴ洢��Ʒ����������������ֵ
    public float Volume = 1;

    [MemoryPackOrder(5)]
    [LabelText("��Ʒ�;ö�")]
    // �;ö�
    public float Durability = 1;

    [MemoryPackOrder(6)]
    [LabelText("�Ƿ��ʰȡ")]
    public bool CanBePickedUp = true;

    [MemoryPackOrder(7)]
    [LabelText("��Ʒ��ǩ")]
    // ����ö�ٱ���stackItemType�����ڴ洢��Ʒ������
    public ItemTag ItemTags;

    [MemoryPackOrder(8)]
    [LabelText("��Ʒ�ѵ���Ϣ")]
    // �����ṹ�����stack�����ڴ洢��Ʒ�Ķѵ������Ͷѵ�����
    public ItemStack Stack;

    [MemoryPackOrder(9)]
    [LabelText("��Ʒλ��")]
    // �������λ��
    public Vector3 Position;

    [MemoryPackOrder(10)]
    [LabelText("��Ʒ��ת")]
    public Quaternion Rotation;

    [MemoryPackOrder(11)]
    [LabelText("��Ʒ����")]
    public Vector3 Scale;

    [MemoryPackOrder(12)]
    [LabelText("��ǰ�����")]
    public float CurrentVolume
    {
        get
        {
            return Stack.amount * Volume;
        }
    }

    [ShowInInspector, ReadOnly]
    [LabelText("ȫ��Ψһ��ʶ")]
    public int Guid { get => _guid; set => _guid = value; }

    [MemoryPackOrder(13)]
    [LabelText("��Ʒ��������")]
    // ������Ʒ���������ݣ����������Ĺ����� ���׵Ļ���ֵ ��ҩ��ʣ��������
    public string ItemSpecialData;

    [MemoryPackOrder(14)]
    private int _guid;

    // GUID���ԣ�������л�������


    // MemoryPack���캯�����ؼ�����
    [MemoryPackConstructor]
    public ItemData()
    {
        // �����л�ʱ��������GUID������ԭʼֵ��
        if (Guid == 0) // ��� GUID δ���ã��������µ� GUID
        {
            Guid = System.Guid.NewGuid().GetHashCode(); // ����ϵͳ GUID ���������� GUID
        }
    }


    //��дToString�����������ڿ���̨�����Ʒ��Ϣ
    public override string ToString()
    {
        string str = 
            $"��Ʒ���ƣ�{Name}," +
            $"��ƷID��{ID}," +
            $"��Ʒ������{Description}," +
            $"��Ʒ�����{Volume}," +
            $"��Ʒ�;öȣ�{Durability}," +
            $"�Ƿ��ʰȡ��{CanBePickedUp}," +
            $"��Ʒ��ǩ��{ItemTags}," +
            $"��Ʒ�ѵ���Ϣ��{Stack}," +
            $"��Ʒλ�ã�{Position}," +
            $"��Ʒ��ת��{Rotation}," +
            $"��Ʒ���ţ�{Scale}," +
            $"��Ʒ�������ݣ�{ItemSpecialData}," +
            $"ȫ��Ψһ��ʶ��{Guid}";
            
        return str;
    
    }

    // �°淽����ͨ��Vector3�������ñ任���ݣ�����ѡ������
    public void SetTransformValue(
        Vector3 position,
        Quaternion? rotation = null,
        Vector3? scale = null)
    {
        Position = position;

        // ʹ�ÿպϲ����������ԭ��ֵ
        Rotation = rotation ?? Rotation;
        Scale = scale ?? Scale;

        /* ��ʹ��Ĭ��ֵ����
        Rotation = rotation.HasValue ? rotation.Value : Quaternion.identity;
        Scale = scale.HasValue ? scale.Value : Vector3.one;
        */
    }

}

[MemoryPackable]
[System.Serializable]
public partial class ItemTag
{
    public ItemType Type_Base
    {
        get
        {
            return Item_TypeTag[0];
        }

        set
        {
            Item_TypeTag[0] = value;
        }
    }
    public ItemType Type_Sub
    {
        get
        {
            return Item_TypeTag[1];
        }

        set
        {
            Item_TypeTag[1] = value;
        }
    }
    

    public List<ItemType> Item_TypeTag = new List<ItemType> { ItemType.None, ItemType.None };
    public List<ItemMaterial> Item_Material = new List<ItemMaterial> { ItemMaterial.None};
    
}

public enum ItemType
{
    None,
    Food,
    Weapon,
    Armor,
    Gun,
    Ammo,
    Material,
    Axe,
    Sword,
    Bow,
}

public enum ItemQuality
{
    Normal,
    Rare,
    Epic,
    Legendary,
}

public enum ItemMaterial
{
    None,
    Wood,
    Stone,
    Metal,
    Leather,
}



//���ڿ���Unity��Ϸ���Ҵ���һ����
//1.�����������ʶ��Ʒ�����ͣ��������������ף���ҩ��
//2.һ����Ʒ�����ͻ��ж�������ͣ����������н�ս,Զ��,���������ͣ�������ͷ�������ɣ��Ȳ����Ų���������
//3.һ����Ʒ�����ͻ��ж���������ͣ�����שͷ ����������Ҳ�ǲ���
//4.����Ҫ��������Ҫ�����һ��ItemType��,������������չ
//5.����Ҫ��Unity�Ĵ����оͿ���������
/*public enum Type_Sub
{
    None,
    Armor_Head,
    Armor_Body,
    Armor_Legs,
    Armor_Feet,
    Material_Wood,
    Material_Stone,
}*/
/*
[System.Serializable]
public class ArmorType : ItemTag
{
    public Type_Armor Armor_type;

}
public enum Type_Armor
{
    Head,
    Body,
    Legs,
    Feet,
}*/
