using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "�ºϳ��䷽", menuName = "�ϳ�/�ϳ��䷽")]
public class CraftingRecipeSO : ScriptableObject
{
    #region Public Fields 
    [Header("�䷽������Ϣ")]
    public string recipeID = Guid.NewGuid().ToString(); // Ψһ��ʶ�� 
    public string version = "1.2.0"; // �䷽�汾 

    [Header("�������")]
    public Input_List inputs = new Input_List();

    [Header("�������")]
    public Output_List outputs = new Output_List();
    #endregion

    #region Private Fields 
    [NonSerialized]
    private bool isValid = true; // �䷽�Ƿ���Ч 
    [NonSerialized]
    private DateTime lastModified = DateTime.Now; // ����޸�ʱ�� 
    #endregion

    #region Public Methods 
    [Button("��֤�䷽")]
    public void ValidateRecipe()
    {
        bool valid = true;

        // ��֤������� 
        if (inputs.RowItems_List == null || inputs.RowItems_List.Count == 0)
        {
            Debug.LogError("�䷽ȱ���������");
            valid = false;
        }
        else
        {
            foreach (CraftingIngredient ingredient in inputs.RowItems_List)
            {
                if (string.IsNullOrEmpty(ingredient.ItemName))
                {
                    Debug.LogError("�������ȱ������");
                    valid = false;
                }
                if (ingredient.amount <= 0)
                {
                    Debug.LogError("������������������0");
                    valid = false;
                }
            }
        }

        // ��֤������� 
        if (outputs.results == null || outputs.results.Count == 0)
        {
            Debug.LogError("�䷽ȱ���������");
            valid = false;
        }
        else
        {
            foreach (Result_List result in outputs.results)
            {
                if (string.IsNullOrEmpty(result.resultItem))
                {
                    Debug.LogError("�������ȱ����Ʒ����");
                    valid = false;
                }
                if (result.resultAmount <= 0)
                {
                    Debug.LogError("������������������0");
                    valid = false;
                }
            }
        }

        isValid = valid;
        Debug.Log($"�䷽��֤���: {valid}");
    }

    [Button("�����䷽")]
    public void ResetRecipe()
    {
        recipeID = Guid.NewGuid().ToString();
        version = "1.2.0";
        name = "���䷽";
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
        return $"�䷽����: {name}\n" +
               $"�汾: {version}\n" +
               $"״̬: {(isValid ? "��Ч" : "��Ч")}\n" +
               $"�������: {inputs.ToString()}\n" +
               $"�������: {outputs.ToString()}";
    }
    #endregion


}
#region Nested Classes 
[Serializable]
public class Input_List
{
    [Header("��Ҫ��ԭ�����б�")]
    public List<CraftingIngredient> RowItems_List = new List<CraftingIngredient>();

    public override string ToString()
    {
        if (RowItems_List == null || RowItems_List.Count == 0)
            return "��ԭ����";

        List<string> ingredientStrings = new List<string>();
        foreach (var ingredient in RowItems_List)
        {
            ingredientStrings.Add(ingredient.ToString());
        }
        return $"ԭ����: [{string.Join(", ", ingredientStrings)}]";
    }

    public string ToString(bool Ranking)
    {
        if (RowItems_List == null || RowItems_List.Count == 0)
            return "��ԭ����";

        List<string> ingredientStrings = new List<string>();

        foreach (var ingredient in RowItems_List)
        {
            ingredientStrings.Add(ingredient.ToString());
        }

        // ���RankingΪtrue�����ֵ�˳������
        if (Ranking)
        {
            ingredientStrings.Sort();
        }

        return $"ԭ����: [{string.Join(", ", ingredientStrings)}]";
    }

}

[Serializable]
public class Output_List
{
    [Header("�����б�")]
    public List<Result_List> results = new List<Result_List>();

    public override string ToString()
    {
        if (results == null || results.Count == 0)
            return "�޲���";

        List<string> resultStrings = new List<string>();
        foreach (var result in results)
        {
            resultStrings.Add($"{result.resultAmount}x  {result.resultItem}");
        }
        return $"����: [{string.Join(", ", resultStrings)}]";
    }
    public string ToString(bool Ranking)
    {
        if (results == null || results.Count == 0)
            return "�޲���";

        List<string> ingredientStrings = new List<string>();

        foreach (var ingredient in results)
        {
            ingredientStrings.Add(ingredient.ToString());
        }

        // ���RankingΪtrue�����ֵ�˳������
        if (Ranking)
        {
            ingredientStrings.Sort();
        }

        return $"����: [{string.Join(", ", ingredientStrings)}]";
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