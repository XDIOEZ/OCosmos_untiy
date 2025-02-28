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
    //�ֶ��ϳ����
    public Canvas Craft;
    //�������
    public Canvas Bag;
    //װ�����
    public Canvas Equip;
    //��������
    public Canvas QuickAccessBar;
    //�ֲ�������
    public Canvas HandSlotCanvas;
    //�Ҽ��˵����
    public Canvas RightClickMenuCanvas;
    //�������
    public Canvas Setting;

    public Inventory[] PlayerInventory;

    void Start()
    {
        RightClickMenuCanvas.GetComponent<Menu_RightClick_ItemInfo>().Belong_Player = CanvasBelong_Item;

        //��ȡ�����Ӷ��󱳰�����PlayerInventory
        PlayerInventory = GetComponentsInChildren<Inventory>();
      foreach ( Inventory ivt in PlayerInventory)
        {
            ivt.Belong_Item = CanvasBelong_Item;
        }
    }
}
