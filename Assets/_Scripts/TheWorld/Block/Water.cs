using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Block
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.gameObject.tag != "Entity")
        {
            return;
        }
         /*Debug.Log("��ײ����" + collision.gameObject.name);*/
            var mover = collision.gameObject.GetComponentInChildren<Mover>();
            if (mover != null)
            {
                mover.AddSpeedChange(("ˮ�����ļ���", ValueChangeType.Multiply,0), 0.5f);
            }
            else
            {
                Debug.LogWarning("δ�ҵ� Mover ���������������Ӷ����У���");
            }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Entity")
        {
            return;
        }
        var parentObject = collision.gameObject.transform;
        if (parentObject != null)
        {
            var mover = parentObject.GetComponentInChildren<Mover>();

            if (mover != null)
            {
                mover.RemoveSpeedChange(("ˮ�����ļ���", ValueChangeType.Multiply, 0));
            }
            else
            {
                Debug.LogWarning("δ�ҵ� Mover ���������������Ӷ����У���");
            }
        }
        else
        {
            Debug.LogWarning("��ײ����û�и�����");
        }
    }
}

