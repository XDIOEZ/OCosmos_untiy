using Force.DeepCloner;
using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTable : Item,IWork, IInteract
{
    // 1.inventory ��������
    public Inventory inputInventory;
    // 3.inventory �������
    public Inventory outputInventory;
    //�ϳɰ�ť
    public Button button;
    //�رհ�ť
    public Button closeButton;
    //���
    public Canvas canvas;
    // 4.workerData ��������
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
    //�л�UI��ʾ״̬
    public void SwitchUI()
    {
        canvas.enabled =!canvas.enabled;
    }
    //����Ui
    public void OpenUI()
    {
        canvas.enabled = true;
    }
    //�ر�Ui
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

    // ʵ�ʺϳɷ���ʵ��
    private void Craft()
    {
       List<CraftingIngredient> currentRecipeList = new List<CraftingIngredient>();
        for (int i = 0; i < inputInventory.Data.itemSlots.Count; i++)
        {
            var item_slot = inputInventory.Data.itemSlots[i];
       
            if (item_slot._ItemData == null)
            {
                // ������Ϊ�գ����һ���յ� CraftingIngredient ���󵽵�ǰ�ϳ��嵥
                currentRecipeList.Add(new CraftingIngredient("", 0));
                continue;
            }
            // ������Ʒ���䵱ǰ������ӵ���ǰ�ϳ��嵥
            currentRecipeList.Add(new CraftingIngredient(item_slot._ItemData.Name, 1));
        }

        // ��۱�����ɣ�����ǰ�ϳ��嵥ת��Ϊ�ַ��������ڼ���Ƿ���ƥ����䷽
        if (GameResManager.Instance.recipeDict.ContainsKey(ToStringList(currentRecipeList)))
        {
            Output_List output_list = GameResManager.Instance.recipeDict[ToStringList(currentRecipeList)];

            // ��������б��е�ÿ�������
            foreach (var output in output_list.results)
            {
                ItemData output_item;

                // ͬ�����������Ʒ
                GameObject prefab = GameResManager.Instance.AllPrefabs[output.resultItem];
                if (prefab != null)
                {
                    // ��¡Ԥ�������Ʒ����
                    output_item = prefab.GetComponent<Item>().DeepClone().Item_Data;
                    // ���������Ʒ������
                    output_item.Stack.amount = output.resultAmount;
                    // ���ϳɺ����Ʒ��ӵ��������
                    if (outputInventory.AddItem(output_item) == false)
                    {
                        return;
                    }


                    // ���ݺϳ��嵥��ɾ���������ڵ���Ʒ
                    for (int i = 0; i < inputInventory.Data.itemSlots.Count; i++)
                    {
                        var itemSlot = inputInventory.Data.itemSlots[i];
                        var ingredient = currentRecipeList[i];
                        if (itemSlot._ItemData != null && itemSlot._ItemData.Name == ingredient.ItemName)
                        {
                            // ������Ʒ����
                            itemSlot._ItemData.Stack.amount -= ingredient.amount;
                            if (itemSlot._ItemData.Stack.amount <= 0)
                            {
                                // �����Ʒ������Ϊ 0���������������Ƴ�����Ʒ
                                inputInventory.RemoveItem(itemSlot, i);
                            }
                            // ˢ�� UI ��ʾ
                            itemSlot.UI.RefreshUI();
                        }
                    }
                    return;
                }
                else
                {
                    // ���δ���ҵ�Ԥ���壬���������Ϣ
                    Debug.LogError($"δ���ҵ�Ԥ���壺{output.resultItem}");
                }
            }
        }
        // �ϳ�ʧ�ܴ���
        else
        {
            Debug.Log("�ϳ�ʧ��,����䷽û�в���");
        }
        // ���ϳ��嵥�б�ת��Ϊ�ַ�����������Ϊ�ֵ�ļ�
        string ToStringList(List<CraftingIngredient> list)
        {
            string[] ingredientStrings = new string[list.Count];
            foreach (var ingredient in list)
            {
                // ��ÿ�� CraftingIngredient ����ת��Ϊ�ַ���
                ingredientStrings[list.IndexOf(ingredient)] = ingredient.ToString();
            }
            // ���ظ�ʽ������ַ���
            return $"ԭ����: [{string.Join(",", ingredientStrings)}]";
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