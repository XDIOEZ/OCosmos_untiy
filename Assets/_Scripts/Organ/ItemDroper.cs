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

    [Tooltip("�����ߵ����߶�")]
    public float parabolaHeight = 2f; // ���Ա�������Ϊֻ�� 
    [Tooltip("���������߶�������ʱ��")]
    public float baseDropDuration = 0.5f;
    [Tooltip("�������жȣ����ڵ�������ʱ�������ı仯�ٶ�")]
    public float distanceSensitivity = 0.1f;

    [Button("DropItemByList")]
    public void DropItem()
    {
        Debug.Log("DropItemByList");

        if (DroperInventory == null)
        {
            Debug.LogError("DroperInventory δ���ã�");
            return;
        }

        ItemToDrop_Slot = DroperInventory.GetItemSlot(_Index);

        if (ItemToDrop_Slot == null)
        {
            Debug.Log("û����Ʒ���Զ�����");
            return;
        }

        if (string.IsNullOrEmpty(ItemToDrop_Slot._ItemData.PrefabPath))
        {
            Debug.LogError("��ƷԤ����·��Ϊ�գ�");
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
            Debug.LogError("ʵ��������ʧ�ܣ�");
            return;
        }

        Debug.Log("Instantiate new object: " + newObject.name);
        print(newObject.transform.position);
        Item newItem = newObject.GetComponent<Item>();
        if (newItem == null)
        {
            Debug.LogError("������δ�ҵ� Item �����");
            Destroy(newObject);
            return;
        }

        newItem.Item_Data =(ItemToDrop_Slot._ItemData);
        newItem.Item_Data.CanBePickedUp = false; // �����ڼ䲻�ɼ��� 

        // �������λ��
        dropPos = Camera.main.ScreenToWorldPoint(DropPos_UI.position);
        dropPos = new Vector3(dropPos.x, dropPos.y, 0);

        // ���������߶���
        StartCoroutine(ParabolaAnimation(
            newObject.transform,
            transform.position,
            dropPos,
            newItem
        ));

        // �Ƴ������е���Ʒ
        DroperInventory.RemoveItem(ItemToDrop_Slot, _Index);
    },
    (error) =>
    {
        Debug.LogError($"ʵ����ʧ��: {ItemToDrop_Slot._ItemData.PrefabPath}, ������Ϣ: {error.Message}");
    }
);
    }

    private IEnumerator ParabolaAnimation(Transform itemTransform, Vector3 startPos, Vector3 endPos, Item item)
    {
        if (itemTransform == null || item == null)
        {
            Debug.LogError("����Ĳ���Ϊ�գ�");
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
        item.Item_Data.CanBePickedUp = true; // ����������ɼ��� 
    }

    private Vector3 CalculateControlPoint(Vector3 start, Vector3 end, float distance)
    {
        // ����������С�߶� 
        const float minHeight = 0.5f;
        const float maxHeight = 5f;

        // ���ݾ������Բ�ֵ�߶� 
        float height = Mathf.Lerp(minHeight, maxHeight, Mathf.InverseLerp(0f, 10f, distance));

        // ȷ���߶��ں���Χ�� 
        height = Mathf.Clamp(height, minHeight, maxHeight);

        // �����м�㲢��Ӹ߶�ƫ�� 
        return (start + end) * 0.5f + Vector3.up * height;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // ���α��������߹�ʽ 
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    public void Update()
    {
      /*  if (transform == null)
        {
            Debug.LogError("Transform ���δ�ҵ���");
            enabled = false;
            return;
        }*/

        /*if (Input.GetKeyDown(KeyCode.F))
        {
            DropItem();
        }*/
    }
}