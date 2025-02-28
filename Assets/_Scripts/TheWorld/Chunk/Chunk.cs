using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

// Chunk �����ڹ����������ز����������������Ʒ��ʵ����������� 
public class Chunk : MonoBehaviour
{
    #region �ֶ� (�����Ӷ���������)
    // ����ģʽ���� 
    public bool isDebug = true;
    // ÿ֡ʵ�������� 
    public int blocksPerFrame;
    // ʵ�������ʱ�� 
    public float instantiateDelay = 0.05f;
    // ����ߴ����� 
    public Vector2Int chunkSize = new Vector2Int(25, 25);
    // ������������ 
    public ChunkData chunkData;
    // ����ʵ������ 
    public List<GameObject> Blocks_GameObject = new List<GameObject>();
    // ��Ʒʵ������ 
    public List<Item> Items_GameObject = new List<Item>();
    // ���鸸�ڵ� 
    public Transform BlockParent;
    // ��Ʒ���ڵ� 
    public Transform ItemParent;
    //�����ֵ�
    [ShowInInspector]
    public Dictionary<Vector3, BlockData> blockDict = new Dictionary<Vector3, BlockData>();
    #endregion 

    #region Unity �������ڷ��� 
    // �� Chunk �������ʱ���ã����ڱ�����Ʒ���ݵ��������� 
    private void OnDisable()
    {
        SaveChunkItemToData();
    }
    #endregion

    #region �������� 

    //�������������з����λ��
    public void UpdateBlockPosition(Vector3 position)
    {
        
    }
    /// <summary> 
    /// ���������е���Ʒ���ݵ��������� 
    /// </summary> 
    public void SaveChunkItemToData()
    {
        // ���� Items_GameObject �����е�Ϊ�յ������Ӧ�������� chunkData.itemDatas  ��Ӧ����ͬ������ɾ�� 
        Debug.Log("������Ʒ����");
        for (int i = Items_GameObject.Count - 1; i >= 0; i--)
        {
            if (Items_GameObject[i] == null)
            {
                Items_GameObject.RemoveAt(i);
                chunkData.itemDatas[i] = null;
                chunkData.itemDatas.RemoveAt(i);
            }
        }

        // ���� Items_GameObject ���䱣�浽 chunkData.itemDatas  
        foreach (Item item in Items_GameObject)
        {
            SaveItemTochunkData(item, chunkData);
        }
    }
    /// <summary> 
    /// ����Ʒ�б��������Ʒ���������丸���� 
    /// </summary> 
    /// <param name="item_add">Ҫ��ӵ���Ʒ</param> 
    public void AddItem(Item item_add)
    {
        Items_GameObject.Add(item_add);
        item_add.transform.SetParent(ItemParent);
    }

    /// <summary> 
    /// ��ʼ�� Chunk������λ�ò���������ʵ����Э�̣�������Ʒ 
    /// </summary> 
    /// <param name="chunkData">��������</param> 
    public void Initialize(ChunkData chunkData)
    {
        this.chunkData = chunkData;

        // ���� Chunk ��λ��Ϊ chunkData �� ChunkSize.x �� ChunkSize.y ֵ 
        transform.localPosition = new Vector3(chunkData.chunkPosition.x, chunkData.chunkPosition.y, transform.position.z);  // ʹ�� Z ��ĵ�ǰֵ 

        // ����Э��ʵ�������� 
        StartCoroutine(InstantiateBlocksCoroutine());
        LoadChunks_Item(chunkData);
    }

    /// <summary> 
    /// ����Ʒ���浽 Chunk ����Ʒ�б��У��������丸���� 
    /// </summary> 
    /// <param name="item">Ҫ�������Ʒ</param> 
    public void SaveItemTOChunk(Item item)
    {
        Items_GameObject.Add(item);
        item.transform.SetParent(ItemParent);
    }

    /// <summary> 
    /// ����Ʒ���ݱ��浽�������ݵ���Ʒ�����б��� 
    /// </summary> 
    /// <param name="itemData">Ҫ�������Ʒ����</param> 
    public void SaveItemDataTOChunkData(ItemData itemData)
    {
        chunkData.itemDatas.Add(itemData);
    }
    #endregion

