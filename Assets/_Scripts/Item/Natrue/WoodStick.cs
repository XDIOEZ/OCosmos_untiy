using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodStick : Item
{
    /*    public WoodStickData _data;*/
    public GenericItemData Data;
    public override ItemData Item_Data
    {
        get
        {
            return Data;
        }
        set
        {
            Data = (GenericItemData)value;
        }
    }
    /*{
        get;
        set;
            *//*
        get
        {
            return _data;
        }
        set
        {
            _data = (WoodStickData)value;
        }*//*
    }*/
/*    public override Item_Data GetData()
    {
        Debug.Log($"获取{name}数据中....");
        return _data;
    }
    public override void SetData(Item_Data data)
    {
        Debug.Log($"设置{name}数据中....");
        _data = (WoodStickData)data;
    }*/

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
[MemoryPackable]
[System.Serializable]
public partial class GenericItemData : ItemData
{
    public string code;
}
