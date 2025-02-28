using MemoryPack;
using NaughtyAttributes.Test;
using Org.BouncyCastle.Asn1.Cmp;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

// 使用[System.Serializable]特性使该类可以被序列化，以便在Unity编辑器中显示和编辑
[MemoryPackUnion(0, typeof(WeaponData))]
[MemoryPackUnion(1, typeof(ArmorData))]
[MemoryPackUnion(2, typeof(AmmoData))]
[MemoryPackUnion(3, typeof(AppleTreeData))]
[MemoryPackUnion(4, typeof(Apple_Red_Data))]
[MemoryPackUnion(5, typeof(PlayerData))]
[MemoryPackUnion(6, typeof(CoalData))]
[MemoryPackUnion(7, typeof(GenericItemData))]
[MemoryPackUnion(8, typeof(PickaxeToolData))]
[MemoryPackUnion(9, typeof(AxeToolData))]
//[MemoryPackUnion(8, typeof(Block_ItemData))]
[MemoryPackable(SerializeLayout.Explicit)]
[System.Serializable]   
public  abstract partial class ItemData
{
    [MemoryPackOrder(0)]
    [Title("---物品数据---")]
    // 公共字符串变量name，用于存储物品的名称
    public string Name;

    // 公共整型变量id，用于存储物品的唯一标识符s
    [MemoryPackOrder(1)]
    public int ID;
    
    // 公共字符串变量description，用于存储物品的描述信息
    [MemoryPackOrder(2)]
    public string Description = "什么都没有描述";

    // 公共字符串变量PrefabPath，用于存储物品的预制体路径
    [MemoryPackOrder(3)]
    public string PrefabPath = "";

    // 公共浮点型变量Volume，用于存储物品的体积或其他相关数值
    [MemoryPackOrder(4)]
    public float Volume = 1;

    //耐久度
    [MemoryPackOrder(5)]
    public float Durability = 1;

    [MemoryPackOrder(6)]
    public bool CanBePickedUp = true;

    // 公共枚举变量stackItemType，用于存储物品的类型
    [MemoryPackOrder(7)]
    public ItemTag ItemTags;

    // 公共结构体变量stack，用于存储物品的堆叠数量和堆叠容量
    [MemoryPackOrder(8)]
    public ItemStack Stack;

    //TODO 保存玩家位置,旋转,缩放等信息
    [MemoryPackOrder(9)]
    public Vector3 Position;

    [MemoryPackOrder(10)]
    public Quaternion Rotation;

    [MemoryPackOrder(11)]
    public Vector3 Scale;

    [MemoryPackOrder(12)]
    public float CurrentVolume
    {
        get
        {
            return Stack.amount * Volume;
        }
    }
    [ShowInInspector, ReadOnly]
    public int Guid { get => _guid; set => _guid = value; }

    [MemoryPackOrder(13)]//TODO 保存物品的特殊数据，比如武器的攻击力 护甲的护甲值 弹药的剩余数量等
    public string ItemSpecialData;
    [MemoryPackOrder(14)]
    private int _guid;

    // GUID属性（添加序列化保护）
   

    // MemoryPack构造函数（关键！）
    [MemoryPackConstructor]
    public ItemData()
    {
        // 反序列化时不生成新GUID（保留原始值）
        if (Guid == 0) // 如果 GUID 未设置，则生成新的 GUID
        {
            Guid = System.Guid.NewGuid().GetHashCode(); // 调用系统 GUID 生成器生成 GUID
        }
    }


    //重写ToString方法，用于在控制台输出物品信息
    public override string ToString()
    {

        Debug.Log("物品名称：" + Name + "\n物品ID：" + ID + "\n物品描述：" + Description + "\n物品图标路径："   + "\n物品体积：" + Volume);
        return "物品名称：" + Name + "\n物品ID：" + ID + "\n物品描述：" + Description + "\n物品图标路径："+ "\n物品体积：" + Volume;
    }

    // 新版方法：通过Vector3参数设置变换数据（含可选参数）
    public void SetTransformValue(
        Vector3 position,
        Quaternion? rotation = null,
        Vector3? scale = null)
    {
        Position = position;

        // 使用空合并运算符保持原有值
        Rotation = rotation ?? Rotation;
        Scale = scale ?? Scale;

        /* 或使用默认值策略
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



//我在开发Unity游戏帮我创建一个类
//1.这个类用来标识物品的类型，比如武器，护甲，弹药等
//2.一个物品的类型会有多个子类型，比如武器有近战,远程,冲锋等子类型，护甲有头部，躯干，腿部，脚部等子类型
//3.一个物品的类型会有多个基础类型，比如砖头 它既是武器也是材料
//4.我需要你结合上述要求设计一个ItemType类,优雅且易于扩展
//5.我需要在Unity的窗口中就可以配置它
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
