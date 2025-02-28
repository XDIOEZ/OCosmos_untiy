using Force.DeepCloner;
using MemoryPack;
using System;
using System.Collections.Generic;
using UltEvents;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    public Item Belong_Item; // ������Ʒ
    public Inventory_Data Data = new Inventory_Data();
    public int MinStackVolume = 2;
    public UltEvent<int, ItemData> onDataChanged = new UltEvent<int, ItemData>(); // ����Ʒ�۷����仯ʱ�������¼�
    public UltEvent<int, ItemSlot> onSlotChanged = new UltEvent<int, ItemSlot>(); // ����Ʒ�۷����仯ʱ�������¼�
    public UltEvent<int> onUIChanged = new UltEvent<int>(); // �����������仯ʱ�������¼�

    //��ʶ_����Ƿ��Ѿ�����
    public bool Inventory_Slots_All_IsFull
    {
        get
        {
            for (int i = 0; i < Data.itemSlots.Count; i++)
            {
                if (Data.itemSlots[i].IsFull == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    #region  ��������
    public void Awake()
    {

        onSlotChanged += ChangeItemData_Default;//ע��Ĭ�Ͻ��������¼�
        onSlotChanged += (int _Index_, ItemSlot _itemSlot_Input_) =>
        {
            onDataChanged.Invoke(_Index_, _itemSlot_Input_._ItemData);
        };
    }
    public void Start()
    {
        //����Data.itemSlots
        foreach (ItemSlot itemSlot in Data.itemSlots)
        {
            //����inventory
            itemSlot.Belong_Inventory = this;
            //����slotindex
            itemSlot.Index = Data.itemSlots.IndexOf(itemSlot);
            itemSlot.SlotMaxVolume = 128;

        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
        }

        //̧��ʱ�ָ�Ĭ���¼�
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
        }
    }
    #endregion
    #region ��ɾ��
    public bool AddItem(ItemData Input_ItemData, int Index = 0)
    {
        if (Input_ItemData == null || !CanAddTheItem(Input_ItemData))
        {
            Debug.Log("��ƷΪ��||��Ʒ����");
            return false;
        }

        bool canStack = false;
        int stackIndex = -1;
        int emptyIndex = -1;

        // ����������ֻ�е�Input_ItemData.Volume < MinStackVolumeʱ������ѵ� 
        if (Input_ItemData.Volume < MinStackVolume)
        {
            // ����Ƿ���Զѵ�
            for (int i = 0; i < Data.itemSlots.Count; i++)
            {
                if (Data.itemSlots[i].IsFull || Data.itemSlots[i]._ItemData == null || Data.itemSlots[i]._ItemData.ItemSpecialData != Input_ItemData.ItemSpecialData)
                {
                    continue;
                }
                if (Data.itemSlots[i]._ItemData.PrefabPath == Input_ItemData.PrefabPath && Data.itemSlots[i]._ItemData.ItemSpecialData == Input_ItemData.ItemSpecialData)
                {
                    if (Data.itemSlots[i]._ItemData.CurrentVolume + Input_ItemData.CurrentVolume <= Data.itemSlots[i].SlotMaxVolume)
                    {
                        canStack = true;
                        stackIndex = i;
                        break;
                    }
                }
            }
        }

        // Ѱ�ҿ�λ
        if (!canStack)
        {
            for (int i = 0; i < Data.itemSlots.Count; i++)
            {
                if (Data.itemSlots[i]._ItemData == null)
                {
                    emptyIndex = i;
                    break;
                }
            }
        }

        // ������Ʒ����
        if (canStack)
        {
            Data.itemSlots[stackIndex]._ItemData.Stack.amount += Input_ItemData.Stack.amount;
            onDataChanged.Invoke(stackIndex, Input_ItemData);
            return true;
        }
        else if (emptyIndex != -1)
        {
            Data.itemSlots[emptyIndex]._ItemData = Input_ItemData;
            onDataChanged.Invoke(emptyIndex, Input_ItemData);
            return true;
        }
        else
        {
            Debug.Log("��������");
            return false;
        }
    }
    public bool CanAddTheItem(ItemData Input_ItemData)
    {
        if (Input_ItemData == null)
        {
            Debug.Log("��ƷΪ��");
            return false;
        }

        // ����������ֻ�е�Input_ItemData.Volume < MinStackVolumeʱ������ѵ� 
        if (Input_ItemData.Volume >= MinStackVolume)
        {
            // Ѱ�ҿ�λ 
            for (int i = 0; i < Data.itemSlots.Count; i++)
            {
                if (Data.itemSlots[i]._ItemData == null)
                {
                    
                    return true;
                }
            }
            Debug.Log("��������");
            return false;
        }

        // ԭ�еĶѵ��߼� 
        for (int i = 0; i < Data.itemSlots.Count; i++)
        {
            if (Data.itemSlots[i].IsFull || Data.itemSlots[i]._ItemData == null || Data.itemSlots[i]._ItemData.ItemSpecialData != Input_ItemData.ItemSpecialData)
            {
                continue;
            }
            if (Data.itemSlots[i]._ItemData.PrefabPath == Input_ItemData.PrefabPath && Data.itemSlots[i]._ItemData.ItemSpecialData == Input_ItemData.ItemSpecialData)
            {
                if (Data.itemSlots[i]._ItemData.CurrentVolume + Input_ItemData.CurrentVolume > Data.itemSlots[i].SlotMaxVolume)
                {
                    break;
                }

                return true;
            }
        }

        // ���û���ҵ��ɶѵ���λ�ã���Ѱ�ҿ�λ 
        for (int i = 0; i < Data.itemSlots.Count; i++)
        {
            if (Data.itemSlots[i]._ItemData == null)
            {

                return true;
            }
        }

        Debug.Log("��������");
        return false;
    }
    public void RemoveItem(ItemSlot itemSlot_, int Index = 0)
    {
        itemSlot_._ItemData = null;
        onUIChanged.Invoke(Index);
    }
    #region ChangeItemData_Default
    public void ChangeItemData_Default(int _Index_, ItemSlot InputSlot_Hand)
    {
        // Debug.Log("��ʼ������Ʒ");    
        // ����ǰ����Ʒ����
        ItemData _LocalData = Data.itemSlots[_Index_]._ItemData;
        ItemData _InputData_Hand = InputSlot_Hand._ItemData;
        //����ǰ�Ĳ������
        ItemSlot _InputSlot_Hand = InputSlot_Hand;
        ItemSlot _LocalSlot = Data.itemSlots[_Index_];
       

        /* if (InputSlot_Hand.Belong_Inventory.GetComponent<Inventory_Hand>().isGetHalf == true)
         {*/
        //����Ϊ��
        if (_InputData_Hand == null && _LocalData == null)
        {
            return;
        }

        #region ����������,����������
        if (_InputData_Hand != null && _LocalData == null)
        {
            float ChangeAmount;
           
            ChangeAmount = (int)Mathf.Ceil(_InputData_Hand.Stack.amount * InputSlot_Hand.Belong_Inventory.GetComponent<Inventory_Hand>().GetItemAmountRate);
            
            ChangeItemAmount(InputSlot_Hand, _LocalSlot, (int)ChangeAmount);
            onUIChanged.Invoke(_Index_);
            return;
            /* while (_InputSlot_Hand._ItemData.Stack.amount >= 0)
             {
                 if (_LocalSlot._ItemData == null)
                 {
                     Item_Data tempItemData = _InputData_Hand.DeepClone();
                     tempItemData.Stack.amount = 1;
                     _LocalSlot._ItemData = tempItemData;
                     _LocalSlot._ItemData.Stack.amount = 0;
                     Debug.Log("��¡��Ʒ���ݳɹ�" + _LocalSlot._ItemData.PrefabPath);
                 }
                 _LocalSlot._ItemData.Stack.amount += 1;
                 _InputSlot_Hand._ItemData.Stack.amount -= 1;

                 // ������ز�λ�����Ѿ�����,����ѭ��
                 if (_LocalSlot._ItemData.CurrentVolume >= LocalMaxVolume
                   || _InputData_Hand.Stack.amount <= 0 || _InputSlot_Hand._ItemData.Stack.amount <= ChangeAmount)
                 {
                     Debug.Log(_LocalSlot._ItemData.CurrentVolume);
                     Debug.Log(LocalMaxVolume);
                     Debug.Log(_InputData_Hand.Stack.amount);
                     Debug.Log(_InputSlot_Hand._ItemData.Stack.amount);
                     Debug.Log("(������ͬ)������Ʒ��λ:" + _Index_ + " ��Ʒ:" + _InputSlot_Hand._ItemData.PrefabPath);

                     //��������λ��ƷΪ��,��ˢ��UI
                     if (_InputData_Hand.Stack.amount <= 0)
                     {
                         InputSlot_Hand.ResetData();
                     }

                     onUIChanged.Invoke(_Index_);

                     return;
                 }



             }*/
        }
        #endregion

        #region ����������,����û����
        if (_LocalData != null && _InputData_Hand == null)
        {
            float ChangeAmount;
            ChangeAmount = (int)Mathf.Ceil(_LocalData.Stack.amount * InputSlot_Hand.Belong_Inventory.GetComponent<Inventory_Hand>().GetItemAmountRate);
            ChangeItemAmount(_LocalSlot, InputSlot_Hand, (int)ChangeAmount);
            onUIChanged.Invoke(_Index_);
            return;
        }
        #endregion

        #region ���⽻��

        if (_LocalSlot._ItemData.Volume > MinStackVolume || _LocalSlot._ItemData.ItemSpecialData != _InputSlot_Hand._ItemData.ItemSpecialData)
        {
            Debug.Log("���⽻��");
            _LocalSlot.Change(_InputSlot_Hand);
            onUIChanged.Invoke(_Index_);
            return;
        }
        #endregion

        #region ��Ʒ��ͬ
        if (_InputSlot_Hand._ItemData.Name == _LocalSlot._ItemData.Name)
        {
            float ChangeAmount;
            ChangeAmount = (int)Mathf.Ceil(_LocalSlot._ItemData.Stack.amount * InputSlot_Hand.Belong_Inventory.GetComponent<Inventory_Hand>().GetItemAmountRate);
            ChangeItemAmount(_LocalSlot, InputSlot_Hand, (int)ChangeAmount);
            onUIChanged.Invoke(_Index_);
            return;
        }
        #endregion

        #region ���߲�Ϊ������Ʒ����ͬ
        if (_InputData_Hand != null && _LocalData != null && _LocalData.PrefabPath != _InputData_Hand.PrefabPath)
        {
            /*Debug.Log("���߷ǿտ�ʼ����");
            if (_InputData_Hand.Stack.amount * _InputData_Hand.Volume > _LocalSlot.SlotMaxVolume)
            {
                Debug.Log(" ����ʧ��, �������");
                return;
            }*/

            _LocalSlot.Change(_InputSlot_Hand);

            onUIChanged.Invoke(_Index_);

            Debug.Log("(��Ʒ��ͬ)������Ʒ��λ:" + _Index_ + " ��Ʒ:" + _InputSlot_Hand._ItemData.PrefabPath);

            return;
        }
        #endregion

      




       /* }
        else
        {
            #region ���߲�۲�Ϊ��,����Ʒ��ͬ
            // �����������Ʒ��ͬ
            if (_LocalSlot._ItemData != null && _InputSlot_Hand._ItemData != null && _InputSlot_Hand._ItemData.PrefabPath == _LocalSlot._ItemData.PrefabPath)
            {
                Debug.Log("��ͬ��Ʒ��ʼ����");
                if (_LocalSlot._ItemData.Volume > MinStackVolume || _LocalSlot._ItemData.ItemSpecialData != _InputSlot_Hand._ItemData.ItemSpecialData)
                {
                    _LocalSlot.Change(_InputSlot_Hand);
                    onUIChanged.Invoke(_Index_);
                    return;
                }
                while (true)
                {
                    // ������ز�λ�����Ѿ�����,����ѭ��
                    if (_LocalData.Stack.amount <= 0)
                    {
                        _LocalSlot.ResetData();
                        onUIChanged.Invoke(_Index_);
                        return;
                    }

                    Debug.Log("���ز�λ����δ��+1");
                    _LocalData.Stack.amount -= 1;

                    _InputData_Hand.Stack.amount += 1;
                }
            }
            #endregion
            #region ���ϲ��ǿյ�,����Ĳ���ǿյ�
            //���ؿ�λ_����ǿ�
            if (_InputData_Hand != null && _LocalData == null)
            {


                while (_InputSlot_Hand._ItemData.Stack.amount >= 0)
                {


                    if (_LocalSlot._ItemData == null)
                    {

                        //TODO ��¡��Ʒ����,��������
                        Item_Data tempItemData = _InputData_Hand.DeepClone();
                        tempItemData.Stack.amount = 1;
                        _LocalSlot._ItemData = tempItemData;

                        _LocalSlot._ItemData.Stack.amount = 0;
                        Debug.Log("��¡��Ʒ���ݳɹ�" + _LocalSlot._ItemData.PrefabPath);


                    }




                    _LocalSlot._ItemData.Stack.amount += 1;

                    _InputSlot_Hand._ItemData.Stack.amount -= 1;

                    // ������ز�λ�����Ѿ�����,����ѭ��
                    if (_LocalSlot._ItemData.Volume * _LocalSlot._ItemData.Stack.amount >= LocalMaxVolume
                      || _InputData_Hand.Stack.amount <= 0)
                    {

                        Debug.Log("(������ͬ)������Ʒ��λ:" + _Index_ + " ��Ʒ:" + _InputSlot_Hand._ItemData.PrefabPath);

                        //��������λ��ƷΪ��,��ˢ��UI
                        if (_InputData_Hand.Stack.amount <= 0)
                        {
                            InputSlot_Hand.ResetData();
                        }

                        onUIChanged.Invoke(_Index_);

                        return;
                    }



                }


            }
            #endregion
            #region ���طǿ�,����Ϊ��
            if (_LocalData != null && _InputData_Hand == null)
            {
                // Debug.Log("�����λ_��ʼ����");

                _LocalSlot.Change(_InputSlot_Hand);


            }
            #endregion
            #region ���߲�Ϊ������Ʒ����ͬ
            if (_InputData_Hand != null && _LocalData != null && _LocalData.PrefabPath != _InputData_Hand.PrefabPath)
            {
                Debug.Log("���߷ǿտ�ʼ����");
                *//*if (_InputData_Hand.Stack.amount * _InputData_Hand.Volume > _LocalSlot.SlotMaxVolume)
                {
                    Debug.Log(" ����ʧ��, �������");
                    return;
                }*//*

                _LocalSlot.Change(_InputSlot_Hand);

                onUIChanged.Invoke(_Index_);

                Debug.Log("(��Ʒ��ͬ)������Ʒ��λ:" + _Index_ + " ��Ʒ:" + _InputSlot_Hand._ItemData.PrefabPath);

                return;
            }
            #endregion
        }*/
    }
    #endregion
    /// <summary>
    /// �޸�����
    /// </summary>
    /// <param name="_LocalSlot">���ٵĲ�λ</param>
    /// <param name="_InputSlot_Hand">���ӵĲ�λ</param>
    /// <param name="amount">�޸ĵ�����</param>
    public bool ChangeItemAmount(ItemSlot _LocalSlot, ItemSlot _InputSlot_Hand, int Count)
    {
        int ChangeCount = 0;
       // Debug.Log("��ͬ��Ʒ��ʼ����");

        if (_InputSlot_Hand._ItemData == null)
        {
            ItemData tempItemData = _LocalSlot._ItemData.DeepClone();
            tempItemData.Stack.amount = 1;
            _InputSlot_Hand._ItemData = tempItemData;
            _InputSlot_Hand._ItemData.Stack.amount = 0;
           // Debug.Log("��¡��Ʒ���ݳɹ�" + _LocalSlot._ItemData.PrefabPath);
        }

        if (_LocalSlot._ItemData.ItemSpecialData != _InputSlot_Hand._ItemData.ItemSpecialData)
        {
            return false;
        }
        while (true)
        {
            
            _LocalSlot._ItemData.Stack.amount -= 1;
            ChangeCount++;
            _InputSlot_Hand._ItemData.Stack.amount += 1;

            // ������ز�λ�����Ѿ�����,����ѭ��
            if (ChangeCount >= Count || _LocalSlot._ItemData.Stack.amount <= 0|| _InputSlot_Hand._ItemData.Stack.amount >= _InputSlot_Hand.SlotMaxVolume)
            {
               /* Debug.Log(_InputSlot_Hand._ItemData.CurrentVolume >= _InputSlot_Hand.SlotMaxVolume);
                Debug.Log(_LocalSlot._ItemData.Stack.amount <= 0);
                Debug.Log(_LocalSlot._ItemData.Stack.amount <= Count);
                Debug.Log(_InputSlot_Hand._ItemData.Stack.amount);
                Debug.Log(_InputSlot_Hand.SlotMaxVolume);*/

                if (_LocalSlot._ItemData.Stack.amount <= 0)
                {
                    _LocalSlot.ResetData();
                }
                return true;
            }

        }
    }

    public void SetOneItem(int Index, ItemData Input_ItemData)
    {

    }
    #endregion
    #region  �����Ʒ��ѯ
    public ItemSlot GetItemSlot(int Index)
    {
        //�������������Χ,��ȡ���ֵ
        if (Index >= Data.itemSlots.Count)
        {
            Index = Data.itemSlots.Count - 1;
        }
        //Debug.Log("��ȡ��Ʒ��λ:" + _Index_);
        return Data.itemSlots[Index];
    }
    public ItemData GetItemData(int Index)
    {
        return GetItemSlot(Index)._ItemData;
    }
    #endregion
}

[System.Serializable]
public class Inventory_Data
{
    public string inventoryName = string.Empty;//��������

    public List<ItemSlot> itemSlots = new List<ItemSlot>();//������Ʒ���б�
}

public class CloneItem<T> where T : ItemData
{
    public ItemData Clone(T obj)
    {
        // �� Item_Data ���л�Ϊ�ֽ�����
        byte[] serializedData = MemoryPackSerializer.Serialize(obj);

        // �����л�Ϊ�µ� Item_Data ����
        ItemData tempItemData = MemoryPackSerializer.Deserialize<ItemData>(serializedData);

        return tempItemData;
    }
}
