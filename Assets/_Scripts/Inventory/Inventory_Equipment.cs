using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class Inventory_Equipment : MonoBehaviour
{
    // Start is called before the first frame update
    public Inventory inventory;
    public List<ItemType> EquipmentTypes = new List<ItemType>();
    //���׷���������ʵ��
    public IReceiveDamage Protect_entity;

    //ʵ��Ѫ��
    public Hp Hp
    {
        get
        {
            return Protect_entity.Hp;
        }

        set
        {
            Protect_entity.Hp = value;
        }
    }
    //�ҽ�ʵ�����
    public Defense Entity_Defense
    {
        get
        {
            return Protect_entity.DefenseValue;
        }

        set
        {
            Protect_entity.DefenseValue = value;
        }
    }

    void Awake()
    {
        inventory = GetComponent<Inventory>();
        inventory.Data.inventoryName = "װ�����";
        inventory.Data.itemSlots = new List<ItemSlot>();

        // ��ʼ����λ��������������ʵ����
        for (int i = 0; i < 4; i++)
        {
            ItemSlot slot = new ItemSlot(inventory);
            slot._ItemData = null; // ��ʼΪ��
            inventory.Data.itemSlots.Add(slot);
        }


        //�������в�λ��BseTypeΪArmor
        for (int i = 0; i < inventory.Data.itemSlots.Count; i++)
        {
            inventory.Data.itemSlots[i].CanAcceptItemType.Type_Base = ItemType.Armor;
        }

        /*        // ��ʼ��װ������
                EquipmentTypes = new List<string> { "Armor_Hand", "Armor_Head", "Armor_Chest", "Armor_Legs" };*/

        // �������в�λ������������
        for (int i = 0; i < inventory.Data.itemSlots.Count; i++)
        {
            if (i < EquipmentTypes.Count)
            {
                inventory.Data.itemSlots[i].CanAcceptItemType.Type_Sub = EquipmentTypes[i];
            }
            else
            {
                Debug.LogWarning("װ�������б��е��������ڲ�λ������");
                break;
            }
        }

        inventory.onSlotChanged += EquipArmor;
    }


    void Start()
    {
        //damageReceiver = GetComponentInParent<PlayerUIManager>().CanvasBelong_Item.GetComponentInChildren<DamageReceiver>();

        Protect_entity = GetComponentInParent<PlayerUIManager>().CanvasBelong_Item.GetComponent<IReceiveDamage>();

        inventory.onSlotChanged -= inventory.ChangeItemData_Default;
    }

    public void EquipArmor(int slotIndex, ItemSlot InputSlot)
    {
        inventory.onSlotChanged -= inventory.ChangeItemData_Default;

        //Debug.Log("��ʼװ��");

        if (Protect_entity == null)
        {
            Debug.LogError("δ�ҵ� DamageReceiver �����");
            return;
        }

        //TODO:����ֺ�װ����Ϊ��,�����κβ���
        if (InputSlot._ItemData == null && inventory.GetItemData(slotIndex) == null)
        {
            return;
        }

        //ж�¿���
        if (inventory.GetItemData(slotIndex) != null)//���������װ��
        {
            Debug.Log("�����װ�� || ������װ��"); 
            if(InputSlot._ItemData==null)//���������Ϊ��
            {
                ArmorData armorData = (ArmorData)inventory.GetItemData(slotIndex);

                Entity_Defense -= armorData.defense;

                inventory.ChangeItemData_Default(slotIndex, InputSlot);
                inventory.onUIChanged?.Invoke(slotIndex);
                return;
            }
        }

        //ǰ�����ֲ�Ϊ��,�����װ���򽻻����{AddTargetInventory.ChangeItemData_Default( slotIndex, InputSlot );AddTargetInventory.onUIChanged?.Invoke(slotIndex); }
        //�������Ҳ��װ��,�򽻻�װ��


        //װ������
        if (InputSlot._ItemData.ItemTags.Type_Base == ItemType.Armor)
        {
            Debug.Log("�����װ��|| ������װ��");
            if (InputSlot._ItemData.ItemTags.Item_TypeTag[0] == inventory.Data.itemSlots[slotIndex].CanAcceptItemType.Item_TypeTag[0])
            {
               

                ArmorData armorData = (ArmorData)InputSlot._ItemData;

               Entity_Defense += armorData.defense;

                inventory.ChangeItemData_Default(slotIndex, InputSlot);
                // AddTargetInventory.onUIChanged?.Invoke(slotIndex);
                Debug.Log("װ����: " + inventory.Data.itemSlots[slotIndex]._ItemData.Name + "������ֵΪ " + Entity_Defense + "��");
                return;
            }
        }


        Debug.Log("װ���Ĳ��ǿ��ס�����:" + InputSlot._ItemData.ItemTags.Type_Base);
        Debug.Log(slotIndex + " �Ų�λ��װ���������λ���Ͳ�ƥ�䡣");
        Debug.Log("װ���ķ����������λ�������Ͳ�ƥ�䡣" + InputSlot._ItemData.ItemTags.Type_Sub + " �� " + inventory.Data.itemSlots[slotIndex].CanAcceptItemType.Type_Sub);
    } 
}