    #region ˽�з��� 
    /// <summary> 
    /// Э��ʵ�������飬����ÿ֡ʵ�����ķ������� 
    /// </summary> 
    /// <returns></returns> 
    private IEnumerator InstantiateBlocksCoroutine()
    {
        if (isDebug) Debug.Log($"��ʼʵ�������飬���� {chunkData.blockDatas.Count}  ��������Ҫ����");

        // ƫ����������ȷ�����������Ķ��� 
        float offsetX = (chunkSize.x % 2 == 0) ? chunkSize.x * 0.5f - 0.5f : chunkSize.x * 0.5f;
        float offsetY = (chunkSize.y % 2 == 0) ? chunkSize.y * 0.5f - 0.5f : chunkSize.y * 0.5f;

        int blocksInstantiatedThisFrame = 0; // ����׷�ٵ�ǰ֡ʵ�����ķ������� 

        for (int i = 0; i < chunkData.blockDatas.Count; i++)
        {
            BlockData block = chunkData.blockDatas[i];

            blockDict[block.position] = chunkData.blockDatas[i];

            // ʹ�� chunkData �� ChunkSize.x �� ChunkSize.y �����㷽���λ�ã������ƫ���� 
            if (block.name == "air"||block.name == "")
            {
                chunkData.blockDatas.RemoveAt(i);
                continue;
            }
            Vector3 blockPosition;

            #region λ�ô���

            if (block.position.x == 0 && block.position.y == 0)
            {
                blockPosition = new Vector3(
               0 + (i % chunkSize.x) * 1 - offsetX,
               0 + (i / chunkSize.x) * 1 - offsetY,
               10
              );
                block.position.Set((int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z);
            }
            else
            {
                blockPosition = block.position;
            }
            #endregion


            string prefabName = block.name;

            if (isDebug)
            {
                Debug.Log($"���ڴ���� {i + 1} �����飬����: {prefabName}, λ��: {blockPosition}");
            }

            //ʵ��������
            if (GameResManager.Instance.AllPrefabs[prefabName] is GameObject blockPrefab)
            {
                // ʵ�������� 
                GameObject blockObject = Instantiate(blockPrefab);

                blockObject.transform.parent = BlockParent; // ����������Ϊ Chunk ���Ӷ��� 

                blockObject.transform.position = blockPosition;

                Blocks_GameObject.Add(blockObject);

                if (isDebug) Debug.Log($"���� {prefabName} ʵ�����ɹ���λ��: {blockPosition}");
            }
            else
            {
                if (isDebug) Debug.LogWarning($"δ����Դ���ҵ�����Ԥ����: {prefabName}");
            }

            blocksInstantiatedThisFrame++;

            // ÿ֡ʵ���� blocksPerFrame ������ 
            if (blocksInstantiatedThisFrame >= blocksPerFrame)
            {
                blocksInstantiatedThisFrame = 0; // ���ü����� 
                yield return null; // ��ͣЭ�̣����� Unity �������Ȩ���ȴ���һ֡ 
            }
        }

        if (isDebug) Debug.Log($"����ʵ������ɣ��ܹ�ʵ������ {Blocks_GameObject.Count} �����顣");
    }

    /// <summary> 
    /// ���������е���Ʒ 
    /// </summary> 
    /// <param name="chunkData_">��������</param> 
    private void LoadChunks_Item(ChunkData chunkData_)
    {
        // ��� chunkData_.itemDatas �Ƿ�Ϊ�� 
        if (chunkData_.itemDatas == null)
        {
            Debug.LogWarning("chunkData_.itemDatas Ϊ��");
            chunkData_.itemDatas = new List<ItemData>();
            return;
        }

        // ���� chunkData_.itemDatas 
        foreach (ItemData itemData in chunkData_.itemDatas)
        {
          //  Debug.Log(itemData.ToString());
            XDTool.InstantiateAddressableAsync(
                itemData.PrefabPath,
                itemData.Position,
                itemData.Rotation,
                (item_GameObject) =>
                {
                    item_GameObject.transform.SetParent(ItemParent);
                    item_GameObject.GetComponent<Item>().Item_Data = itemData;  
                    Items_GameObject.Add(item_GameObject.GetComponent<Item>());
                    item_GameObject.name = itemData.Name;
                    item_GameObject.transform.localScale = itemData.Scale;
                    item_GameObject.SetActive(true);
                },
                (error) =>
                {
                    Debug.LogError($"ʵ����ʧ��: {itemData.PrefabPath}, ������Ϣ: {error.Message}");
                }
            );
        }
    }



    /// <summary> 
    /// ����Ʒ���浽���������� 
    /// </summary> 
    /// <param name="item">Ҫ�������Ʒ</param> 
    /// <param name="chunkdata_">��������</param> 
    private void SaveItemTochunkData(Item item, ChunkData chunkdata_)
    {
        try
        {
            if (item == null)
            {
                return;
            }

            Quaternion validatedRotation = item.transform.rotation;
            // ������Ʒ���� 
            Vector3 validatedScale = item.transform.localScale;
            // ������Ʒ���꣨��������ϵת���� 
            Vector3 validatedPosition = new Vector3(
                Mathf.Round(item.transform.position.x * 100) / 100f,
                Mathf.Round(item.transform.position.y * 100) / 100f,
                Mathf.Round(item.transform.position.z * 100) / 100f
            );
            item.Item_Data.SetTransformValue(validatedPosition, validatedRotation, validatedScale);

            // ��ֹ�ظ���ӵĶ�����֤ 
            if (!chunkdata_.itemDatas.Any(x => x.Guid == item.Item_Data.Guid))
            {
                chunkdata_.itemDatas.Add(item.Item_Data);
                Debug.Log($"��� {item.name}   �� {chunkdata_.chunkName}  (GUID:{item.Item_Data.Guid})");
            }
            else
            {
                Debug.LogWarning($"�ظ���Ʒ {item.name}   ������");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"������Ʒ {item.name}   ʱ��������: {e.Message}");
        }
    }
    #endregion
}