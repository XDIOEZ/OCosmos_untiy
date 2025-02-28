using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDroper : MonoBehaviour
{
    public Inventory DroperInventory;
    public ItemSlot ItemToDrop_Slot;
    public Transform DropPos_UI;
    public Vector3 dropPos;
    public int _Index = 0;

    [Tooltip("抛物线的最大高度")]
    public float parabolaHeight = 2f; // 可以保留但设为只读 
    [Tooltip("基础抛物线动画持续时间")]
    public float baseDropDuration = 0.5f;
    [Tooltip("距离敏感度，用于调整动画时间随距离的变化速度")]
    public float distanceSensitivity = 0.1f;

    [Button("DropItemByList")]
    public void DropItem()
    {
        Debug.Log("DropItemByList");

        if (DroperInventory == null)
        {
            Debug.LogError("DroperInventory 未设置！");
            return;
        }

        ItemToDrop_Slot = DroperInventory.GetItemSlot(_Index);

        if (ItemToDrop_Slot == null)
        {
            Debug.Log("没有物品可以丢弃！");
            return;
        }

        if (string.IsNullOrEmpty(ItemToDrop_Slot._ItemData.PrefabPath))
        {
            Debug.LogError("物品预制体路径为空！");
            return;
        }

        XDTool.InstantiateAddressableAsync(
    ItemToDrop_Slot._ItemData.PrefabPath,
    transform.position,
    Quaternion.identity,
    (newObject) =>
    {
        if (newObject == null)
        {
            Debug.LogError("实例化物体失败！");
            return;
        }

        Debug.Log("Instantiate new object: " + newObject.name);
        print(newObject.transform.position);
        Item newItem = newObject.GetComponent<Item>();
        if (newItem == null)
        {
            Debug.LogError("物体中未找到 Item 组件！");
            Destroy(newObject);
            return;
        }

        newItem.Item_Data =(ItemToDrop_Slot._ItemData);
        newItem.Item_Data.CanBePickedUp = false; // 动画期间不可捡起 

        // 计算掉落位置
        dropPos = Camera.main.ScreenToWorldPoint(DropPos_UI.position);
        dropPos = new Vector3(dropPos.x, dropPos.y, 0);

        // 启动抛物线动画
        StartCoroutine(ParabolaAnimation(
            newObject.transform,
            transform.position,
            dropPos,
            newItem
        ));

        // 移除背包中的物品
        DroperInventory.RemoveItem(ItemToDrop_Slot, _Index);
    },
    (error) =>
    {
        Debug.LogError($"实例化失败: {ItemToDrop_Slot._ItemData.PrefabPath}, 错误信息: {error.Message}");
    }
);
    }

    private IEnumerator ParabolaAnimation(Transform itemTransform, Vector3 startPos, Vector3 endPos, Item item)
    {
        if (itemTransform == null || item == null)
        {
            Debug.LogError("传入的参数为空！");
            yield break;
        }

        float timeElapsed = 0f;
        float distance = Vector3.Distance(startPos, endPos);
        Vector3 controlPoint = CalculateControlPoint(startPos, endPos, distance);
        float calculatedDuration = baseDropDuration + distance * distanceSensitivity;

        while (timeElapsed < calculatedDuration)
        {
            float t = timeElapsed / calculatedDuration;
            itemTransform.position = CalculateBezierPoint(t, startPos, controlPoint, endPos);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        itemTransform.position = endPos;
        item.Item_Data.CanBePickedUp = true; // 动画结束后可捡起 
    }

    private Vector3 CalculateControlPoint(Vector3 start, Vector3 end, float distance)
    {
        // 定义最大和最小高度 
        const float minHeight = 0.5f;
        const float maxHeight = 5f;

        // 根据距离线性插值高度 
        float height = Mathf.Lerp(minHeight, maxHeight, Mathf.InverseLerp(0f, 10f, distance));

        // 确保高度在合理范围内 
        height = Mathf.Clamp(height, minHeight, maxHeight);

        // 计算中间点并添加高度偏移 
        return (start + end) * 0.5f + Vector3.up * height;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 二次贝塞尔曲线公式 
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    public void Update()
    {
      /*  if (transform == null)
        {
            Debug.LogError("Transform 组件未找到！");
            enabled = false;
            return;
        }*/

        /*if (Input.GetKeyDown(KeyCode.F))
        {
            DropItem();
        }*/
    }
}