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
        //��ȡ��Ʒ��Ϣ
        item = GetComponent<Item>();
    }

    public virtual ItemData Pickup()
    {
        if (item.Item_Data.CanBePickedUp == false)
        {
            Debug.Log("���ܱ�ʰȡ" + item.Item_Data.Name);
            return null;
        }

      /*  //�����ʰȡʱ,CanbepickUpΪfalse�򲻽���ʰȡ
        if (item.GetData().CanBePickedUp==false)
        {
            Debug.Log("Can't be picked up"+item.GetData().Name);
            return null;
        }*/
        //��Ʒ��ʰȡʱ,��Ʒ��CanbepickUp��������Ϊfalse
        item.Item_Data.CanBePickedUp = false;
        Destroy(item.gameObject);

        //������Ʒ��Ϣ
        return item.Item_Data;
    }
}
