using Force.DeepCloner;
using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTable : Item,IWork, IInteract
{
    // 1.inventory 输入容器
    public Inventory inputInventory;
    // 3.inventory 输出容器
    public Inventory outputInventory;
    //合成按钮
    public Button button;
    //关闭按钮
    public Button closeButton;
    //面板
    public Canvas canvas;
    // 4.workerData 工作数据
    public WorkerData workerData;

    public override ItemData Item_Data
    {
        get
        {
            return workerData;
        }
        set
        {
            workerData = (WorkerData)value;
        }
    }

    void Start()
    {
        button.onClick.AddListener(Craft);
        closeButton.onClick.AddListener(Interact_Start);
    }
    //切换UI显示状态
    public void SwitchUI()
    {
        canvas.enabled =!canvas.enabled;
    }
    //开启Ui
    public void OpenUI()
    {
        canvas.enabled = true;
    }
    //关闭Ui
    public void CloseUI()
    {
        canvas.enabled = false;
    }
    public void Work_Start()
    {
        Craft();
    }

    public void Interact_Start()
    {
        SwitchUI();
    }


    public override void Use()
    {
        Interact_Start();
    }

    // 实际合成方法实现
    private void Craft()
    {
       List<CraftingIngredient> currentRecipeList = new List<CraftingIngredient>();
        for (int i = 0; i < inputInventory.Data.itemSlots.Count; i++)
        {
            var item_slot = inputInventory.Data.itemSlots[i];
       
            if (item_slot._ItemData == null)
            {
                // 如果插槽为空，添加一个空的 CraftingIngredient 对象到当前合成清单
                currentRecipeList.Add(new CraftingIngredient("", 0));
                continue;
            }
            // 将该物品及其当前数量添加到当前合成清单
            currentRecipeList.Add(new CraftingIngredient(item_slot._ItemData.Name, 1));
        }

        // 插槽遍历完成，将当前合成清单转换为字符串，用于检查是否有匹配的配方
        if (GameResManager.Instance.recipeDict.ContainsKey(ToStringList(currentRecipeList)))
        {
            Output_List output_list = GameResManager.Instance.recipeDict[ToStringList(currentRecipeList)];

            // 遍历输出列表中的每个输出项
            foreach (var output in output_list.results)
            {
                ItemData output_item;

                // 同步加载输出物品
                GameObject prefab = GameResManager.Instance.AllPrefabs[output.resultItem];
                if (prefab != null)
                {
                    // 克隆预制体的物品数据
                    output_item = prefab.GetComponent<Item>().DeepClone().Item_Data;
                    // 设置输出物品的数量
                    output_item.Stack.amount = output.resultAmount;
                    // 将合成后的物品添加到输出容器
                    if (outputInventory.AddItem(output_item) == false)
                    {
                        return;
                    }


                    // 根据合成清单，删除输入插槽内的物品
                    for (int i = 0; i < inputInventory.Data.itemSlots.Count; i++)
                    {
                        var itemSlot = inputInventory.Data.itemSlots[i];
                        var ingredient = currentRecipeList[i];
                        if (itemSlot._ItemData != null && itemSlot._ItemData.Name == ingredient.ItemName)
                        {
                            // 减少物品数量
                            itemSlot._ItemData.Stack.amount -= ingredient.amount;
                            if (itemSlot._ItemData.Stack.amount <= 0)
                            {
                                // 如果物品数量减为 0，从输入容器中移除该物品
                                inputInventory.RemoveItem(itemSlot, i);
                            }
                            // 刷新 UI 显示
                            itemSlot.UI.RefreshUI();
                        }
                    }
                    return;
                }
                else
                {
                    // 如果未能找到预制体，输出错误信息
                    Debug.LogError($"未能找到预制体：{output.resultItem}");
                }
            }
        }
        // 合成失败处理
        else
        {
            Debug.Log("合成失败,这个配方没有产物");
        }
        // 将合成清单列表转换为字符串，用于作为字典的键
        string ToStringList(List<CraftingIngredient> list)
        {
            string[] ingredientStrings = new string[list.Count];
            foreach (var ingredient in list)
            {
                // 将每个 CraftingIngredient 对象转换为字符串
                ingredientStrings[list.IndexOf(ingredient)] = ingredient.ToString();
            }
            // 返回格式化后的字符串
            return $"原材料: [{string.Join(",", ingredientStrings)}]";
        }
    }

    public void Interact_Cancel()
    {
        CloseUI();
    }

    public void Interact_Update()
    {
        throw new System.NotImplementedException();
    }

    public void Work_Update()
    {
        throw new System.NotImplementedException();
    }

    public void Work_Stop()
    {
        throw new System.NotImplementedException();
    }
}


public interface IWork
{
    public void Work_Start();

    public void Work_Update();

    public void Work_Stop();
}