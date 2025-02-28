using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_RightClick_ItemInfo : MonoBehaviour
{
    public ScrollRect itemInfoPanel;//�������
    public GameObject ScrollContent;//������� ��Ϊ��ť�ĸ�����
    public ItemData SelectedItemData;//�Ҽ�����������Ʒ����
    public GameObject ButtonPrefab;//��ťԤ����
    public Button UseButton;//ʹ�ð�ť
    public Button InfoButton;//��Ϣ��ť
    public Button DiscardButton;//������ť
    public Button CloseButton;//�رհ�ť
    public CanvasGroup CanvasGroup;
    public Item Belong_Player;

    private void Start()
    {
        CloseButton.onClick.AddListener(CloseMenu);
        CanvasGroup = GetComponent<CanvasGroup>();
    }
    public void SetItemData(ItemData itemData)
    {

    }
    public void ShowMenu()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.interactable = true;
    }
    public void SetItemInfo(ItemSlot itemSlot)
    {
        ShowMenu() ;
        transform.position = itemSlot.UI.transform.position;
        Debug.Log("���Ҽ��˵�" + itemSlot._ItemData.Name);
        //ʹ��
        UseButton.onClick.RemoveAllListeners();
        UseButton.onClick.AddListener(() => { CreateAndUseItem(itemSlot); });

        //��Ʒ��Ϣ

        //����
    }
    public void CloseMenu()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.interactable = false;
    }

    void CreateAndUseItem(ItemSlot itemSlot)
    {
        if(itemSlot._ItemData == null)
        {
            Debug.Log("��ƷΪ�գ�");
            return;
        }
        XDTool.InstantiateAddressableAsync(itemSlot._ItemData.PrefabPath,transform.position, Quaternion.identity,
                (newObject) =>
                {
                    if (newObject != null)
                    {
                        //ʵ��������
                        Item newItem = newObject.GetComponent<Item>();
                        newItem.Item_Data = itemSlot._ItemData;
                        //ʹ������
                        newItem.Use();

                        //�ж��Ƿ�ΪӪ����Ʒ
                        if (newItem is INutrient && Belong_Player is INutrient)
                        {
                            INutrient nutrient_er = Belong_Player.GetComponent<INutrient>();
                            nutrient_er.Eat(newItem as INutrient);
                        }
                     

                        Destroy(newItem.gameObject);

                        Debug.Log("������ʹ����Ʒ��" + itemSlot._ItemData.Name);

                        itemSlot._ItemData.Stack.amount--;
                        itemSlot.UI.RefreshUI();

                      
                        if (itemSlot._ItemData.Stack.amount <= 0)
                        {
                            itemSlot.ResetData();
                            CloseMenu();
                        }
                        

                    }
                    else
                    {
                        Debug.LogError("ʵ����������Ϊ�գ�");
                    }
                },
                (error) =>
                {
                    Debug.LogError($"ʵ����ʧ��: {itemSlot._ItemData.PrefabPath}, ������Ϣ: {error.Message}");
                }
);
    }

    
    
}
