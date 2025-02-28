using Sirenix.OdinInspector;
using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot_UI : MonoBehaviour, IPointerDownHandler,IScrollHandler
{
    [ShowInInspector]
    public ItemSlot itemSlot; // 物体插槽的引用
    public Image image; // 显示当前物体的图标
    public TMP_Text text; // 显示当前物体的数量
    public Slider slider; // 如果插槽内的物体有耐久度，可以显示
    public Button button; // 物体的按钮
    public UltEvent onItemClick; // 物体被点击的事件
    public UltEvent<Vector2> onItemScroll; // 物体被滚轮的事件

    private void Awake()
    {
        // 如果为空 获取 子对象 的image和text组件
        // 跳过第一个子物体
        image ??= GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();
        slider = GetComponentInChildren<Slider>();
        // 监听按钮
        button = GetComponent<Button>();
    }

    void Start()
    {/*
        // 监听按钮的点击事件
        button.onClick.AddListener(() =>
        {
            onItemClick.Invoke();
        });*/
        itemSlot.UI = this; // 绑定UI组件
    }

    // 释放Button的监听
    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }




    // 更新UI函数
    [Button]
    public void RefreshUI()
    {
        // 更新数量
        UpdateItemAmount();

        // 更新图标
        UpdateItemIcon();
    }

    // 封装更新数量的方法
    private void UpdateItemAmount()
    {
        //检测物品是否为空
        if ( itemSlot == null ||itemSlot._ItemData == null|| itemSlot._ItemData.Stack == null|| itemSlot._ItemData.Stack.amount == 0)
        {
            //打印是什么东西为空
            if (itemSlot == null)
            {
                //Debug.LogWarning("物品槽为空");
                return;
            }
            else if (itemSlot._ItemData == null)
            {
                //Debug.LogWarning("物品数据为空");
                return;
               
            }
            else if (itemSlot._ItemData.Stack == null)
            {
               // Debug.LogWarning("物品堆叠为空");
                return;
            }
            /*else if (itemSlot._ItemData.Stack.amount == 0)
            {
                Debug.LogWarning("物品数量为0");
            }*/
        }
        int itemAmount = (int)itemSlot._ItemData.Stack.amount;
        // 如果数量为零，则隐藏数量文本
        if (itemAmount == 0)
        {
            text.gameObject.SetActive(false);  // 隐藏数量
        }
        else
        {
            // 显示物体的数量
            text.text = itemAmount.ToString();
            text.gameObject.SetActive(true);  // 显示数量
        }
    }

    // 封装更新图标的方法
    private void UpdateItemIcon()
    {
        // 如果物品数据为空，直接返回
        if (itemSlot._ItemData == null || itemSlot._ItemData.PrefabPath == "")
        {
            image.gameObject.SetActive(false);  // 隐藏图标
            return;
        };
        
        
      /*  // 显示物体的图标
        XDTool.LoadAssetAsync<Sprite>(itemSlot._ItemData.iconPath, (sprite) =>
        {
            image.sprite = sprite;
        });*/
        XDTool.InstantiateAddressableAsync(itemSlot._ItemData.PrefabPath,transform.position, transform.rotation, (go) =>
        {
            image.sprite = go.GetComponentInChildren<SpriteRenderer>().sprite;
            Destroy(go);
        });

        // 显示图标
        image.gameObject.SetActive(true);
    }
    
    //TODO 新增滚轮控制物品使用
    public void Use(PointerEventData eventData)
    {
         // 检查是否是鼠标左键
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onItemClick.Invoke();
        }
        //检查是否是鼠标右键
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(itemSlot._ItemData!= null)
            itemSlot.Belong_Inventory.Belong_Item.GetComponent<PlayerController>()._playerUI.RightClickMenuCanvas
           .GetComponent<Menu_RightClick_ItemInfo> ().SetItemInfo(itemSlot);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Use(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        //向上滚动
        if (eventData.scrollDelta.y > 0)
        {
            //拿起插槽物品
            onItemClick.Invoke();
        }
        //向下滚动
        if (eventData.scrollDelta.y < 0)
        { 
           //放下手中的物品
        
        }
    }
}
