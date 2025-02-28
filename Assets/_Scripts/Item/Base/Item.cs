using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
#endif
using UnityEditor;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    /*    //TODO 创建方法 获取 继承Item类 的对象的中的ItemData属性 打印出其中的内容
        private Item_Data Item_Data;

        public  virtual Item_Data _Data
        {
            get
            {
                return Item_Data;
            }

            set
            {
                Item_Data = value;
            }
        }*/
    public abstract ItemData Item_Data { get; set; }

    public abstract void Use();

    // 添加菜单按钮以同步名称
#if UNITY_EDITOR
    [ContextMenu("同步名称,路径,Id,Guid")] // 修改为中文
    private void SyncName()
    {
        if (Item_Data != null)
        {
            Item_Data.Name = this.gameObject.name;
            Debug.Log($"游戏对象名称已同步至 {Item_Data.Name}");
        }
        else
        {
            Debug.LogWarning("物品数据为空，无法同步名称。");
        }
        //TODO 获取物体的Prefab路径 通过Addressable找到对应的资源,修改AddressableName为Prefab的名称
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.gameObject);
        if (!string.IsNullOrEmpty(prefabPath))
        {
            // 通过Addressable找到对应的资源，并修改AddressableName
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(prefabPath));
            if (entry != null)
            {
                entry.SetAddress(this.gameObject.name);
                Debug.Log($"Addressable 资源名称已修改为 {this.gameObject.name}");
            }
            else
            {
                Debug.LogError("未找到对应的 Addressable 资源。+请检查是否已添加到 Addressable 设置中。");
            }
        }
        else
        {
            Debug.LogWarning("无法获取 Prefab 路径，可能不是 Prefab 实例。");
        }
        Item_Data.PrefabPath = this.gameObject.name;
        Item_Data.ID = ++XDTool.ItemId;
        Item_Data.Guid = XDTool.NextGuid;
    }
#endif

    /*    public abstract Item_Data GetData();


        public abstract void SetData(Item_Data data);*/

    /*  

        public override Item_Data GetData()
        {
            Debug.Log($"获取{name}数据中....");
            return _data;
        }
        public override void SetData(Item_Data data)
        {
            Debug.Log($"设置{name}数据中....");
            _data = (AppleTreeData)data;
        }

    */

}
