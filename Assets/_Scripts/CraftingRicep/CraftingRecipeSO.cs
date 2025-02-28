using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "新合成配方", menuName = "合成/合成配方")]
public class CraftingRecipeSO : ScriptableObject
{
    #region Public Fields 
    [Header("配方基本信息")]
    public string recipeID = Guid.NewGuid().ToString(); // 唯一标识符 
    public string version = "1.2.0"; // 配方版本 

    [Header("输入材料")]
    public Input_List inputs = new Input_List();

    [Header("输出产物")]
    public Output_List outputs = new Output_List();
    #endregion

    #region Private Fields 
    [NonSerialized]
    private bool isValid = true; // 配方是否有效 
    [NonSerialized]
    private DateTime lastModified = DateTime.Now; // 最后修改时间 
    #endregion

    #region Public Methods 
    [Button("验证配方")]
    public void ValidateRecipe()
    {
        bool valid = true;

        // 验证输入材料 
        if (inputs.RowItems_List == null || inputs.RowItems_List.Count == 0)
        {
            Debug.LogError("配方缺少输入材料");
            valid = false;
        }
        else
        {
            foreach (CraftingIngredient ingredient in inputs.RowItems_List)
            {
                if (string.IsNullOrEmpty(ingredient.ItemName))
                {
                    Debug.LogError("输入材料缺少名称");
                    valid = false;
                }
                if (ingredient.amount <= 0)
                {
                    Debug.LogError("输入材料数量必须大于0");
                    valid = false;
                }
            }
        }

        // 验证输出产物 
        if (outputs.results == null || outputs.results.Count == 0)
        {
            Debug.LogError("配方缺少输出产物");
            valid = false;
        }
        else
        {
            foreach (Result_List result in outputs.results)
            {
                if (string.IsNullOrEmpty(result.resultItem))
                {
                    Debug.LogError("输出产物缺少物品名称");
                    valid = false;
                }
                if (result.resultAmount <= 0)
                {
                    Debug.LogError("输出产物数量必须大于0");
                    valid = false;
                }
            }
        }

        isValid = valid;
        Debug.Log($"配方验证结果: {valid}");
    }

    [Button("重置配方")]
    public void ResetRecipe()
    {
        recipeID = Guid.NewGuid().ToString();
        version = "1.2.0";
        name = "新配方";
        inputs = new Input_List();
        outputs = new Output_List();
        ValidateRecipe();
    }

    public bool IsRecipeValid()
    {
        return isValid;
    }

    public override string ToString()
    {
        return $"配方名称: {name}\n" +
               $"版本: {version}\n" +
               $"状态: {(isValid ? "有效" : "无效")}\n" +
               $"输入材料: {inputs.ToString()}\n" +
               $"输出产物: {outputs.ToString()}";
    }
    #endregion


}
#region Nested Classes 
[Serializable]
public class Input_List
{
    [Header("需要的原材料列表")]
    public List<CraftingIngredient> RowItems_List = new List<CraftingIngredient>();

    public override string ToString()
    {
        if (RowItems_List == null || RowItems_List.Count == 0)
            return "无原材料";

        List<string> ingredientStrings = new List<string>();
        foreach (var ingredient in RowItems_List)
        {
            ingredientStrings.Add(ingredient.ToString());
        }
        return $"原材料: [{string.Join(", ", ingredientStrings)}]";
    }

    public string ToString(bool Ranking)
    {
        if (RowItems_List == null || RowItems_List.Count == 0)
            return "无原材料";

        List<string> ingredientStrings = new List<string>();

        foreach (var ingredient in RowItems_List)
        {
            ingredientStrings.Add(ingredient.ToString());
        }

        // 如果Ranking为true，按字典顺序排序
        if (Ranking)
        {
            ingredientStrings.Sort();
        }

        return $"原材料: [{string.Join(", ", ingredientStrings)}]";
    }

}

[Serializable]
public class Output_List
{
    [Header("产物列表")]
    public List<Result_List> results = new List<Result_List>();

    public override string ToString()
    {
        if (results == null || results.Count == 0)
            return "无产物";

        List<string> resultStrings = new List<string>();
        foreach (var result in results)
        {
            resultStrings.Add($"{result.resultAmount}x  {result.resultItem}");
        }
        return $"产物: [{string.Join(", ", resultStrings)}]";
    }
    public string ToString(bool Ranking)
    {
        if (results == null || results.Count == 0)
            return "无产物";

        List<string> ingredientStrings = new List<string>();

        foreach (var ingredient in results)
        {
            ingredientStrings.Add(ingredient.ToString());
        }

        // 如果Ranking为true，按字典顺序排序
        if (Ranking)
        {
            ingredientStrings.Sort();
        }

        return $"产物: [{string.Join(", ", ingredientStrings)}]";
    }
}

[Serializable]
public class Result_List
{
    public string resultItem = "";
    public int resultAmount = 1;

    public override string ToString()
    {
        return $"{resultAmount}x {resultItem}";
    }
}

[Serializable]
public class CraftingIngredient
{
    public string ItemName = "";
    public int amount = 1;

    public override string ToString()
    {
        return $"{amount}x {ItemName}";
    }

    public CraftingIngredient(string inputItem, int amount)
    {
        ItemName = inputItem;
        this.amount = amount;
    }
}
#endregion