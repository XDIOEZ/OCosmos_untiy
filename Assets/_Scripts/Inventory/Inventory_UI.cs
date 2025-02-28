using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UltEvents;

public class Inventory_UI : MonoBehaviour
{
    public Inventory inventory; // 引用的背包对象
    public List<ItemSlot_UI> itemSlots_UI = new List<ItemSlot_UI>(); // 存储所有插槽的UI
    public GameObject itemSlot_UI_prefab; // 预制体

    public SelectSlot TargetSendItemSlot; // 待传递物品槽的选择器

    public UltEvent<int> OnUIChanged
    {
        get
        {
            return inventory.onUIChanged;
        }

        set
        {
            inventory.onUIChanged = value;
        }
    }

    /*    public SelectSlot TargetSendItemSlot
        {
            get
            {
                if (_targetSendItemSlot == null)
                {
                    //通过名字查找物体
                    _targetSendItemSlot = GameObject.Find("TargetSendItemSlot").GetComponent<SelectSlot>();


                }
                return _targetSendItemSlot;
            }

            set
            {
                _targetSendItemSlot = value;
            }
        }*/

    public void Awake()
    {
        if (inventory == null)
        inventory = GetComponent<Inventory>(); // 获取背包对象
        TargetSendItemSlot = GameObject.Find("TargetSendItemSlot").GetComponent<SelectSlot>();
    }
    private void Start()
    {
        if(inventory.Data.inventoryName == "")
        {
            inventory.Data.inventoryName = gameObject.name;
        }
        OnUIChanged += RefreshSlotUI; // 当背包发生变化时刷新UI


        inventory. onDataChanged +=  (int index , ItemData itemData) => OnUIChanged.Invoke(index);


        InstantiateItemSlots(); // 初始化实例化所有ItemSlot_UI
        /*        transform.SetParent(BelongTO, false); // 设置父物体*/
       
   }
    private void OnDestroy()
    {
        OnUIChanged -= RefreshSlotUI; // 当背包发生变化时刷新UI
    }
    [Button]
    public void InstantiateItemSlots()
    {
        // 清空原有的 ItemSlot_UI（如果需要清空数据）
        itemSlots_UI.Clear();

        // 如果已经有足够数量的 ItemSlot_UI，直接更新，不需要重新实例化
        if (itemSlots_UI.Count >= inventory.Data.itemSlots.Count)
        {
            // 更新已存在的 ItemSlot_UI
            RefreshAllInventory();
            return;
        }

        // 清空子对象，如果需要完全替换UI，才销毁旧的
        foreach (Transform child in transform)
        {
            //添加预处理指令

            //在编辑器模式下删除
           // DestroyImmediate(child.gameObject);
            Destroy(child.gameObject);
        }

        // 实例化不足数量的 ItemSlot_UI
        for (int i = itemSlots_UI.Count; i < inventory.Data.itemSlots.Count; i++)
        {
            // 实例化新的 ItemSlot_UI 并设置为子对象
            GameObject itemSlot_UI_go = Instantiate(itemSlot_UI_prefab, transform);
            // 存储新的 ItemSlot_UI
            itemSlots_UI.Add(itemSlot_UI_go.GetComponent<ItemSlot_UI>());
        }

        // 添加按钮监听器
        AddListenersToItemSlots();
        // 刷新 UI
        RefreshAllInventory();
    }
    // 初始化UI的方法
    public void RefreshAllInventory()
    {
        for (int i = 0; i < itemSlots_UI.Count; i++)
        {
            //将UI的背后数据切换引用为真实背包数据
            itemSlots_UI[i].itemSlot = inventory.Data.itemSlots[i];
            //刷新UI
            OnUIChanged.Invoke(i);
        }
    }

    public void RefreshSlotUI(int index)
    {
        //超出索引范围,按最后一个槽位刷新UI
        if (index >= itemSlots_UI.Count)
        {
            index = itemSlots_UI.Count - 1;
        }
      
        itemSlots_UI[index].RefreshUI();
    }
    //TODO 监听列表中全部的ItemSlot_UI中的Button
    public void AddListenersToItemSlots()
    {
        //Debug.Log("Add listeners to Item Slots" + itemSlots_UI.Count);
        for (int i = 0; i < itemSlots_UI.Count; i++)
        {
            int index = i;
            var onItemClick = itemSlots_UI[index].onItemClick;
            var onItemScroll = itemSlots_UI[index].onItemScroll;
            if (onItemClick != null)
            {
                onItemClick.Clear(); // 清除旧的监听器
                onItemClick += () => OnItemSlotClicked(index); // 添加新的监听器
            }
            if (onItemScroll != null)
            {
                onItemScroll.Clear(); // 清除旧的监听器
                onItemScroll += (v) => OnItemSlotScrolled(index,v); // 添加新的监听器
            }
            
        }
    }

    //滚轮事件
   void  OnItemSlotScrolled(int _index_,Vector2 _direction_)
    {
        Debug.Log("OnItemSlotScrolled"+_index_+" "+_direction_);
    }

    //按钮点击事件
    private void OnItemSlotClicked(int _index_)
    {
        inventory.onSlotChanged?.Invoke(_index_,TargetSendItemSlot.HandInventoryUI.inventory.GetItemSlot(_index_));
        //刷新手上插槽的UI
        TargetSendItemSlot.HandInventoryUI.OnUIChanged.Invoke(_index_);
    }

}