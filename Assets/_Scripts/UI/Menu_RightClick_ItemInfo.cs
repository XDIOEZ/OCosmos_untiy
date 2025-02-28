using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_RightClick_ItemInfo : MonoBehaviour
{
    public ScrollRect itemInfoPanel;//面板引用
    public GameObject ScrollContent;//面板内容 作为按钮的父物体
    public ItemData SelectedItemData;//右键点击后传入的物品数据
    public GameObject ButtonPrefab;//按钮预制体
    public Button UseButton;//使用按钮
    public Button InfoButton;//信息按钮
    public Button DiscardButton;//丢弃按钮
    public Button CloseButton;//关闭按钮
    public CanvasGroup CanvasGroup;
    public Item Belong_Player;

    private void Start()
    {
        CloseButton.onClick.AddListener(CloseMenu);
        CanvasGroup = GetComponent<CanvasGroup>();
    }
    public void SetItemData(ItemData itemData)
    {

    }
    public void ShowMenu()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.interactable = true;
    }
    public void SetItemInfo(ItemSlot itemSlot)
    {
        ShowMenu() ;
        transform.position = itemSlot.UI.transform.position;
        Debug.Log("打开右键菜单" + itemSlot._ItemData.Name);
        //使用
        UseButton.onClick.RemoveAllListeners();
        UseButton.onClick.AddListener(() => { CreateAndUseItem(itemSlot); });

        //物品信息

        //丢弃
    }
    public void CloseMenu()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.interactable = false;
    }

    void CreateAndUseItem(ItemSlot itemSlot)
    {
        if(itemSlot._ItemData == null)
        {
            Debug.Log("物品为空！");
            return;
        }
        XDTool.InstantiateAddressableAsync(itemSlot._ItemData.PrefabPath,transform.position, Quaternion.identity,
                (newObject) =>
                {
                    if (newObject != null)
                    {
                        //实例化物体
                        Item newItem = newObject.GetComponent<Item>();
                        newItem.Item_Data = itemSlot._ItemData;
                        //使用物体
                        newItem.Use();

                        //判断是否为营养物品
                        if (newItem is INutrient && Belong_Player is INutrient)
                        {
                            INutrient nutrient_er = Belong_Player.GetComponent<INutrient>();
                            nutrient_er.Eat(newItem as INutrient);
                        }
                     

                        Destroy(newItem.gameObject);

                        Debug.Log("创建并使用物品：" + itemSlot._ItemData.Name);

                        itemSlot._ItemData.Stack.amount--;
                        itemSlot.UI.RefreshUI();

                      
                        if (itemSlot._ItemData.Stack.amount <= 0)
                        {
                            itemSlot.ResetData();
                            CloseMenu();
                        }
                        

                    }
                    else
                    {
                        Debug.LogError("实例化的物体为空！");
                    }
                },
                (error) =>
                {
                    Debug.LogError($"实例化失败: {itemSlot._ItemData.PrefabPath}, 错误信息: {error.Message}");
                }
);
    }

    
    
}
