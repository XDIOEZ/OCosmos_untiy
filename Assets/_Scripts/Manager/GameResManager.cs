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
    #region 字段
    // 加载进度
    public int LoadedCount = 0; // 当前已加载的资源数量
    [Header("Prefab标签列表")]
    public List<string> ADBLabels_Prefab;
    [Header("合成配方标签列表")]
    public List<string> ADBLabels_CraftingRecipe;
    [Header("TileBase标签列表")]
    public List<string> ADBLabels_TileBase;

    // 合并后的预制体字典
    [ShowInInspector]
    public Dictionary<string, GameObject> AllPrefabs = new Dictionary<string, GameObject>(); // 只保存预制体

    [ShowInInspector]
    // 改为存储配方对象字典
    public Dictionary<string, Output_List> recipeDict = new Dictionary<string, Output_List>();
    [ShowInInspector]
    // 改为存储TileBase对象字典
    public Dictionary<string, TileBase> tileBaseDict = new Dictionary<string, TileBase>();

    
    #endregion

    #region Unity生命周期方法
    /// <summary>
    /// Unity生命周期方法，启动时调用
    /// </summary>
    private void Start()
    {
        //加载预制件
        LoadPrefabByLabels(ADBLabels_Prefab);
        //加载配方
        LoadRecipeByLabels(ADBLabels_CraftingRecipe);
        //加载TileBase
        LoadTileBaseByLabels(ADBLabels_TileBase);
    }
    #endregion



    #region 通过标签加载Prefab的方法
    /// <summary>
    /// 通过标签列表加载预制件
    /// </summary>
    public void LoadPrefabByLabels(List<string> labels)
    {
        if (labels == null || labels.Count == 0)
        {
            Debug.LogWarning("标签列表为空或未提供。");
            return;
        }

        // 使用标签列表加载资源
        Addressables.LoadAssetsAsync<GameObject>(labels, null, Addressables.MergeMode.Union).Completed += OnLoadCompleted;

    }
    /// <summary>
    /// 资源加载完成的回调
    /// </summary>
    /// <param name="handle">异步操作句柄</param>
    void OnLoadCompleted(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var prefab in handle.Result)
            {
                if (prefab == null)
                {
                    Debug.LogError("加载的预制件为空。");
                    continue;
                }

                if (AllPrefabs.ContainsKey(prefab.name))
                {
                    Debug.LogWarning($"预制件已存在: {prefab.name}");
                }
                else
                {
                    AllPrefabs[prefab.name] = prefab;
                    LoadedCount++;
                    //Debug.Log($"成功加载并添加预制件: {prefab.name}");
                }
            }
        }
        else
        {
            Debug.LogError("资源加载失败。");
        }
    }
    #endregion

    #region 通过标签加载配方的方法
        /// <summary>
        /// 通过标签列表加载配方
        /// </summary>
        public void LoadRecipeByLabels(List<string> labels)
        {
            if (labels == null || labels.Count == 0)
            {
                Debug.LogWarning("标签列表为空或未提供。");
                return;
            }

            // 使用标签列表加载资源
            Addressables.LoadAssetsAsync<CraftingRecipeSO>(labels, null, Addressables.MergeMode.Union).Completed += OnRecipeLoadCompleted;

        }
        /// <summary>
        /// 配方加载完成的回调
        /// </summary>
        /// <param name="handle">异步操作句柄</param>
        void OnRecipeLoadCompleted(AsyncOperationHandle<IList<CraftingRecipeSO>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var recipe in handle.Result)
                {
                    if (recipe == null)
                    {
                        Debug.LogError("加载的配方为空。");
                        continue;
                    }

                    if (recipeDict.ContainsKey(recipe.inputs.ToString()))
                    {
                        Debug.LogWarning($"配方已存在: {recipe.name}");
                    }
                    else
                    {
                        recipeDict[recipe.inputs.ToString()] = recipe.outputs;
                       // Debug.Log($"成功加载并添加配方: {recipe.name}");
                    }
                }
            }
            else
            {
                Debug.LogError("配方加载失败。");
            }
        }
    #endregion

    #region 通过标签加载TileBase的方法
    /// <summary>
    /// 通过标签列表加载TileBase
    /// </summary>
    public void LoadTileBaseByLabels(List<string> labels)
    {
        if (labels == null || labels.Count == 0)
        {
            Debug.LogWarning("标签列表为空或未提供。");
            return;
        }

        // 使用标签列表加载资源
        Addressables.LoadAssetsAsync<TileBase>(labels, null, Addressables.MergeMode.Union).Completed += OnTileBaseLoadCompleted;

    }
    /// <summary>
    /// TileBase加载完成的回调
    /// </summary>
    /// <param name="handle">异步操作句柄</param>
    void OnTileBaseLoadCompleted(AsyncOperationHandle<IList<TileBase>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var tileBase in handle.Result)
            {
                if (tileBase == null)
                {
                    Debug.LogError("加载的TileBase为空。");
                    continue;
                }

                if (tileBase.name == null)
                {
                    Debug.LogError("TileBase的name为空。");
                    continue;
                }

                if (tileBaseDict.ContainsKey(tileBase.name))
                {
                    Debug.LogWarning($"TileBase已存在: {tileBase.name}");
                }
                else
                {
                    tileBaseDict[tileBase.name] = tileBase;
                    LoadedCount++;
                    //Debug.Log($"成功加载并添加TileBase: {tileBase.name}");
                }
            }
        }
        else
        {
            Debug.LogError("TileBase加载失败。");
        }
    }
    #endregion




}
