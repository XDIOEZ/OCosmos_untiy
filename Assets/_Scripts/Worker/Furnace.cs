using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class Furnace : Item
{
    //熔炉数据
    public FurnaceData _furnaceData;

    public UltEvent onSmeltFinished; // 熔炼完成事件
    public UltEvent onSmeltStart; // 熔炼开始事件

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



    #region 初始化
    public void Start()
    {
        Init();
    }
    //初始化
    public void Init()
    {
        //初始化物品槽
        _furnaceData.itemInventory = GetComponentInChildren<Inventory>();
        //初始化燃料槽
        _furnaceData.fuelInventory = GetComponentInChildren<Inventory>();
        //初始化成品槽
        _furnaceData.productInventory = GetComponentInChildren<Inventory>();
    }
    #endregion

    #region 熔炉控制
    //开始熔炼
    public void StartSmelt()
    {
        if (_furnaceData.isSmelting)
        {
            return;
        }
    }

    //停止熔炼
    public void StopSmelt()
    {
        if (!_furnaceData.isSmelting)
        {
            return;
        }
    }
    #endregion

    #region 物品熔炼

    //熔炼一个物品
    public void SmeltItem(Inventory itemInventory, Inventory productInventory, CookRecipe cookRecipe)
    {
        List<string> ingredientStrings = new List<string>();
        
    }
    #endregion

    #region 数据类

    [System.Serializable]
    public class FurnaceData : ItemData
    {
        [Header("熔炼物品")]
        //物品槽
        public Inventory itemInventory;
        //燃料槽
        public Inventory fuelInventory;
        //成品槽
        public Inventory productInventory;

        [Header("熔炉参数")]
        public float maxFuel = 100f;  // 最大燃料量

        public float currentFuel = 0f; // 当前燃料量

        public float fuelBurnRate = 1f; // 燃料消耗速率（每秒消耗多少燃料）

        public float ItemBurnAmount = 1f; //一次熔炼物品的熔炼量

        public float ProductAmount = 1f; // 熔炼物品的产出量

        public bool isSmelting = false; // 是否正在熔炼
    }
    #endregion
}
