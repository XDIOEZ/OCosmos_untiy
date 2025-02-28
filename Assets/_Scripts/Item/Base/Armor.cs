using MemoryPack;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    public ArmorData _Data;

    public override ItemData Item_Data
    {
        get
        {
            return _Data;
        }
        set
        {
            _Data = (ArmorData)value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
/*    public override Item_Data GetData()
    {
        return _Data;
    }
    public override void SetData(Item_Data data)
    {
        _Data = (ArmorData)data;
    }*/

    public override void Use()
    {
        throw new NotImplementedException();
    }
}
[MemoryPackable]
[System.Serializable]
public partial class ArmorData : ItemData
{
    [Title("---»¤¼×ÊýÖµ---")]
    public Defense defense;
    public ArmorType armorType;

    public static implicit operator ArmorData(ItemSlot v)
    {
        throw new NotImplementedException();
    }
}
public enum ArmorType
{
    Head,
    Body,
    Leg,
    Feet,
}
