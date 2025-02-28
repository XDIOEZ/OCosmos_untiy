using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatEverything : Organ
{
    public Item Eat(Vector3 position)
    {
        // ��� Camera.main �Ƿ����
        if (Camera.main == null)
        {
            Debug.LogError("δ�ҵ����������");
            return null;
        }

        // �������λ��תΪ�������꣨�����Ҫ��
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

        // ��ȡ��λ����Χ������
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, 1f);

        // ���û���ҵ��ɳԵ���Ʒ������ null
        return null;
    }
}
