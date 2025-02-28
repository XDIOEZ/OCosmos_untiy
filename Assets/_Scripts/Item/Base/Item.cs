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
    /*    //TODO �������� ��ȡ �̳�Item�� �Ķ�����е�ItemData���� ��ӡ�����е�����
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

    // ��Ӳ˵���ť��ͬ������
#if UNITY_EDITOR
    [ContextMenu("ͬ������,·��,Id,Guid")] // �޸�Ϊ����
    private void SyncName()
    {
        if (Item_Data != null)
        {
            Item_Data.Name = this.gameObject.name;
            Debug.Log($"��Ϸ����������ͬ���� {Item_Data.Name}");
        }
        else
        {
            Debug.LogWarning("��Ʒ����Ϊ�գ��޷�ͬ�����ơ�");
        }
        //TODO ��ȡ�����Prefab·�� ͨ��Addressable�ҵ���Ӧ����Դ,�޸�AddressableNameΪPrefab������
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.gameObject);
        if (!string.IsNullOrEmpty(prefabPath))
        {
            // ͨ��Addressable�ҵ���Ӧ����Դ�����޸�AddressableName
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(prefabPath));
            if (entry != null)
            {
                entry.SetAddress(this.gameObject.name);
                Debug.Log($"Addressable ��Դ�������޸�Ϊ {this.gameObject.name}");
            }
            else
            {
                Debug.LogError("δ�ҵ���Ӧ�� Addressable ��Դ��+�����Ƿ�����ӵ� Addressable �����С�");
            }
        }
        else
        {
            Debug.LogWarning("�޷���ȡ Prefab ·�������ܲ��� Prefab ʵ����");
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
            Debug.Log($"��ȡ{name}������....");
            return _data;
        }
        public override void SetData(Item_Data data)
        {
            Debug.Log($"����{name}������....");
            _data = (AppleTreeData)data;
        }

    */

}
