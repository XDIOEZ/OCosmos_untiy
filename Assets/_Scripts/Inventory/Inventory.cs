using Force.DeepCloner;
using MemoryPack;
using System;
using System.Collections.Generic;
using UltEvents;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    public Item Belong_Item; // 所属物品
    public Inventory_Data Data = new Inventory_Data();
    public int MinStackVolume = 2;
    public UltEvent<int, ItemData> onDataChanged = new UltEvent<int, ItemData>(); // 当物品槽发生变化时触发的事件
    public UltEvent<int, ItemSlot> onSlotChanged = new UltEvent<int, ItemSlot>(); // 当物品槽发生变化时触发的事件
    public UltEvent<int> onUIChanged = new UltEvent<int>(); // 当背包发生变化时触发的事件

    //标识_插槽是否已经满了
    public bool Inventory_Slots_All_IsFull
    {
        get
        {
            for (int i = 0; i < Data.itemSlots.Count; i++)
            {
                if (Data.itemSlots[i].IsFull == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    #region  生命周期
    public void Awake()
    {

        onSlotChanged += ChangeItemData_Default;//注册默认交换物体事件
        onSlotChanged += (int _Index_, ItemSlot _itemSlot_Input_) =>
        {
            onDataChanged.Invoke(_Index_, _itemSlot_Input_._ItemData);
        };
    }
    public void Start()
    {
        //遍历Data.itemSlots
        foreach (ItemSlot itemSlot in Data.itemSlots)
        {
            //设置inventory
            itemSlot.Belong_Inventory = this;
            //设置slotindex
            itemSlot.Index = Data.itemSlots.IndexOf(itemSlot);
            itemSlot.SlotMaxVolume = 128;

        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
        }

        //抬起时恢复默认事件
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
        }
    }
    #endregion
    #region 增删改
    public bool AddItem(ItemData Input_ItemData, int Index = 0)
    {
        if (Input_ItemData == null || !CanAddTheItem(Input_ItemData))
        {
            Debug.Log("物品为空||物品已满");
            return false;
        }

        bool canStack = false;
        int stackIndex = -1;
        int emptyIndex = -1;

        // 新增条件：只有当Input_ItemData.Volume < MinStackVolume时才允许堆叠 
        if (Input_ItemData.Volume < MinStackVolume)
        {
            // 检查是否可以堆叠
            for (int i = 0; i < Data.itemSlots.Count; i++)
            {
                if (Data.itemSlots[i].IsFull || Data.itemSlots[i]._ItemData == null || Data.itemSlots[i]._ItemData.ItemSpecialData != Input_ItemData.ItemSpecialData)
                {
                    continue;
                }
                if (Data.itemSlots[i]._ItemData.PrefabPath == Input_ItemData.PrefabPath && Data.itemSlots[i]._ItemData.ItemSpecialData == Input_ItemData.ItemSpecialData)
                {
                    if (Data.itemSlots[i]._ItemData.CurrentVolume + Input_ItemData.CurrentVolume <= Data.itemSlots[i].SlotMaxVolume)
                    {
                        canStack = true;
                        stackIndex = i;
                        break;
                    }
                }
            }
        }

        // 寻找空位
        if (!canStack)
        {
            for (int i = 0; i < Data.itemSlots.Count; i++)
            {
                if (Data.itemSlots[i]._ItemData == null)
                {
                    emptyIndex = i;
                    break;
                }
            }
        }

        // 进行物品操作
        if (canStack)
        {
            Data.itemSlots[stackIndex]._ItemData.Stack.amount += Input_ItemData.Stack.amount;
            onDataChanged.Invoke(stackIndex, Input_ItemData);
            return true;
        }
        else if (emptyIndex != -1)
        {
            Data.itemSlots[emptyIndex]._ItemData = Input_ItemData;
            onDataChanged.Invoke(emptyIndex, Input_ItemData);
            return true;
        }
        else
        {
            Debug.Log("背包已满");
            return false;
        }
    }
    public bool CanAddTheItem(ItemData Input_ItemData)
    {
        if (Input_ItemData == null)
        {
            Debug.Log("物品为空");
            return false;
        }

        // 新增条件：只有当Input_ItemData.Volume < MinStackVolume时才允许堆叠 
        if (Input_ItemData.Volume >= MinStackVolume)
        {
            // 寻找空位 
            for (int i = 0; i < Data.itemSlots.Count; i++)
            {
                if (Data.itemSlots[i]._ItemData == null)
                {
                    
                    return true;
                }
            }
            Debug.Log("背包已满");
            return false;
        }

        // 原有的堆叠逻辑 
        for (int i = 0; i < Data.itemSlots.Count; i++)
        {
            if (Data.itemSlots[i].IsFull || Data.itemSlots[i]._ItemData == null || Data.itemSlots[i]._ItemData.ItemSpecialData != Input_ItemData.ItemSpecialData)
            {
                continue;
            }
            if (Data.itemSlots[i]._ItemData.PrefabPath == Input_ItemData.PrefabPath && Data.itemSlots[i]._ItemData.ItemSpecialData == Input_ItemData.ItemSpecialData)
            {
                if (Data.itemSlots[i]._ItemData.CurrentVolume + Input_ItemData.CurrentVolume > Data.itemSlots[i].SlotMaxVolume)
                {
                    break;
                }

                return true;
            }
        }

        // 如果没有找到可堆叠的位置，则寻找空位 
        for (int i = 0; i < Data.itemSlots.Count; i++)
        {
            if (Data.itemSlots[i]._ItemData == null)
            {

                return true;
            }
        }

        Debug.Log("背包已满");
        return false;
    }
    public void RemoveItem(ItemSlot itemSlot_, int Index = 0)
    {
        itemSlot_._ItemData = null;
        onUIChanged.Invoke(Index);
    }
    #region ChangeItemData_Default
    public void ChangeItemData_Default(int _Index_, ItemSlot InputSlot_Hand)
    {
        // Debug.Log("开始交换物品");    
        // 交换前的物品数据
        ItemData _LocalData = Data.itemSlots[_Index_]._ItemData;
        ItemData _InputData_Hand = InputSlot_Hand._ItemData;
        //交换前的插槽数据
        ItemSlot _InputSlot_Hand = InputSlot_Hand;
        ItemSlot _LocalSlot = Data.itemSlots[_Index_];
       

        /* if (InputSlot_Hand.Belong_Inventory.GetComponent<Inventory_Hand>().isGetHalf == true)
         {*/
        //两者为空
        if (_InputData_Hand == null && _LocalData == null)
        {
            return;
        }

        #region 手上有物体,本地无物体
        if (_InputData_Hand != null && _LocalData == null)
        {
            float ChangeAmount;
           
            ChangeAmount = (int)Mathf.Ceil(_InputData_Hand.Stack.amount * InputSlot_Hand.Belong_Inventory.GetComponent<Inventory_Hand>().GetItemAmountRate);
            
            ChangeItemAmount(InputSlot_Hand, _LocalSlot, (int)ChangeAmount);
            onUIChanged.Invoke(_Index_);
            return;
            /* while (_InputSlot_Hand._ItemData.Stack.amount >= 0)
             {
                 if (_LocalSlot._ItemData == null)
                 {
                     Item_Data tempItemData = _InputData_Hand.DeepClone();
                     tempItemData.Stack.amount = 1;
                     _LocalSlot._ItemData = tempItemData;
                     _LocalSlot._ItemData.Stack.amount = 0;
                     Debug.Log("克隆物品数据成功" + _LocalSlot._ItemData.PrefabPath);
                 }
                 _LocalSlot._ItemData.Stack.amount += 1;
                 _InputSlot_Hand._ItemData.Stack.amount -= 1;

                 // 如果本地槽位容量已经满了,跳出循环
                 if (_LocalSlot._ItemData.CurrentVolume >= LocalMaxVolume
                   || _InputData_Hand.Stack.amount <= 0 || _InputSlot_Hand._ItemData.Stack.amount <= ChangeAmount)
                 {
                     Debug.Log(_LocalSlot._ItemData.CurrentVolume);
                     Debug.Log(LocalMaxVolume);
                     Debug.Log(_InputData_Hand.Stack.amount);
                     Debug.Log(_InputSlot_Hand._ItemData.Stack.amount);
                     Debug.Log("(武器相同)交换物品槽位:" + _Index_ + " 物品:" + _InputSlot_Hand._ItemData.PrefabPath);

                     //如果输入槽位物品为空,则刷新UI
                     if (_InputData_Hand.Stack.amount <= 0)
                     {
                         InputSlot_Hand.ResetData();
                     }

                     onUIChanged.Invoke(_Index_);

                     return;
                 }



             }*/
        }
        #endregion

        #region 本地有物体,手上没物体
        if (_LocalData != null && _InputData_Hand == null)
        {
            float ChangeAmount;
            ChangeAmount = (int)Mathf.Ceil(_LocalData.Stack.amount * InputSlot_Hand.Belong_Inventory.GetComponent<Inventory_Hand>().GetItemAmountRate);
            ChangeItemAmount(_LocalSlot, InputSlot_Hand, (int)ChangeAmount);
            onUIChanged.Invoke(_Index_);
            return;
        }
        #endregion

        #region 特殊交换

        if (_LocalSlot._ItemData.Volume > MinStackVolume || _LocalSlot._ItemData.ItemSpecialData != _InputSlot_Hand._ItemData.ItemSpecialData)
        {
            Debug.Log("特殊交换");
            _LocalSlot.Change(_InputSlot_Hand);
            onUIChanged.Invoke(_Index_);
            return;
        }
        #endregion

        #region 物品相同
        if (_InputSlot_Hand._ItemData.Name == _LocalSlot._ItemData.Name)
        {
            float ChangeAmount;
            ChangeAmount = (int)Mathf.Ceil(_LocalSlot._ItemData.Stack.amount * InputSlot_Hand.Belong_Inventory.GetComponent<Inventory_Hand>().GetItemAmountRate);
            ChangeItemAmount(_LocalSlot, InputSlot_Hand, (int)ChangeAmount);
            onUIChanged.Invoke(_Index_);
            return;
        }
        #endregion

        #region 两者不为空且物品不相同
        if (_InputData_Hand != null && _LocalData != null && _LocalData.PrefabPath != _InputData_Hand.PrefabPath)
        {
            /*Debug.Log("两者非空开始交换");
            if (_InputData_Hand.Stack.amount * _InputData_Hand.Volume > _LocalSlot.SlotMaxVolume)
            {
                Debug.Log(" 交换失败, 插槽已满");
                return;
            }*/

            _LocalSlot.Change(_InputSlot_Hand);

            onUIChanged.Invoke(_Index_);

            Debug.Log("(物品不同)交换物品槽位:" + _Index_ + " 物品:" + _InputSlot_Hand._ItemData.PrefabPath);

            return;
        }
        #endregion

      




       /* }
        else
        {
            #region 两者插槽不为空,且物品相同
            // 如果交换的物品相同
            if (_LocalSlot._ItemData != null && _InputSlot_Hand._ItemData != null && _InputSlot_Hand._ItemData.PrefabPath == _LocalSlot._ItemData.PrefabPath)
            {
                Debug.Log("相同物品开始交换");
                if (_LocalSlot._ItemData.Volume > MinStackVolume || _LocalSlot._ItemData.ItemSpecialData != _InputSlot_Hand._ItemData.ItemSpecialData)
                {
                    _LocalSlot.Change(_InputSlot_Hand);
                    onUIChanged.Invoke(_Index_);
                    return;
                }
                while (true)
                {
                    // 如果本地槽位容量已经满了,跳出循环
                    if (_LocalData.Stack.amount <= 0)
                    {
                        _LocalSlot.ResetData();
                        onUIChanged.Invoke(_Index_);
                        return;
                    }

                    Debug.Log("本地槽位容量未满+1");
                    _LocalData.Stack.amount -= 1;

                    _InputData_Hand.Stack.amount += 1;
                }
            }
            #endregion
            #region 手上不是空的,点击的插槽是空的
            //本地空位_输入非空
            if (_InputData_Hand != null && _LocalData == null)
            {


                while (_InputSlot_Hand._ItemData.Stack.amount >= 0)
                {


                    if (_LocalSlot._ItemData == null)
                    {

                        //TODO 克隆物品数据,不是引用
                        Item_Data tempItemData = _InputData_Hand.DeepClone();
                        tempItemData.Stack.amount = 1;
                        _LocalSlot._ItemData = tempItemData;

                        _LocalSlot._ItemData.Stack.amount = 0;
                        Debug.Log("克隆物品数据成功" + _LocalSlot._ItemData.PrefabPath);


                    }




                    _LocalSlot._ItemData.Stack.amount += 1;

                    _InputSlot_Hand._ItemData.Stack.amount -= 1;

                    // 如果本地槽位容量已经满了,跳出循环
                    if (_LocalSlot._ItemData.Volume * _LocalSlot._ItemData.Stack.amount >= LocalMaxVolume
                      || _InputData_Hand.Stack.amount <= 0)
                    {

                        Debug.Log("(武器相同)交换物品槽位:" + _Index_ + " 物品:" + _InputSlot_Hand._ItemData.PrefabPath);

                        //如果输入槽位物品为空,则刷新UI
                        if (_InputData_Hand.Stack.amount <= 0)
                        {
                            InputSlot_Hand.ResetData();
                        }

                        onUIChanged.Invoke(_Index_);

                        return;
                    }



                }


            }
            #endregion
            #region 本地非空,手上为空
            if (_LocalData != null && _InputData_Hand == null)
            {
                // Debug.Log("输入空位_开始交换");

                _LocalSlot.Change(_InputSlot_Hand);


            }
            #endregion
            #region 两者不为空且物品不相同
            if (_InputData_Hand != null && _LocalData != null && _LocalData.PrefabPath != _InputData_Hand.PrefabPath)
            {
                Debug.Log("两者非空开始交换");
                *//*if (_InputData_Hand.Stack.amount * _InputData_Hand.Volume > _LocalSlot.SlotMaxVolume)
                {
                    Debug.Log(" 交换失败, 插槽已满");
                    return;
                }*//*

                _LocalSlot.Change(_InputSlot_Hand);

                onUIChanged.Invoke(_Index_);

                Debug.Log("(物品不同)交换物品槽位:" + _Index_ + " 物品:" + _InputSlot_Hand._ItemData.PrefabPath);

                return;
            }
            #endregion
        }*/
    }
    #endregion
    /// <summary>
    /// 修改数量
    /// </summary>
    /// <param name="_LocalSlot">减少的槽位</param>
    /// <param name="_InputSlot_Hand">增加的槽位</param>
    /// <param name="amount">修改的数量</param>
    public bool ChangeItemAmount(ItemSlot _LocalSlot, ItemSlot _InputSlot_Hand, int Count)
    {
        int ChangeCount = 0;
       // Debug.Log("相同物品开始交换");

        if (_InputSlot_Hand._ItemData == null)
        {
            ItemData tempItemData = _LocalSlot._ItemData.DeepClone();
            tempItemData.Stack.amount = 1;
            _InputSlot_Hand._ItemData = tempItemData;
            _InputSlot_Hand._ItemData.Stack.amount = 0;
           // Debug.Log("克隆物品数据成功" + _LocalSlot._ItemData.PrefabPath);
        }

        if (_LocalSlot._ItemData.ItemSpecialData != _InputSlot_Hand._ItemData.ItemSpecialData)
        {
            return false;
        }
        while (true)
        {
            
            _LocalSlot._ItemData.Stack.amount -= 1;
            ChangeCount++;
            _InputSlot_Hand._ItemData.Stack.amount += 1;

            // 如果本地槽位容量已经满了,跳出循环
            if (ChangeCount >= Count || _LocalSlot._ItemData.Stack.amount <= 0|| _InputSlot_Hand._ItemData.Stack.amount >= _InputSlot_Hand.SlotMaxVolume)
            {
               /* Debug.Log(_InputSlot_Hand._ItemData.CurrentVolume >= _InputSlot_Hand.SlotMaxVolume);
                Debug.Log(_LocalSlot._ItemData.Stack.amount <= 0);
                Debug.Log(_LocalSlot._ItemData.Stack.amount <= Count);
                Debug.Log(_InputSlot_Hand._ItemData.Stack.amount);
                Debug.Log(_InputSlot_Hand.SlotMaxVolume);*/

                if (_LocalSlot._ItemData.Stack.amount <= 0)
                {
                    _LocalSlot.ResetData();
                }
                return true;
            }

        }
    }

    public void SetOneItem(int Index, ItemData Input_ItemData)
    {

    }
    #endregion
    #region  插槽物品查询
    public ItemSlot GetItemSlot(int Index)
    {
        //如果索引超出范围,则取最大值
        if (Index >= Data.itemSlots.Count)
        {
            Index = Data.itemSlots.Count - 1;
        }
        //Debug.Log("获取物品槽位:" + _Index_);
        return Data.itemSlots[Index];
    }
    public ItemData GetItemData(int Index)
    {
        return GetItemSlot(Index)._ItemData;
    }
    #endregion
}

[System.Serializable]
public class Inventory_Data
{
    public string inventoryName = string.Empty;//背包名称

    public List<ItemSlot> itemSlots = new List<ItemSlot>();//保存物品的列表
}

public class CloneItem<T> where T : ItemData
{
    public ItemData Clone(T obj)
    {
        // 将 Item_Data 序列化为字节数组
        byte[] serializedData = MemoryPackSerializer.Serialize(obj);

        // 反序列化为新的 Item_Data 对象
        ItemData tempItemData = MemoryPackSerializer.Deserialize<ItemData>(serializedData);

        return tempItemData;
    }
}
