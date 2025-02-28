using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UltEvents;

public class Inventory_UI : MonoBehaviour
{
    public Inventory inventory; // ���õı�������
    public List<ItemSlot_UI> itemSlots_UI = new List<ItemSlot_UI>(); // �洢���в�۵�UI
    public GameObject itemSlot_UI_prefab; // Ԥ����

    public SelectSlot TargetSendItemSlot; // ��������Ʒ�۵�ѡ����

    public UltEvent<int> OnUIChanged
    {
        get
        {
            return inventory.onUIChanged;
        }

        set
        {
            inventory.onUIChanged = value;
        }
    }

    /*    public SelectSlot TargetSendItemSlot
        {
            get
            {
                if (_targetSendItemSlot == null)
                {
                    //ͨ�����ֲ�������
                    _targetSendItemSlot = GameObject.Find("TargetSendItemSlot").GetComponent<SelectSlot>();


                }
                return _targetSendItemSlot;
            }

            set
            {
                _targetSendItemSlot = value;
            }
        }*/

    public void Awake()
    {
        if (inventory == null)
        inventory = GetComponent<Inventory>(); // ��ȡ��������
        TargetSendItemSlot = GameObject.Find("TargetSendItemSlot").GetComponent<SelectSlot>();
    }
    private void Start()
    {
        if(inventory.Data.inventoryName == "")
        {
            inventory.Data.inventoryName = gameObject.name;
        }
        OnUIChanged += RefreshSlotUI; // �����������仯ʱˢ��UI


        inventory. onDataChanged +=  (int index , ItemData itemData) => OnUIChanged.Invoke(index);


        InstantiateItemSlots(); // ��ʼ��ʵ��������ItemSlot_UI
        /*        transform.SetParent(BelongTO, false); // ���ø�����*/
       
   }
    private void OnDestroy()
    {
        OnUIChanged -= RefreshSlotUI; // �����������仯ʱˢ��UI
    }
    [Button]
    public void InstantiateItemSlots()
    {
        // ���ԭ�е� ItemSlot_UI�������Ҫ������ݣ�
        itemSlots_UI.Clear();

        // ����Ѿ����㹻������ ItemSlot_UI��ֱ�Ӹ��£�����Ҫ����ʵ����
        if (itemSlots_UI.Count >= inventory.Data.itemSlots.Count)
        {
            // �����Ѵ��ڵ� ItemSlot_UI
            RefreshAllInventory();
            return;
        }

        // ����Ӷ��������Ҫ��ȫ�滻UI�������پɵ�
        foreach (Transform child in transform)
        {
            //���Ԥ����ָ��

            //�ڱ༭��ģʽ��ɾ��
           // DestroyImmediate(child.gameObject);
            Destroy(child.gameObject);
        }

        // ʵ�������������� ItemSlot_UI
        for (int i = itemSlots_UI.Count; i < inventory.Data.itemSlots.Count; i++)
        {
            // ʵ�����µ� ItemSlot_UI ������Ϊ�Ӷ���
            GameObject itemSlot_UI_go = Instantiate(itemSlot_UI_prefab, transform);
            // �洢�µ� ItemSlot_UI
            itemSlots_UI.Add(itemSlot_UI_go.GetComponent<ItemSlot_UI>());
        }

        // ��Ӱ�ť������
        AddListenersToItemSlots();
        // ˢ�� UI
        RefreshAllInventory();
    }
    // ��ʼ��UI�ķ���
    public void RefreshAllInventory()
    {
        for (int i = 0; i < itemSlots_UI.Count; i++)
        {
            //��UI�ı��������л�����Ϊ��ʵ��������
            itemSlots_UI[i].itemSlot = inventory.Data.itemSlots[i];
            //ˢ��UI
            OnUIChanged.Invoke(i);
        }
    }

    public void RefreshSlotUI(int index)
    {
        //����������Χ,�����һ����λˢ��UI
        if (index >= itemSlots_UI.Count)
        {
            index = itemSlots_UI.Count - 1;
        }
      
        itemSlots_UI[index].RefreshUI();
    }
    //TODO �����б���ȫ����ItemSlot_UI�е�Button
    public void AddListenersToItemSlots()
    {
        //Debug.Log("Add listeners to Item Slots" + itemSlots_UI.Count);
        for (int i = 0; i < itemSlots_UI.Count; i++)
        {
            int index = i;
            var onItemClick = itemSlots_UI[index].onItemClick;
            var onItemScroll = itemSlots_UI[index].onItemScroll;
            if (onItemClick != null)
            {
                onItemClick.Clear(); // ����ɵļ�����
                onItemClick += () => OnItemSlotClicked(index); // ����µļ�����
            }
            if (onItemScroll != null)
            {
                onItemScroll.Clear(); // ����ɵļ�����
                onItemScroll += (v) => OnItemSlotScrolled(index,v); // ����µļ�����
            }
            
        }
    }

    //�����¼�
   void  OnItemSlotScrolled(int _index_,Vector2 _direction_)
    {
        Debug.Log("OnItemSlotScrolled"+_index_+" "+_direction_);
    }

    //��ť����¼�
    private void OnItemSlotClicked(int _index_)
    {
        inventory.onSlotChanged?.Invoke(_index_,TargetSendItemSlot.HandInventoryUI.inventory.GetItemSlot(_index_));
        //ˢ�����ϲ�۵�UI
        TargetSendItemSlot.HandInventoryUI.OnUIChanged.Invoke(_index_);
    }

}