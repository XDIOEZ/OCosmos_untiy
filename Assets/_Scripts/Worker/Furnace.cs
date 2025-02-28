using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class Furnace : Item, IWork, IInteract
{

    // ʹ�� [LabelText] �� [ShowInInspector] ���������ֶ�
    [LabelText("��¯����"), ShowInInspector]
    public WorkerData _furnaceData;

    [LabelText("�Ƿ����ڹ���"), ShowInInspector]
    public bool isWorking;

    [LabelText("�Ƿ���Թ���"), ShowInInspector]
    public bool canWork;

    [LabelText("���ȼ����(��λ:��)"), ShowInInspector]
    public float maxFuelAmount;

    [LabelText("��ǰȼ����"), ShowInInspector]
    public float currentFuelAmount;

    [LabelText("�����ٶ�(��λ:��)"), ShowInInspector]
    public float workSpeed;

    // �����
    [LabelText("�����"), ShowInInspector]
    public Inventory Input_inventory { get; set; }

    // �����
    [LabelText("�����"), ShowInInspector]
    public Inventory Output_inventory { get; set; }

    // ȼ�ϲ�
    [LabelText("ȼ�ϲ�"), ShowInInspector]
    public Inventory Fuel_inventory { get; set; }


    //ʵ����ʱ��ʼ��Inventory
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