using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFood : MonoBehaviour
{
    public HungryValueChange hungryValueChange;

    public void Start()
    {
        hungryValueChange = GetComponent<HungryValueChange>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // 在这里处理触发器碰撞的逻辑
        //如果碰撞对象上挂接了CanBeEat组件，则执行吃食物逻辑
        CanBeEat canBeEat = collision.GetComponent<CanBeEat>();
        if (collision.GetComponent<CanBeEat>())
        {
            // 吃食物逻辑
            hungryValueChange.CurrentHungryValue += canBeEat.AddEnergyValue;
            Destroy(collision.gameObject);
        }
       
    }


}
