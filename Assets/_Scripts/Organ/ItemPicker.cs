using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    public Inventory AddTargetInventory;

    

    //��ʶ�Ƿ����ʰȡ����
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

    //������ײ,��ȡ��ײ�����ϵ�ItemData����ӵ�Inventory��
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(AddTargetInventory == null)
        {
            Debug.LogWarning("ItemPicker:AddTargetInventory is null");
            return;
        }

        if (other.GetComponentInParent<ICanBePickUp>()!= null)
        {
            //���ʰȡ��Ʒ
            Debug.Log($"ItemPicker:���:{transform.parent.name}ʰȡ��Ʒ:{other.GetComponentInParent<ICanBePickUp>()}");
       


            if (AddTargetInventory.CanAddTheItem(other.GetComponentInParent<ICanBePickUp>().PickUp_ItemData))
            {
                AddTargetInventory.AddItem(other.GetComponentInParent<ICanBePickUp>().Pickup());
            }
           
        }
    }

}
