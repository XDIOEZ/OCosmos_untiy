using Sirenix.OdinInspector;
using System.Collections.Generic;
#if UNITY_EDITOR

using UnityEditor.AddressableAssets.Settings;

#endif

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public class GameResManager : SingletonAutoMono<GameResManager>
{
    #region �ֶ�
    // ���ؽ���
    public int LoadedCount = 0; // ��ǰ�Ѽ��ص���Դ����
    [Header("Prefab��ǩ�б�")]
    public List<string> ADBLabels_Prefab;
    [Header("�ϳ��䷽��ǩ�б�")]
    public List<string> ADBLabels_CraftingRecipe;
    [Header("TileBase��ǩ�б�")]
    public List<string> ADBLabels_TileBase;

    // �ϲ����Ԥ�����ֵ�
    [ShowInInspector]
    public Dictionary<string, GameObject> AllPrefabs = new Dictionary<string, GameObject>(); // ֻ����Ԥ����

    [ShowInInspector]
    // ��Ϊ�洢�䷽�����ֵ�
    public Dictionary<string, Output_List> recipeDict = new Dictionary<string, Output_List>();
    [ShowInInspector]
    // ��Ϊ�洢TileBase�����ֵ�
    public Dictionary<string, TileBase> tileBaseDict = new Dictionary<string, TileBase>();

    
    #endregion

    #region Unity�������ڷ���
    /// <summary>
    /// Unity�������ڷ���������ʱ����
    /// </summary>
    private void Start()
    {
        //����Ԥ�Ƽ�
        LoadPrefabByLabels(ADBLabels_Prefab);
        //�����䷽
        LoadRecipeByLabels(ADBLabels_CraftingRecipe);
        //����TileBase
        LoadTileBaseByLabels(ADBLabels_TileBase);
    }
    #endregion



    #region ͨ����ǩ����Prefab�ķ���
    /// <summary>
    /// ͨ����ǩ�б����Ԥ�Ƽ�
    /// </summary>
    public void LoadPrefabByLabels(List<string> labels)
    {
        if (labels == null || labels.Count == 0)
        {
            Debug.LogWarning("��ǩ�б�Ϊ�ջ�δ�ṩ��");
            return;
        }

        // ʹ�ñ�ǩ�б������Դ
        Addressables.LoadAssetsAsync<GameObject>(labels, null, Addressables.MergeMode.Union).Completed += OnLoadCompleted;

    }
    /// <summary>
    /// ��Դ������ɵĻص�
    /// </summary>
    /// <param name="handle">�첽�������</param>
    void OnLoadCompleted(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var prefab in handle.Result)
            {
                if (prefab == null)
                {
                    Debug.LogError("���ص�Ԥ�Ƽ�Ϊ�ա�");
                    continue;
                }

                if (AllPrefabs.ContainsKey(prefab.name))
                {
                    Debug.LogWarning($"Ԥ�Ƽ��Ѵ���: {prefab.name}");
                }
                else
                {
                    AllPrefabs[prefab.name] = prefab;
                    LoadedCount++;
                    //Debug.Log($"�ɹ����ز����Ԥ�Ƽ�: {prefab.name}");
                }
            }
        }
        else
        {
            Debug.LogError("��Դ����ʧ�ܡ�");
        }
    }
    #endregion

    #region ͨ����ǩ�����䷽�ķ���
        /// <summary>
        /// ͨ����ǩ�б�����䷽
        /// </summary>
        public void LoadRecipeByLabels(List<string> labels)
        {
            if (labels == null || labels.Count == 0)
            {
                Debug.LogWarning("��ǩ�б�Ϊ�ջ�δ�ṩ��");
                return;
            }

            // ʹ�ñ�ǩ�б������Դ
            Addressables.LoadAssetsAsync<CraftingRecipeSO>(labels, null, Addressables.MergeMode.Union).Completed += OnRecipeLoadCompleted;

        }
        /// <summary>
        /// �䷽������ɵĻص�
        /// </summary>
        /// <param name="handle">�첽�������</param>
        void OnRecipeLoadCompleted(AsyncOperationHandle<IList<CraftingRecipeSO>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var recipe in handle.Result)
                {
                    if (recipe == null)
                    {
                        Debug.LogError("���ص��䷽Ϊ�ա�");
                        continue;
                    }

                    if (recipeDict.ContainsKey(recipe.inputs.ToString()))
                    {
                        Debug.LogWarning($"�䷽�Ѵ���: {recipe.name}");
                    }
                    else
                    {
                        recipeDict[recipe.name] = recipe.outputs;
                       // Debug.Log($"�ɹ����ز�����䷽: {recipe.name}");
                    }
                }
            }
            else
            {
                Debug.LogError("�䷽����ʧ�ܡ�");
            }
        }
    #endregion

    #region ͨ����ǩ����TileBase�ķ���
    /// <summary>
    /// ͨ����ǩ�б����TileBase
    /// </summary>
    public void LoadTileBaseByLabels(List<string> labels)
    {
        if (labels == null || labels.Count == 0)
        {
            Debug.LogWarning("��ǩ�б�Ϊ�ջ�δ�ṩ��");
            return;
        }

        // ʹ�ñ�ǩ�б������Դ
        Addressables.LoadAssetsAsync<TileBase>(labels, null, Addressables.MergeMode.Union).Completed += OnTileBaseLoadCompleted;

    }
    /// <summary>
    /// TileBase������ɵĻص�
    /// </summary>
    /// <param name="handle">�첽�������</param>
    void OnTileBaseLoadCompleted(AsyncOperationHandle<IList<TileBase>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var tileBase in handle.Result)
            {
                if (tileBase == null)
                {
                    Debug.LogError("���ص�TileBaseΪ�ա�");
                    continue;
                }

                if (tileBase.name == null)
                {
                    Debug.LogError("TileBase��nameΪ�ա�");
                    continue;
                }

                if (tileBaseDict.ContainsKey(tileBase.name))
                {
                    Debug.LogWarning($"TileBase�Ѵ���: {tileBase.name}");
                }
                else
                {
                    tileBaseDict[tileBase.name] = tileBase;
                    LoadedCount++;
                    //Debug.Log($"�ɹ����ز����TileBase: {tileBase.name}");
                }
            }
        }
        else
        {
            Debug.LogError("TileBase����ʧ�ܡ�");
        }
    }
    #endregion




}
