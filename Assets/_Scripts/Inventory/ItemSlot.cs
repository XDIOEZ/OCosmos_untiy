using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    // 当前插槽中的物品数据   
    [ShowInInspector]
    public ItemData _ItemData = null; // 关键修改

    public ItemTag CanAcceptItemType = new ItemTag();

    public float SlotMaxVolume = 128f;

    public int Index = -1;

    //所属背包
    public Inventory Belong_Inventory = null;

    public ItemSlot_UI UI = null;


    public bool IsFull
    {
        get
        {   
            if (_ItemData == null)
            {
                return false;
            }
            return _ItemData.CurrentVolume >= SlotMaxVolume;
        }
    }
    #region 增删改
    /// <summary>
    /// 向插槽中添加一个物品。
    /// 若插槽为空，则直接放置该物品。
    /// 若插槽中已有相同ID的物品，则增加其堆叠数量。
    /// </summary>
    /// <param name="itemData">要添加的物品数据。</param>
    public void AddItem(ItemData itemData)
    {
        if (_ItemData == null)
        {
            // 插槽为空，直接放置物品
            _ItemData = itemData;
        }
        else if (_ItemData.ID == itemData.ID)
        {
            // 插槽中已有相同ID的物品，增加堆叠数量
            _ItemData.Stack.amount += itemData.Stack.amount;
        }
        // 若插槽中有不同ID的物品，则不进行任何操作
    }

    /// <summary>
    /// 销毁插槽中的部分或全部物品。
    /// 若销毁数量小于堆叠数量，则减少堆叠；否则，清空插槽。
    /// </summary>
    /// <param name="destroyCount">要销毁的物品数量。</param>
    public void DestroyItem(float destroyCount)
    {
        if (_ItemData != null)
        {
            if (_ItemData.Stack.amount > destroyCount)
            {
                // 销毁数量小于堆叠数量，减少堆叠
                _ItemData.Stack.amount -= destroyCount;
            }
            else
            {
                // 销毁数量大于或等于堆叠数量，清空插槽
                _ItemData = null;
            }
        }
    }

    /// <summary>
    /// 与另一个插槽交换物品。
    /// 若另一个插槽中有物品：
    /// - 若当前插槽为空，将另一个插槽的物品移至当前插槽。
    /// - 若当前插槽中有相同ID的物品，合并堆叠数量。
    /// </summary>
    /// <param name="ItemSlot_Input">要交换的插槽对象。</param>
    public void Change(ItemSlot _ItemData_Input)
    {/*
        if (_ItemData == null)
        {
            // 当前插槽为空，直接将输入插槽的物品移至当前插槽
            _ItemData = _ItemData_Input._ItemData;
            _ItemData_Input._ItemData = null; // 清空输入插槽
        }
        else if (_ItemData.ID == _ItemData_Input._ItemData.ID&&_ItemData.ID!=0&&_ItemData_Input._ItemData.ID!=0)//两者Id不为0，且相同ID
        {
            // 插槽中已有相同ID的物品，合并堆叠数量
            _ItemData.Stack.amount += _ItemData_Input._ItemData.Stack.amount;
            _ItemData_Input._ItemData = null; // 清空输入插槽
        }
        else
        {*/
        // 当前插槽的物品和输入插槽的物品不同，交换物品
        ItemData temp = _ItemData;
        _ItemData = _ItemData_Input._ItemData;
        _ItemData_Input._ItemData = temp;
        /* }*/
    }
    #endregion

    public void ResetData()
    {
        _ItemData =null;
        Belong_Inventory.onUIChanged.Invoke(Index);
      //  Debug.Log("ResetData");
    }

    public  ItemSlot(Inventory Inventory_)
    {
        Belong_Inventory = Inventory_;
    }





}