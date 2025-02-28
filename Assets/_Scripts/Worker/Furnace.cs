using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class Furnace : Item, IWork, IInteract
{

    // 使用 [LabelText] 和 [ShowInInspector] 特性修饰字段
    [LabelText("熔炉数据"), ShowInInspector]
    public WorkerData _furnaceData;

    [LabelText("是否正在工作"), ShowInInspector]
    public bool isWorking;

    [LabelText("是否可以工作"), ShowInInspector]
    public bool canWork;

    [LabelText("最大燃料量(单位:秒)"), ShowInInspector]
    public float maxFuelAmount;

    [LabelText("当前燃料量"), ShowInInspector]
    public float currentFuelAmount;

    [LabelText("工作速度(单位:秒)"), ShowInInspector]
    public float workSpeed;

    // 输入槽
    [LabelText("输入槽"), ShowInInspector]
    public Inventory Input_inventory { get; set; }

    // 输出槽
    [LabelText("输出槽"), ShowInInspector]
    public Inventory Output_inventory { get; set; }

    // 燃料槽
    [LabelText("燃料槽"), ShowInInspector]
    public Inventory Fuel_inventory { get; set; }


    //实例化时初始化Inventory
    public  void Start()
    {
        Input_inventory = GetComponentInChildren<Inventory>();
        Output_inventory = GetComponentInChildren<Inventory>();
        Fuel_inventory = GetComponentInChildren<Inventory>();

        Input_inventory.Data = _furnaceData.Inventory_Data_List[0];
        Output_inventory.Data = _furnaceData.Inventory_Data_List[1];
        Fuel_inventory.Data = _furnaceData.Inventory_Data_List[2];
    }


    public override ItemData Item_Data
    {
        get
        {
            return _furnaceData;
        }
        set
        {
            _furnaceData = value as WorkerData;
        }
    }

    public void Work_Start()
    {
        throw new System.NotImplementedException();
    }

    public void Work_Update()
    {
        throw new System.NotImplementedException();
    }

    public void Work_Stop()
    {
        throw new System.NotImplementedException();
    }

    public void Interact_Start()
    {
        throw new System.NotImplementedException();
    }

    public void Interact_Update()
    {
        throw new System.NotImplementedException();
    }

    public void Interact_Cancel()
    {
        throw new System.NotImplementedException();
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }
}