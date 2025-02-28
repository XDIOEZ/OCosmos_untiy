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

// ʵ��IInteract�ӿ�
public class ManualCraftingStation : MonoBehaviour, IInteract
{
    // 1. ��������
    public Inventory inputInventory;
    // 2. �������
    public Inventory outputInventory;
    // 3. ������� �ҽӵ�UI����,��Ҫ���ڽ����������
    public GameObject interactionPanel;
    // 4. ��ǰ�Ѻϳ�ʱ��
    public float currentCraftingTime;
    // 5. dict �ϳ��嵥�ֵ������
    public Dictionary<string, Output_List> recipes = new Dictionary<string, Output_List>();
    // 6. List ��ǰ��ɵĺϳ��嵥
    public List<CraftingIngredient> currentRecipeList = new List<CraftingIngredient>();
    // 7. List<int> �����ֵ�<λ��,����ֵ>
    public List<int> compensationList = new List<int>();
    // 8. bool ����2����ʶ
    public bool checkGreaterThanTwo = false;

    private void Start()
    {
        // ��ȡ�������İ�ť������¼�����
        if (interactionPanel != null)
        {
            Button craftButton = interactionPanel.GetComponentInChildren<Button>();
            if (craftButton != null)
            {
                craftButton.onClick.AddListener(OnCraftButtonClick);
            }
        }

        // �����䷽
        LoadRecipes();
    }

    // �����䷽�Ĺ���
    [Button]
    public void LoadRecipes()
    {
        // ֱ�Ӵ�GameResManager�������е�recipeDict��ȡ�䷽��Ϣ
        recipes = GameResManager.Instance.recipeDict;
    }

    // ʵ��IInteract�ӿ��еĽ����������ڼ���ϳ�
    public void Interact()
    {
        // ����ϳɣ�������������е���ҵ����ť�¼�
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
        }
    }

    // ��������еİ�ť����¼�����
    private void OnCraftButtonClick()
    {
        Craft();
    }

    // ʵ�ʺϳɷ���ʵ��
    private void Craft()
    {
        currentRecipeList.Clear();
        compensationList.Clear();
        checkGreaterThanTwo = false;

        // �������������е�ȫ�����
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

        // ��۱������
        if (recipes.ContainsKey(ToStringList(currentRecipeList)))
        {
            Output_List output_list = recipes[ToStringList(currentRecipeList)];
            // ����List7������ֵ��Ӹ���Ӧ�б�λ�õ�������
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

                // ͬ�����������Ʒ
                GameObject prefab = GameResManager.Instance.AllPrefabs[output.resultItem];
                if (prefab != null)
                {
                    output_item = prefab.GetComponent<Item>().DeepClone().Item_Data;
                    output_item.Stack.amount = output.resultAmount;
                    outputInventory.AddItem(output_item);


                    // ToDo.���ݺϳ��嵥,ɾ���������ڵ���Ʒ ������Ķ��ϳ��嵥�Ľṹ����֪�����ɾ����Ʒ
                    // ���ݺϳ��嵥,ɾ���������ڵ���Ʒ
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
                    Debug.LogError($"δ���ҵ�Ԥ���壺{output.resultItem}");
                }
            }
        }
        else
        {
            // �ٴ�ִ�з���3
            Craft();
        }
    }

    private string ToStringList(List<CraftingIngredient> list)
    {
        return string.Join(",", list.Select(item => $"{{{item.ItemName},{item.amount}}}"));
    }
}

// ����IInteract�ӿ�
public interface IInteract
{
    void Interact();
}