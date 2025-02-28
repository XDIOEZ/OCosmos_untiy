using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class Furnace : Item
{
    //��¯����
    public FurnaceData _furnaceData;

    public UltEvent onSmeltFinished; // ��������¼�
    public UltEvent onSmeltStart; // ������ʼ�¼�

    public override ItemData Item_Data
    {
        get
        {
            return _furnaceData;
        }   
        set
        {
            _furnaceData = value as FurnaceData;
        }
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }



    #region ��ʼ��
    public void Start()
    {
        Init();
    }
    //��ʼ��
    public void Init()
    {
        //��ʼ����Ʒ��
        _furnaceData.itemInventory = GetComponentInChildren<Inventory>();
        //��ʼ��ȼ�ϲ�
        _furnaceData.fuelInventory = GetComponentInChildren<Inventory>();
        //��ʼ����Ʒ��
        _furnaceData.productInventory = GetComponentInChildren<Inventory>();
    }
    #endregion

    #region ��¯����
    //��ʼ����
    public void StartSmelt()
    {
        if (_furnaceData.isSmelting)
        {
            return;
        }
    }

    //ֹͣ����
    public void StopSmelt()
    {
        if (!_furnaceData.isSmelting)
        {
            return;
        }
    }
    #endregion

    #region ��Ʒ����

    //����һ����Ʒ
    public void SmeltItem(Inventory itemInventory, Inventory productInventory, CookRecipe cookRecipe)
    {
        List<string> ingredientStrings = new List<string>();
        
    }
    #endregion

    #region ������

    [System.Serializable]
    public class FurnaceData : ItemData
    {
        [Header("������Ʒ")]
        //��Ʒ��
        public Inventory itemInventory;
        //ȼ�ϲ�
        public Inventory fuelInventory;
        //��Ʒ��
        public Inventory productInventory;

        [Header("��¯����")]
        public float maxFuel = 100f;  // ���ȼ����

        public float currentFuel = 0f; // ��ǰȼ����

        public float fuelBurnRate = 1f; // ȼ���������ʣ�ÿ�����Ķ���ȼ�ϣ�

        public float ItemBurnAmount = 1f; //һ��������Ʒ��������

        public float ProductAmount = 1f; // ������Ʒ�Ĳ�����

        public bool isSmelting = false; // �Ƿ���������
    }
    #endregion
}
