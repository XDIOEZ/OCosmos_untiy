using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 资源加载模块
/// 1.异步加载
/// 2.委托和 lambda表达式
/// 3.协程
/// 4.泛型
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    #region 资源加载方式
    //同步加载资源
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        return res;
    }
    //异步加载资源
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        //开启异步加载的协程
        MonoMgr.GetInstance().StartCoroutine(ReallyLoadAsync(name, callback));
    }
    //真正的协同程序函数  用于 开启异步加载对应的资源
    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            callback(GameObject.Instantiate(r.asset) as T);
        else
            callback(r.asset as T);
    }
    #endregion
    #region 使用addressable加载资源的函数
    /// <summary>
    /// 加载addressable资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="address"></param>
    /// <param name="callback"></param>
    public void LoadABByAddress<T>(string address, UnityAction<T> callback) where T : Object
    {
        Addressables.LoadAssetAsync<T>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                callback(handle.Result);
            }
            else
            {
                Debug.LogError($"Failed to load addressable asset: {address}");
                callback(null);
            }
        };
    }
    /// <summary>
    /// 加载addressable资源组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="label"></param>
    /// <param name="callback"></param>
    public void LoadABByLabel<T>(AssetLabelReference labelReference, UnityAction<List<T>> callback) where T : Object
    {
        Debug.Log($"Attempting to load assets with label: {labelReference}");
        Addressables.LoadAssetsAsync<T>(labelReference, null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                List<T> loadedAssets = new List<T>(handle.Result);
                callback(loadedAssets);
            }
            else
            {
                Debug.LogError($"Failed to load addressable assets group: {labelReference}");
                callback(null);
            }
        };
    }


    #endregion
}
