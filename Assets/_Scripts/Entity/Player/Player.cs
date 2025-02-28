using MemoryPack;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Entity, INutrient,IReceiveDamage
{
    public int PlayerDeadAmount = 0;
    public PlayerData Data;
    public override ItemData Item_Data
    {
        get
        {
            return Data;
        }

        set
        {
            Data = value as PlayerData;
        }
    }
    public Nutrient Entity_Nutrient { get => Data._nutrientData; set => Data._nutrientData = value; }
    public Defense DefenseValue
    {
        get
        {
            return Data._entityData.defense;
        }

        set
        {
            Data._entityData.defense = value;
        }
    }
    public Hp Hp
    {
        get
        {
            return Data._entityData.hp;
        }

        set
        {
            Data._entityData.hp = value;
        }
    }
    public override void Die()
    {
        throw new NotImplementedException();
    }

    public override void Use()
    {
        throw new NotImplementedException();
    }

    public void Eat(INutrient Be_Eat)
    {
        Entity_Nutrient += Be_Eat.Entity_Nutrient;
    }

    public void BeEat(INutrient Eat_Er)
    {
        Eat_Er.Entity_Nutrient += Entity_Nutrient;
        Debug.Log("吃了" + Eat_Er.Entity_Nutrient + "份" + Eat_Er.GetType().Name);
    }
    
    public string GetPlayerSpInfo()
    {
        return "死亡次数："+ PlayerDeadAmount;
    }

    [Button("保存死亡次数")]
    public void SaveSpInfo()
    {
        Data.ItemSpecialData += GetPlayerSpInfo();
    }

    [Button("读取死亡次数")]
    public void ReadSpInfo()
    {
        int startIndex = Data.ItemSpecialData.IndexOf("死亡次数：");
        //print(startIndex);
        if (startIndex != -1)
        {
            // 计算正确的起始位置
            startIndex += "死亡次数：".Length;

            // 提取死亡次数的字符串
            string GetInfo = Data.ItemSpecialData[startIndex..];

            // 安全解析整数
            if (int.TryParse(GetInfo, out int deadAmount))
            {
                PlayerDeadAmount = deadAmount;  // 写入死亡次数
               // print(startIndex);
            }
            else
            {
              //  print(startIndex);
            }
        }
        else
        {
          //  print(startIndex);
        }
    }

    public void ReceiveDamage(float damage)
    {
        throw new NotImplementedException();
    }
}

public interface IReceiveDamage
{
    Defense DefenseValue { get; set; }
     Hp   Hp { get; set; }
    void ReceiveDamage(float damage);
}
[System.Serializable]
[MemoryPackable]
public partial class Hp
{
    public float value;

    [MemoryPackConstructor]
    public Hp(float value)
    {
        this.value = value;
    }
}

[System.Serializable,MemoryPackable]
public partial class PlayerData : ItemData,IEntity_Data,INutrient_Data
{
    [ShowInInspector]
    public EntityData _entityData { get;set; }
    [ShowInInspector]
    public Nutrient _nutrientData { get; set; }
}

public interface INutrient
{
    [ShowInInspector]
    Nutrient Entity_Nutrient { get; set; }

    void BeEat(INutrient Eat_Er);

    void Eat(INutrient Be_Eat);
}
