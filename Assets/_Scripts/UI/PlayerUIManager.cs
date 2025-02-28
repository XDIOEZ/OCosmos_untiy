using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.Experimental.GraphView.GraphView;
#endif

public class PlayerUIManager : MonoBehaviour
{
    public Item CanvasBelong_Item;
    //手动合成面板
    public Canvas Craft;
    //背包面板
    public Canvas Bag;
    //装备面板
    public Canvas Equip;
    //快捷栏面板
    public Canvas QuickAccessBar;
    //手部插槽面板
    public Canvas HandSlotCanvas;
    //右键菜单面板
    public Canvas RightClickMenuCanvas;
    //设置面板
    public Canvas Setting;

    public Inventory[] PlayerInventory;

    void Start()
    {
        RightClickMenuCanvas.GetComponent<Menu_RightClick_ItemInfo>().Belong_Player = CanvasBelong_Item;

        //获取所有子对象背包存入PlayerInventory
        PlayerInventory = GetComponentsInChildren<Inventory>();
      foreach ( Inventory ivt in PlayerInventory)
        {
            ivt.Belong_Item = CanvasBelong_Item;
        }
    }
}
