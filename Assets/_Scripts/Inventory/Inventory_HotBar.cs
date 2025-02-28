using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_HotBar : MonoBehaviour
{
    // A. _inventory_UI ����
    public Inventory_UI _inventory_UI;

    // B. Inventory ����
    private Inventory inventory;

    // C. Index ʵ�������
    public int CurrentIndex = 1;

    // D. ��������������ֵ
    public int maxIndex;

    // E. ʵ���������λ��
    public Transform spawnLocation;

    // ��ǰʵ������ GameObject
    public GameObject currentObject;

    public GameObject SelectBoxPrefab;

    public GameObject currentSelectBox;

    public int HotBarMaxVolume = 9;

    // ʵ��������
    private void Awake()
    {
        _inventory_UI = GetComponent<Inventory_UI>();
        inventory = _inventory_UI.inventory;
        maxIndex = inventory.Data.itemSlots.Count;
        //inventory.Data.itemSlots.Add(new ItemSlot(inventory));
    }
    private void Start()
    {
        _inventory_UI.inventory.onDataChanged += (int i, ItemData itemData) => ChangeIndex(i);
        //ʵ����SelectBoxPrefab ��_inventory_UI.itemSlots_UI[CurrentIndex].gameObject����������
        currentSelectBox = Instantiate(SelectBoxPrefab, _inventory_UI.itemSlots_UI[CurrentIndex].gameObject.transform);
        ChangeIndex(0);

        foreach(ItemSlot itemSlot in inventory.Data.itemSlots)
        {
            itemSlot.SlotMaxVolume = HotBarMaxVolume;
        }
    }

    //


    // �ı���������
    [Button]
    public void ChangeIndex(int newIndex)//1
    {
        //Debug.Log("���������еĿ������Ʒ����:" + CurrentIndex + " ������:" + newIndex);
        //TODO���������͸�Ϊ����һͷ��ʼ,ʵ��ѭ��
        if (newIndex < 0)
        {
            newIndex = maxIndex - 1;
        }
        else if (newIndex >= maxIndex)
        {
            newIndex = 0;
        }
        CurrentIndex = newIndex;
        GameObject temp = _inventory_UI.itemSlots_UI[CurrentIndex].gameObject;
        //��currentSelectBox����Ϊtemp��������
        currentSelectBox.transform.SetParent(temp.transform, false);
        //��currentSelectBox��λ������Ϊtemp��λ��
        currentSelectBox.transform.localPosition = Vector3.zero;

        ChangeNewObject(newIndex);

    }


    private void DestroyCurrentObject()
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
            currentObject = null;
        }
    }
    private void ChangeNewObject(int __index)
    {
        // ���ٵ�ǰ����
        DestroyCurrentObject();

        // ��������Ƿ���Ч
        if (__index >= 0 && __index <= maxIndex)
        {
            if (inventory.GetItemSlot(__index)._ItemData == null)
            {
                return;
            }
            // ��ȡ��Ʒ��Prefab·��
            string itemPath = inventory.GetItemSlot(__index)._ItemData.PrefabPath;

            // ���·���Ƿ�Ϊ��
            if (string.IsNullOrEmpty(itemPath))
            {
                //Debug.LogError("��Ʒ·��Ϊ�գ��޷��������壡");
                return;
            }

            // �첽����Ԥ����
            XDTool.InstantiateAddressableAsync(itemPath,spawnLocation.position,Quaternion.identity,
                (newObject) =>
                {
                    if (newObject != null)
                    {
                        // ���ø�����
                        newObject.transform.SetParent(spawnLocation, false);
              
                        // �޸ı���λ�ã�ȷ��λ��Ϊ (0, 0, 0)
                        newObject.transform.localPosition = Vector3.zero;
              
                        // �޸ı�����ת��ȷ�� Z ��Ϊ 0
                        Vector3 localRotation = newObject.transform.localEulerAngles;

                        localRotation.z = 0;

                        newObject.transform.localEulerAngles = localRotation;
              
                        // ��ֵ��ǰ����
                        currentObject = newObject;
              
                        // ��ȡ��Ʒ���ݲ�����
                        ItemData itemData = inventory.GetItemSlot(__index)._ItemData;
                        currentObject.GetComponent<Item>().Item_Data =(inventory.GetItemData(__index));
                    }
                    else
                    {
                        Debug.LogError("ʵ����������Ϊ�գ�");
                    }
                },
                (error) =>
                {
                    Debug.LogError($"ʵ����ʧ��: {itemPath}, ������Ϣ: {error.Message}");
                }
);
        }


    }
}