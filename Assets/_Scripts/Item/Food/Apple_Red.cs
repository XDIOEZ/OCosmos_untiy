using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple_Red : Item,INutrient
{
    public Apple_Red_Data _data;
    public override ItemData Item_Data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = (Apple_Red_Data)value;
        }
    }
    public Nutrient Entity_Nutrient { get => _data._nutrientData; set => _data._nutrientData = value; }

    public void BeEat(INutrient Eat_er)
    {
        Eat_er.Entity_Nutrient += Entity_Nutrient;
    }

    public void Eat(INutrient EatItem)
    {
        EatItem.BeEat(this);

        Debug.Log($"Player Eat{EatItem}");
    }

    public override void Use()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
[MemoryPackable]
[System.Serializable]
public partial class Apple_Red_Data : ItemData,INutrient_Data
{
    [ShowInInspector]
    public Nutrient _nutrientData { get; set; }
}
[MemoryPackable]
[System.Serializable]
public partial class Nutrient
{
    public float food_Energy;
    public float Food_Energy { get => food_Energy; set => food_Energy = value; }
    //重写+=运算符，实现Nutrient的累加
    public static Nutrient operator +(Nutrient a, Nutrient b)
    {
        Nutrient result = new Nutrient();
        result.Food_Energy = a.Food_Energy + b.Food_Energy;
        return result;
    }
}
