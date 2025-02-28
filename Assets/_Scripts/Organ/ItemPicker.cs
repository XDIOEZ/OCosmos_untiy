using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    public Inventory AddTargetInventory;

    

    //标识是否可以拾取东西
    public bool canPickUp = true;

    public bool CanPickUp
    {
        get
        {
            if (AddTargetInventory == null)
            {
                if (AddTargetInventory.Inventory_Slots_All_IsFull == true)
                {
                    return  false;
                }
            }
            
            return canPickUp;
        }

        set
        {
            canPickUp = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (AddTargetInventory == null)
        AddTargetInventory = GetComponent<Inventory>();
    }

    //发生碰撞,获取碰撞物体上的ItemData并添加到Inventory中
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(AddTargetInventory == null)
        {
            Debug.LogWarning("ItemPicker:AddTargetInventory is null");
            return;
        }

        if (other.GetComponentInParent<ICanBePickUp>()!= null)
        {
            //玩家拾取物品
            Debug.Log($"ItemPicker:玩家:{transform.parent.name}拾取物品:{other.GetComponentInParent<ICanBePickUp>()}");
       


            if (AddTargetInventory.CanAddTheItem(other.GetComponentInParent<ICanBePickUp>().PickUp_ItemData))
            {
                AddTargetInventory.AddItem(other.GetComponentInParent<ICanBePickUp>().Pickup());
            }
           
        }
    }

}
