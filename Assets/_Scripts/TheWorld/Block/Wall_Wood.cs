using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Wood : Block,IReceiveDamage
{
    public Hp BlockHp = new Hp(2);

    public Defense DefenseValue
    {
        get;
        set;
    }
    public Hp Hp
    {
        get
        {
            return BlockHp;
        }
        set
        {
        }
    }

    public void ReceiveDamage(float damage)
    {
        BlockHp.value -= damage;
        if (BlockHp.value <= 0)
        {
            DigBlock_End();
        }
    }
}
