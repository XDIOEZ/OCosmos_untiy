using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_HotBar : MonoBehaviour
{
    // A. _inventory_UI 引用
    public Inventory_UI _inventory_UI;

    // B. Inventory 数据
    private Inventory inventory;

    // C. Index 实例化序号
    public int CurrentIndex = 1;

    // D. 快捷栏索引的最大值
    public int maxIndex;

    // E. 实例化物体的位置
    public Transform spawnLocation;

    // 当前实例化的 GameObject
    public GameObject currentObject;

    public GameObject SelectBoxPrefab;

    public GameObject currentSelectBox;

    public int HotBarMaxVolume = 9;

    // 实例化方法
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
        //实例化SelectBoxPrefab 到_inventory_UI.itemSlots_UI[CurrentIndex].gameObject的子物体下
        currentSelectBox = Instantiate(SelectBoxPrefab, _inventory_UI.itemSlots_UI[CurrentIndex].gameObject.transform);
        ChangeIndex(0);

        foreach(ItemSlot itemSlot in inventory.Data.itemSlots)
        {
            itemSlot.SlotMaxVolume = HotBarMaxVolume;
        }
    }

    //


    // 改变索引方法
    [Button]
    public void ChangeIndex(int newIndex)//1
    {
        //Debug.Log("交换了现有的快捷栏物品索引:" + CurrentIndex + " 新索引:" + newIndex);
        //TODO超出索引就改为从另一头开始,实现循环
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
        //将currentSelectBox设置为temp的子物体
        currentSelectBox.transform.SetParent(temp.transform, false);
        //将currentSelectBox的位置设置为temp的位置
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
        // 销毁当前物体
        DestroyCurrentObject();

        // 检查索引是否有效
        if (__index >= 0 && __index <= maxIndex)
        {
            if (inventory.GetItemSlot(__index)._ItemData == null)
            {
                return;
            }
            // 获取物品的Prefab路径
            string itemPath = inventory.GetItemSlot(__index)._ItemData.PrefabPath;

            // 检查路径是否为空
            if (string.IsNullOrEmpty(itemPath))
            {
                //Debug.LogError("物品路径为空，无法加载物体！");
                return;
            }

            // 异步加载预制体
            XDTool.InstantiateAddressableAsync(itemPath,spawnLocation.position,Quaternion.identity,
                (newObject) =>
                {
                    if (newObject != null)
                    {
                        // 设置父对象
                        newObject.transform.SetParent(spawnLocation, false);
              
                        // 修改本地位置，确保位置为 (0, 0, 0)
                        newObject.transform.localPosition = Vector3.zero;
              
                        // 修改本地旋转，确保 Z 轴为 0
                        Vector3 localRotation = newObject.transform.localEulerAngles;

                        localRotation.z = 0;

                        newObject.transform.localEulerAngles = localRotation;
              
                        // 赋值当前对象
                        currentObject = newObject;
              
                        // 获取物品数据并设置
                        ItemData itemData = inventory.GetItemSlot(__index)._ItemData;
                        currentObject.GetComponent<Item>().Item_Data =(inventory.GetItemData(__index));
                    }
                    else
                    {
                        Debug.LogError("实例化的物体为空！");
                    }
                },
                (error) =>
                {
                    Debug.LogError($"实例化失败: {itemPath}, 错误信息: {error.Message}");
                }
);
        }


    }
}