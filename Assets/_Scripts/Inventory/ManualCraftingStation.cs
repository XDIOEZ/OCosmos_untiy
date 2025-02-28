using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.ResourceManagement.ResourceLocations;
using Newtonsoft.Json;
using static CraftingRecipeSO;
using Force.DeepCloner;

// 实现IInteract接口
public class ManualCraftingStation : MonoBehaviour, IInteract
{
    // 1. 输入容器
    public Inventory inputInventory;
    // 2. 输出容器
    public Inventory outputInventory;
    // 3. 交互面板 挂接的UI对象,主要用于接受玩家输入
    public GameObject interactionPanel;
    // 4. 当前已合成时间
    public float currentCraftingTime;
    // 5. dict 合成清单字典的引用
    public Dictionary<string, Output_List> recipes = new Dictionary<string, Output_List>();
    // 6. List 当前组成的合成清单
    public List<CraftingIngredient> currentRecipeList = new List<CraftingIngredient>();
    // 7. List<int> 补偿字典<位置,补偿值>
    public List<int> compensationList = new List<int>();
    // 8. bool 大于2检测标识
    public bool checkGreaterThanTwo = false;

    private void Start()
    {
        // 获取交互面板的按钮并添加事件监听
        if (interactionPanel != null)
        {
            Button craftButton = interactionPanel.GetComponentInChildren<Button>();
            if (craftButton != null)
            {
                craftButton.onClick.AddListener(OnCraftButtonClick);
            }
        }

        // 加载配方
        LoadRecipes();
    }

    // 加载配方的功能
    [Button]
    public void LoadRecipes()
    {
        // 直接从GameResManager单例类中的recipeDict获取配方信息
        recipes = GameResManager.Instance.recipeDict;
    }

    // 实现IInteract接口中的交互方法用于激活合成
    public void Interact()
    {
        // 激活合成，监听交互面板中的玩家点击按钮事件
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
        }
    }

    // 交互面板中的按钮点击事件处理
    private void OnCraftButtonClick()
    {
        Craft();
    }

    // 实际合成方法实现
    private void Craft()
    {
        currentRecipeList.Clear();
        compensationList.Clear();
        checkGreaterThanTwo = false;

        // 遍历输入容器中的全部插槽
        foreach (var item_slot in inputInventory.Data.itemSlots)
        {
            if (item_slot._ItemData == null)
            {
                currentRecipeList.Add(new CraftingIngredient("", 0));
                compensationList.Add(0);
                continue;
            }

            if (item_slot._ItemData.Stack.amount <= 1)
            {
                currentRecipeList.Add(new CraftingIngredient(item_slot._ItemData.Name, (int)item_slot._ItemData.Stack.amount));
                compensationList.Add(0);
            }
            else
            {
                checkGreaterThanTwo = true;
                currentRecipeList.Add(new CraftingIngredient(item_slot._ItemData.Name, 1));
                compensationList.Add(1);
                item_slot._ItemData.Stack.amount -= 1;
                checkGreaterThanTwo = false;
            }
        }

        // 插槽遍历完成
        if (recipes.ContainsKey(ToStringList(currentRecipeList)))
        {
            Output_List output_list = recipes[ToStringList(currentRecipeList)];
            // 根据List7将补偿值添加给对应列表位置的物体中
            for (int i = 0; i < compensationList.Count; i++)
            {
                if (compensationList[i] > 0)
                {
                    inputInventory.Data.itemSlots[i]._ItemData.Stack.amount += compensationList[i];
                }
            }
            compensationList.Clear();

            foreach (var output in output_list.results)
            {
                ItemData output_item;

                // 同步加载输出物品
                GameObject prefab = GameResManager.Instance.AllPrefabs[output.resultItem];
                if (prefab != null)
                {
                    output_item = prefab.GetComponent<Item>().DeepClone().Item_Data;
                    output_item.Stack.amount = output.resultAmount;
                    outputInventory.AddItem(output_item);


                    // ToDo.根据合成清单,删除输入插槽内的物品 你必须阅读合成清单的结构才能知道如何删除物品
                    // 根据合成清单,删除输入插槽内的物品
                    for (int i = 0; i < inputInventory.Data.itemSlots.Count; i++)
                    {
                        var itemSlot = inputInventory.Data.itemSlots[i];
                        var ingredient = currentRecipeList[i];
                        if (itemSlot._ItemData != null && itemSlot._ItemData.Name == ingredient.ItemName)
                        {
                            itemSlot._ItemData.Stack.amount -= ingredient.amount;
                            if (itemSlot._ItemData.Stack.amount <= 0)
                            {
                                inputInventory.RemoveItem(itemSlot, i);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"未能找到预制体：{output.resultItem}");
                }
            }
        }
        else
        {
            // 再次执行方法3
            Craft();
        }
    }

    private string ToStringList(List<CraftingIngredient> list)
    {
        return string.Join(",", list.Select(item => $"{{{item.ItemName},{item.amount}}}"));
    }
}

// 定义IInteract接口
public interface IInteract
{
    void Interact();
}