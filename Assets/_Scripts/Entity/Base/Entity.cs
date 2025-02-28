using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Item
{

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public  override void Use()
    {
        Debug.Log("使用物品"+gameObject.name);
    }


/*    public abstract override Item_Data GetData();


    public abstract override void SetData(Item_Data data);
*/
}