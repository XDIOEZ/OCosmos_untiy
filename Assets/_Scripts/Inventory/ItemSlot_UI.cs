using Sirenix.OdinInspector;
using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot_UI : MonoBehaviour, IPointerDownHandler,IScrollHandler
{
    [ShowInInspector]
    public ItemSlot itemSlot; // �����۵�����
    public Image image; // ��ʾ��ǰ�����ͼ��
    public TMP_Text text; // ��ʾ��ǰ���������
    public Slider slider; // �������ڵ��������;öȣ�������ʾ
    public Button button; // ����İ�ť
    public UltEvent onItemClick; // ���屻������¼�
    public UltEvent<Vector2> onItemScroll; // ���屻���ֵ��¼�

    private void Awake()
    {
        // ���Ϊ�� ��ȡ �Ӷ��� ��image��text���
        // ������һ��������
        image ??= GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();
        slider = GetComponentInChildren<Slider>();
        // ������ť
        button = GetComponent<Button>();
    }

    void Start()
    {/*
        // ������ť�ĵ���¼�
        button.onClick.AddListener(() =>
        {
            onItemClick.Invoke();
        });*/
        itemSlot.UI = this; // ��UI���
    }

    // �ͷ�Button�ļ���
    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }




    // ����UI����
    [Button]
    public void RefreshUI()
    {
        // ��������
        UpdateItemAmount();

        // ����ͼ��
        UpdateItemIcon();
    }

    // ��װ���������ķ���
    private void UpdateItemAmount()
    {
        //�����Ʒ�Ƿ�Ϊ��
        if ( itemSlot == null ||itemSlot._ItemData == null|| itemSlot._ItemData.Stack == null|| itemSlot._ItemData.Stack.amount == 0)
        {
            //��ӡ��ʲô����Ϊ��
            if (itemSlot == null)
            {
                //Debug.LogWarning("��Ʒ��Ϊ��");
                return;
            }
            else if (itemSlot._ItemData == null)
            {
                //Debug.LogWarning("��Ʒ����Ϊ��");
                return;
               
            }
            else if (itemSlot._ItemData.Stack == null)
            {
               // Debug.LogWarning("��Ʒ�ѵ�Ϊ��");
                return;
            }
            /*else if (itemSlot._ItemData.Stack.amount == 0)
            {
                Debug.LogWarning("��Ʒ����Ϊ0");
            }*/
        }
        int itemAmount = (int)itemSlot._ItemData.Stack.amount;
        // �������Ϊ�㣬�����������ı�
        if (itemAmount == 0)
        {
            text.gameObject.SetActive(false);  // ��������
        }
        else
        {
            // ��ʾ���������
            text.text = itemAmount.ToString();
            text.gameObject.SetActive(true);  // ��ʾ����
        }
    }

    // ��װ����ͼ��ķ���
    private void UpdateItemIcon()
    {
        // �����Ʒ����Ϊ�գ�ֱ�ӷ���
        if (itemSlot._ItemData == null || itemSlot._ItemData.PrefabPath == "")
        {
            image.gameObject.SetActive(false);  // ����ͼ��
            return;
        };
        
        
      /*  // ��ʾ�����ͼ��
        XDTool.LoadAssetAsync<Sprite>(itemSlot._ItemData.iconPath, (sprite) =>
        {
            image.sprite = sprite;
        });*/
        XDTool.InstantiateAddressableAsync(itemSlot._ItemData.PrefabPath,transform.position, transform.rotation, (go) =>
        {
            image.sprite = go.GetComponentInChildren<SpriteRenderer>().sprite;
            Destroy(go);
        });

        // ��ʾͼ��
        image.gameObject.SetActive(true);
    }
    
    //TODO �������ֿ�����Ʒʹ��
    public void Use(PointerEventData eventData)
    {
         // ����Ƿ���������
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onItemClick.Invoke();
        }
        //����Ƿ�������Ҽ�
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(itemSlot._ItemData!= null)
            itemSlot.Belong_Inventory.Belong_Item.GetComponent<PlayerController>()._playerUI.RightClickMenuCanvas
           .GetComponent<Menu_RightClick_ItemInfo> ().SetItemInfo(itemSlot);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Use(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        //���Ϲ���
        if (eventData.scrollDelta.y > 0)
        {
            //��������Ʒ
            onItemClick.Invoke();
        }
        //���¹���
        if (eventData.scrollDelta.y < 0)
        { 
           //�������е���Ʒ
        
        }
    }
}
