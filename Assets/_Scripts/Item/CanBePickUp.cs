using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBePickUp : MonoBehaviour, ICanBePickUp
{
    public Item item;

    public ItemData PickUp_ItemData
    {
        get
        {
            return item.Item_Data;
        }
        set
        {
            item.Item_Data = value;
        }
    }

    public void Start()
    {
        //获取物品信息
        item = GetComponent<Item>();
    }

    public virtual ItemData Pickup()
    {
        if (item.Item_Data.CanBePickedUp == false)
        {
            Debug.Log("不能被拾取" + item.Item_Data.Name);
            return null;
        }

      /*  //如果被拾取时,CanbepickUp为false则不进行拾取
        if (item.GetData().CanBePickedUp==false)
        {
            Debug.Log("Can't be picked up"+item.GetData().Name);
            return null;
        }*/
        //物品被拾取时,物品的CanbepickUp属性设置为false
        item.Item_Data.CanBePickedUp = false;
        Destroy(item.gameObject);

        //返回物品信息
        return item.Item_Data;
    }
}
