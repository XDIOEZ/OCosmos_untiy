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
         /*Debug.Log("碰撞对象：" + collision.gameObject.name);*/
            var mover = collision.gameObject.GetComponentInChildren<Mover>();
            if (mover != null)
            {
                mover.AddSpeedChange(("水带来的减速", ValueChangeType.Multiply,0), 0.5f);
            }
            else
            {
                Debug.LogWarning("未找到 Mover 组件（父对象或其子对象中）。");
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
                mover.RemoveSpeedChange(("水带来的减速", ValueChangeType.Multiply, 0));
            }
            else
            {
                Debug.LogWarning("未找到 Mover 组件（父对象或其子对象中）。");
            }
        }
        else
        {
            Debug.LogWarning("碰撞对象没有父对象。");
        }
    }
}

