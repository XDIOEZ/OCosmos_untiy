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

// 实现IInteract接口，该接口用于定义可交互对象的交互行为
public class ManualCraftingStation : MonoBehaviour, IInteract
{
    // 1. 输入容器，用于存放合成所需的原材料物品
    public Inventory inputInventory;
    // 2. 输出容器，用于存放合成后得到的物品
    public Inventory outputInventory;
    // 3. 交互面板，挂接的UI对象，主要用于接受玩家输入，例如点击合成按钮等操作
    public GameObject interactionPanel;
    // 4. 当前已合成时间，可用于后续实现合成时间限制等功能，目前暂未使用
    public float currentCraftingTime;
    // 5. 合成清单字典的引用，存储所有可用的合成配方，键为合成所需材料的字符串表示，值为输出列表
    public Dictionary<string, Output_List> recipes = new Dictionary<string, Output_List>();
    // 6. 当前组成的合成清单，记录当前输入容器中物品及其对应的数量，用于检查是否有匹配的合成配方
    public List<CraftingIngredient> currentRecipeList = new List<CraftingIngredient>();
    // 7. 补偿字典，存储每个插槽中物品数量的补偿值，用于处理合成失败时物品数量的调整
    public List<int> compensationList = new List<int>();
    // 8. 大于2检测标识，用于标记输入容器中是否存在数量大于2的物品
    public bool checkGreaterThanTwo = false;
    // 9. 合成次数，记录玩家进行合成操作的次数，用于限制合成尝试次数
    public int craftingTimes = 0;

    // 脚本启动时调用的方法
    private void Start()
    {
        // 获取交互面板的按钮并添加事件监听
        if (interactionPanel != null)
        {
            // 从交互面板中查找按钮组件
            Button craftButton = interactionPanel.GetComponentInChildren<Button>();
            if (craftButton != null)
            {
                // 为按钮的点击事件添加回调方法 OnCraftButtonClick
                craftButton.onClick.AddListener(OnCraftButtonClick);
            }
        }

        // 加载配方，从 GameResManager 单例类中获取所有合成配方
        LoadRecipes();
    }

    // 加载配方的功能方法
    [Button]
    public void LoadRecipes()
    {
        // 直接从 GameResManager 单例类中的 recipeDict 获取配方信息
        recipes = GameResManager.Instance.recipeDict;
    }

    // 实现 IInteract 接口中的交互方法，用于激活合成操作
    public void Interact_Start()
    {
        // 激活合成，监听交互面板中的玩家点击按钮事件
        if (interactionPanel != null)
        {
            // 激活交互面板，使其可见可操作
            interactionPanel.SetActive(true);
        }
    }
    public void Interact_Cancel()
    {
        // 激活合成，监听交互面板中的玩家点击按钮事件
        if (interactionPanel != null)
        {
            // 激活交互面板，使其可见可操作
            interactionPanel.SetActive(false);
        }
    }


    // 交互面板中的按钮点击事件处理方法
    private void OnCraftButtonClick()
    {
        // 当玩家点击交互面板上的合成按钮时，调用 Craft 方法开始合成操作
        Craft();
    }

    // 实际合成方法实现
    private void Craft()
    {
        // 每次调用 Craft 方法时，合成次数加 1
        craftingTimes += 1;
        // 清空当前组成的合成清单，为新的合成尝试做准备
        currentRecipeList.Clear();
        // 重置大于2检测标识
        checkGreaterThanTwo = false;

        for (int i = 0; i < inputInventory.Data.itemSlots.Count; i++)
        {
            var item_slot = inputInventory.Data.itemSlots[i];
            /*
                        // 确保补偿列表长度足够
                        if (compensationList.Count <= i)
                        {
                            compensationList.Add(0);
                        }



                        if (compensationList[i] == 0)
                        {
                            // 如果补偿值为 0，将补偿值设为物品数量减 1，并将物品数量设为 1
                            // 这一步是为了将物品数量调整为 1，方便后续的合成尝试，同时记录需要补偿的数量
                            compensationList[i] = (int)item_slot._ItemData.Stack.amount - 1;
                            item_slot._ItemData.Stack.amount = 1;
                        }
                        else
                        {
                            // 如果补偿值不为 0，增加物品数量并减少补偿值
                            item_slot._ItemData.Stack.amount += 1;
                            compensationList[i] -= 1;
                        }*/
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
        if (recipes.ContainsKey(ToStringList(currentRecipeList)))
        {
           Output_List output_list = recipes[ToStringList(currentRecipeList)];
       /*     // 根据补偿字典将补偿值添加给对应列表位置的物体中
            for (int i = 0; i < compensationList.Count; i++)
            {
                if (compensationList[i] > 0)
                {
                    // 如果补偿值大于 0，将补偿值添加到对应插槽的物品数量上
                    inputInventory.Data.itemSlots[i]._ItemData.Stack.amount += compensationList[i];
                    inputInventory.Data.itemSlots[i].UI.RefreshUI();
                }
            }*/
            Debug.Log("合成成功");
            // 获取匹配的输出列表
       

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
           /* // 限制合成次数，当合成次数达到 30 时，禁止合成
            if (craftingTimes >= 30)
            {
                // 清空补偿字典
                compensationList.Clear();
                // 输出提示信息
                Debug.Log("合成次数达到 30，禁止合成");
                // 重置合成次数
                craftingTimes = 0;
                return;
            }*/

        /*    // 每次合成失败，增加补偿值
            for (int i = 0; i < compensationList.Count; i++)
            {
                var item_slot = inputInventory.Data.itemSlots[i];
                if (item_slot._ItemData != null)
                {
                    // 补偿值加 1
                    compensationList[i]++;
                    // 物品数量加 1
                    item_slot._ItemData.Stack.amount++;
                }
            }*/
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

    public void Interact_Update()
    {
        throw new System.NotImplementedException();
    }
}

// 定义 IInteract 接口，包含一个交互方法
public interface IInteract
{
    void Interact_Start();

    //处于交互状态
    void Interact_Update();
    void Interact_Cancel();
}