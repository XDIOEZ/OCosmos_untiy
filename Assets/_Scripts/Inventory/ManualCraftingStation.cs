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

// ʵ��IInteract�ӿڣ��ýӿ����ڶ���ɽ�������Ľ�����Ϊ
public class ManualCraftingStation : MonoBehaviour, IInteract
{
    // 1. �������������ڴ�źϳ������ԭ������Ʒ
    public Inventory inputInventory;
    // 2. ������������ڴ�źϳɺ�õ�����Ʒ
    public Inventory outputInventory;
    // 3. ������壬�ҽӵ�UI������Ҫ���ڽ���������룬�������ϳɰ�ť�Ȳ���
    public GameObject interactionPanel;
    // 4. ��ǰ�Ѻϳ�ʱ�䣬�����ں���ʵ�ֺϳ�ʱ�����Ƶȹ��ܣ�Ŀǰ��δʹ��
    public float currentCraftingTime;
    // 5. �ϳ��嵥�ֵ�����ã��洢���п��õĺϳ��䷽����Ϊ�ϳ�������ϵ��ַ�����ʾ��ֵΪ����б�
    public Dictionary<string, Output_List> recipes = new Dictionary<string, Output_List>();
    // 6. ��ǰ��ɵĺϳ��嵥����¼��ǰ������������Ʒ�����Ӧ�����������ڼ���Ƿ���ƥ��ĺϳ��䷽
    public List<CraftingIngredient> currentRecipeList = new List<CraftingIngredient>();
    // 7. �����ֵ䣬�洢ÿ���������Ʒ�����Ĳ���ֵ�����ڴ���ϳ�ʧ��ʱ��Ʒ�����ĵ���
    public List<int> compensationList = new List<int>();
    // 8. ����2����ʶ�����ڱ�������������Ƿ������������2����Ʒ
    public bool checkGreaterThanTwo = false;
    // 9. �ϳɴ�������¼��ҽ��кϳɲ����Ĵ������������ƺϳɳ��Դ���
    public int craftingTimes = 0;

    // �ű�����ʱ���õķ���
    private void Start()
    {
        // ��ȡ�������İ�ť������¼�����
        if (interactionPanel != null)
        {
            // �ӽ�������в��Ұ�ť���
            Button craftButton = interactionPanel.GetComponentInChildren<Button>();
            if (craftButton != null)
            {
                // Ϊ��ť�ĵ���¼���ӻص����� OnCraftButtonClick
                craftButton.onClick.AddListener(OnCraftButtonClick);
            }
        }

        // �����䷽���� GameResManager �������л�ȡ���кϳ��䷽
        LoadRecipes();
    }

    // �����䷽�Ĺ��ܷ���
    [Button]
    public void LoadRecipes()
    {
        // ֱ�Ӵ� GameResManager �������е� recipeDict ��ȡ�䷽��Ϣ
        recipes = GameResManager.Instance.recipeDict;
    }

    // ʵ�� IInteract �ӿ��еĽ������������ڼ���ϳɲ���
    public void Interact_Start()
    {
        // ����ϳɣ�������������е���ҵ����ť�¼�
        if (interactionPanel != null)
        {
            // �������壬ʹ��ɼ��ɲ���
            interactionPanel.SetActive(true);
        }
    }
    public void Interact_Cancel()
    {
        // ����ϳɣ�������������е���ҵ����ť�¼�
        if (interactionPanel != null)
        {
            // �������壬ʹ��ɼ��ɲ���
            interactionPanel.SetActive(false);
        }
    }


    // ��������еİ�ť����¼�������
    private void OnCraftButtonClick()
    {
        // ����ҵ����������ϵĺϳɰ�ťʱ������ Craft ������ʼ�ϳɲ���
        Craft();
    }

    // ʵ�ʺϳɷ���ʵ��
    private void Craft()
    {
        // ÿ�ε��� Craft ����ʱ���ϳɴ����� 1
        craftingTimes += 1;
        // ��յ�ǰ��ɵĺϳ��嵥��Ϊ�µĺϳɳ�����׼��
        currentRecipeList.Clear();
        // ���ô���2����ʶ
        checkGreaterThanTwo = false;

        for (int i = 0; i < inputInventory.Data.itemSlots.Count; i++)
        {
            var item_slot = inputInventory.Data.itemSlots[i];
            /*
                        // ȷ�������б����㹻
                        if (compensationList.Count <= i)
                        {
                            compensationList.Add(0);
                        }



                        if (compensationList[i] == 0)
                        {
                            // �������ֵΪ 0��������ֵ��Ϊ��Ʒ������ 1��������Ʒ������Ϊ 1
                            // ��һ����Ϊ�˽���Ʒ��������Ϊ 1����������ĺϳɳ��ԣ�ͬʱ��¼��Ҫ����������
                            compensationList[i] = (int)item_slot._ItemData.Stack.amount - 1;
                            item_slot._ItemData.Stack.amount = 1;
                        }
                        else
                        {
                            // �������ֵ��Ϊ 0��������Ʒ���������ٲ���ֵ
                            item_slot._ItemData.Stack.amount += 1;
                            compensationList[i] -= 1;
                        }*/
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
        if (recipes.ContainsKey(ToStringList(currentRecipeList)))
        {
           Output_List output_list = recipes[ToStringList(currentRecipeList)];
       /*     // ���ݲ����ֵ佫����ֵ��Ӹ���Ӧ�б�λ�õ�������
            for (int i = 0; i < compensationList.Count; i++)
            {
                if (compensationList[i] > 0)
                {
                    // �������ֵ���� 0��������ֵ��ӵ���Ӧ��۵���Ʒ������
                    inputInventory.Data.itemSlots[i]._ItemData.Stack.amount += compensationList[i];
                    inputInventory.Data.itemSlots[i].UI.RefreshUI();
                }
            }*/
            Debug.Log("�ϳɳɹ�");
            // ��ȡƥ�������б�
       

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
           /* // ���ƺϳɴ��������ϳɴ����ﵽ 30 ʱ����ֹ�ϳ�
            if (craftingTimes >= 30)
            {
                // ��ղ����ֵ�
                compensationList.Clear();
                // �����ʾ��Ϣ
                Debug.Log("�ϳɴ����ﵽ 30����ֹ�ϳ�");
                // ���úϳɴ���
                craftingTimes = 0;
                return;
            }*/

        /*    // ÿ�κϳ�ʧ�ܣ����Ӳ���ֵ
            for (int i = 0; i < compensationList.Count; i++)
            {
                var item_slot = inputInventory.Data.itemSlots[i];
                if (item_slot._ItemData != null)
                {
                    // ����ֵ�� 1
                    compensationList[i]++;
                    // ��Ʒ������ 1
                    item_slot._ItemData.Stack.amount++;
                }
            }*/
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

    public void Interact_Update()
    {
        throw new System.NotImplementedException();
    }
}

// ���� IInteract �ӿڣ�����һ����������
public interface IInteract
{
    void Interact_Start();

    //���ڽ���״̬
    void Interact_Update();
    void Interact_Cancel();
}