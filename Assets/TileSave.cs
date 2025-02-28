#region �����ռ�����
using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UltEvents;
using UnityEngine.UIElements;
using NaughtyAttributes;
using ButtonAttribute = Sirenix.OdinInspector.ButtonAttribute;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using zFramework.Internal;
using System.Threading;
#endregion

public class TileSave : MonoBehaviour
{
    #region ��������
    //�Ƿ�д�����
    public bool isSaveToSimpleGrid = false;

[Header("Prefab ��Դ������")]
public GameObject TileMap_Prefab; // TileMap ��Ԥ����
public GameObject TileMaps_Parent; // �������� TileMap �ĸ�����

[Header("����ͼ���")]
public string address = "Assets/_Data/TileMap/TileMapData.bin"; // ���� TileMap ���ݵĵ�ַ

[Header("TileMap ����")]
public Transform player; // ��Ҷ���
public int tileLoadSpeed = 100; // ÿ֡���ص� Tile ����
public int LoadRange = 100; // ���ط�Χ
public float UnloadRate = 1.5f; // ж��ϵ����
[SortingLayer]
public string TileMapLayer; // TileMap �������

[Header("��ʵ������ TileMap")]
[ShowInInspector]
public Dictionary<string, GameObject> Loaded_TileMap_GameObject = new Dictionary<string, GameObject>(); // �����Ӷ�����ֵ䣬Key Ϊ���ƣ�Value Ϊ GameObject
    #endregion

    #region ˽�б���
    [Header("ȫ�����ڴ��е�Tile����")]
    public SimpleGrid loaded_TileMap_Data; // ������ص�Tile����

    // ����һ��HashSet���������ڼ����е�Tilemap
    private HashSet<string> loadingTileMaps = new HashSet<string>();

    public SimpleGrid Loaded_TileMap_Data { get => loaded_TileMap_Data; set => loaded_TileMap_Data = value; }
    #endregion

    #region Unity��������
    public void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        Invoke("Init", 2);
    }

    public void Init()
    {
        Read_TileMaps_FormDisk();
        StartCoroutine(TileUpdateCoroutine());
    }

    IEnumerator TileUpdateCoroutine()
    {
        while (true)
        {
            if (player != null)
            {
                foreach (Vector2Int pos in GetNearChunks(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y), LoadRange))
                {
                    TryLoadNearTileMap(pos);
                    DestroyUnNearTileMap(player.transform.position, (int)(LoadRange * UnloadRate));
                }
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }
    }


    #endregion

    #region ���� ��� ɾ��

    /// <summary>
/// ����Ŀ����Χ��TileMap
/// </summary>
/// <param name="position_">Ŀ��λ��</param>
    public void TryLoadNearTileMap(Vector2 position_)
{
    // ��ȡָ��λ�õ�Tile
    SimpleTile simpleTile = Loaded_TileMap_Data.GetTile(position_);

    // ���ָ��λ�õ�Tile�����ڣ�����һ���µ�Tilemap
    if (simpleTile == null)
    {
        Debug.LogWarning("���Դ�����Tilemap");

        // ��ȡָ��λ������Tile�����ĵ�����
        Vector2 createPos = Loaded_TileMap_Data.GetTileCenter(position_);

        // �����ĵ�����ת��Ϊ�ַ�������ΪTilemap������
        string nameV2 = $"{createPos.x},{createPos.y}";

        // ����һ���µ�SimpleTileʵ��
        simpleTile = new SimpleTile(nameV2);

        // ����Tilemap��GameObject
        GameObject tilemapGo = InitATileMap(nameV2, simpleTile);

        // ����Tilemap��Transform����
        tilemapGo.transform.localPosition = createPos;
        tilemapGo.transform.localRotation = Quaternion.identity;
        tilemapGo.transform.localScale = Vector3.one;

        // ��������Tilemap��ӵ������ֵ���
        Loaded_TileMap_GameObject.Add(simpleTile.name, tilemapGo);

        // ���µ�SimpleTile��ӵ��Ѽ��ص�Tile������
        Loaded_TileMap_Data.TileMaps.Add(simpleTile.name, simpleTile);
    }
    else // ���ָ��λ�õ�Tile�Ѵ��ڣ��������е�Tilemap
    {
        // ���Tilemap�Ѿ������ڹ����ֵ��У�ֱ�ӷ���
        if (Loaded_TileMap_GameObject.ContainsKey(simpleTile.name))
        {
            return;
        }
        else
        {
            Load_A_TileMap(simpleTile);
        }
    }
}

    /// <summary>
    /// �첽�����Ѿ��������ֵ��е�Tilemap
    /// </summary>
    /// <param name="tilemapGo">Tilemap��GameObject</param>
    /// <param name="simpleTile_">SimpleTile����</param>
    private IEnumerator FillTileToMapAsync(GameObject tilemapGo, SimpleTile simpleTile_)
{
    // ��Tilemap������ӵ����ڼ��صļ�����
    loadingTileMaps.Add(simpleTile_.name);

    // ��ʼ�����������
    int processCounter = 0;

    // ��ȡTilemap���
    Tilemap tilemap = tilemapGo.GetComponent<Tilemap>();

    // ���TileBaseΪ�գ���ʼ��TileBase����ɼ���
    if (simpleTile_.TileBase == null)
    {
        simpleTile_.TileBase = new Dictionary<string, string>();
        Loaded_TileMap_GameObject.Add(simpleTile_.name, tilemapGo);
        loadingTileMaps.Remove(simpleTile_.name);
        Debug.Log($"Tilemap '{simpleTile_.name}' �첽�������");
        yield return null;
    }

    // ����TileBase�ֵ��е�ÿ����Ŀ
    foreach (var tileEntry in simpleTile_.TileBase)
    {
        // ���������ַ���
        if (!ParseCoordinate(tileEntry.Key, out int x, out int y))
            continue;

        // ����Vector3Int��ʾ��Ԫ��λ��
        Vector3Int cellPosition = new(x, y, 0);

        // �ӻ����л�ȡTileBase����
        TileBase tile = GetTileByName(tileEntry.Value);

        // ���TileBase����Ϊ�գ�����Tilemap��Tile
        if (tile != null)
        {
            tilemap.SetTile(cellPosition, tile);
        }

        // ÿ����һ��������Tile�󣬵ȴ�һ֡
        if (++processCounter % tileLoadSpeed == 0)
            yield return null;
    }

    // ��Tilemap��ӵ������ֵ���
    Loaded_TileMap_GameObject[simpleTile_.name] = tilemapGo;

    // �����ڼ��صļ������Ƴ�Tilemap����
    loadingTileMaps.Remove(simpleTile_.name);

    // ���������ɵ���־
   // Debug.Log($"Tilemap '{simpleTile_.name}' �첽�������");
}

    [Button("�������Ŀ����Χ��TileMap")]
    public void DestroyUnNearTileMap(Vector2 position, int MaxRange = 100)
    {
        List<string> keysToDelete = new List<string>();

        foreach (var entry in Loaded_TileMap_GameObject)
        {
            float distance = Vector2.Distance(entry.Value.transform.position, position);

            if (distance > MaxRange)
            {
                if (loadingTileMaps.Contains(entry.Key))
                    continue;

                keysToDelete.Add(entry.Key);
            }
        }

        foreach (string key in keysToDelete)
        {
            Destroy_A_TileMap(key);
        }
    }

    #endregion
    #region ���غ�ж�ط���
    public void Load_A_TileMap(SimpleTile simpleTile)
    {
        // ����Tilemap��GameObject
        GameObject tilemapGo = InitATileMap(simpleTile.name, simpleTile);

        // ��������Tilemap��ӵ������ֵ���
        Loaded_TileMap_GameObject.Add(simpleTile.name, tilemapGo);

        // �첽���Tilemap������
        StartCoroutine(FillTileToMapAsync(tilemapGo, simpleTile));
    }
    public void Destroy_A_TileMap(string _name)
    {
        ExportMapAsync(Loaded_TileMap_GameObject[_name].GetComponent<Tilemap>());
        Loaded_TileMap_GameObject.Remove(_name);
    }
    #endregion

    #region ��д����
    [Button ("�Ӵ��̶�ȡTileMap����")]
    public void Read_TileMaps_FormDisk()
    {
        if (string.IsNullOrEmpty(address))
        {
            Debug.LogError("Load address is empty.");
            return;
        }

        if (!File.Exists(address))
        {
            Debug.LogError("File not found at: " + address);
            return;
        }

        byte[] bytes = File.ReadAllBytes(address);
        Loaded_TileMap_Data = MemoryPackSerializer.Deserialize<SimpleGrid>(bytes);

        if (Loaded_TileMap_Data == null)
        {
            Debug.LogError("Failed to deserialize data!");
            return;
        }
    }

    [Button ("���浱ǰ�����Ѿ�ʵ������TileMap&&д�뵽����")]
    /// <summary>
    /// �޸�������TileMap����
    /// </summary>
    public void Save_TileMaps_TODisk()
    {
        SaveAllTileMap();
        // ���л�loadedTileData����Ϊ�ֽ�����
        byte[] bytes = MemoryPackSerializer.Serialize(Loaded_TileMap_Data);

        // ��鱣���ַ�Ƿ�Ϊ��
        if (string.IsNullOrEmpty(address))
        {
            Debug.LogError("Save address is empty.");
            return;
        }

        // ��ȡ�����ַ��Ŀ¼·��
        string directory = Path.GetDirectoryName(address);

        // ���Ŀ¼�����ڣ��򴴽�Ŀ¼
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // ���ֽ�����д��ָ�����ļ�·��
        File.WriteAllBytes(address, bytes);

        // �������ɹ�����־
        Debug.Log($"Level saved to: {address}");
    }
    public void SaveAllTileMap()
    {
        // �Ȼ�ȡ���� Key ���б��������ʱ�޸� Dictionary ���´���
        List<string> keys = new List<string>(Loaded_TileMap_GameObject.Keys);

        foreach (var key in keys)
        {
            // ȷ�� key ��Ȼ���ڣ����� `KeyNotFoundException`
            if (Loaded_TileMap_GameObject.ContainsKey(key))
            {
                Destroy_A_TileMap(key);
            }
        }
    }

    #endregion

    #region �������� 

    public static List<Vector2Int> GetNearChunks(Vector2Int currentChunk, int range = 100)
    {
        List<Vector2Int> nearChunks = new List<Vector2Int>
       {
           new Vector2Int(currentChunk.x, currentChunk.y),
           new Vector2Int(currentChunk.x - range, currentChunk.y - range),
           new Vector2Int(currentChunk.x - range, currentChunk.y),
           new Vector2Int(currentChunk.x - range, currentChunk.y + range),
           new Vector2Int(currentChunk.x, currentChunk.y - range),
           new Vector2Int(currentChunk.x, currentChunk.y + range),
           new Vector2Int(currentChunk.x + range, currentChunk.y - range),
           new Vector2Int(currentChunk.x + range, currentChunk.y),
           new Vector2Int(currentChunk.x + range, currentChunk.y + range)
       };

        return nearChunks;
    }

    public void ExportMapAsync(Tilemap tilemap)
    {
        Loom.Post(async () =>
        {
            BoundsInt area = tilemap.cellBounds;
            SimpleTile Save_TileMap = new SimpleTile(tilemap.name)
            {
                position = tilemap.transform.position,
                rotation = tilemap.transform.rotation,
                scale = tilemap.transform.localScale,
                TileBase = new Dictionary<string, string>()
            };

            Dictionary<Vector3Int, string> tileData = new Dictionary<Vector3Int, string>();

            await Loom.ToMainThread;

            for (int x = area.xMin; x < area.xMax; x++)
            {
                for (int y = area.yMin; y < area.yMax; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);

                    TileBase tile = tilemap.GetTile(position);

                    if (tile != null)
                    {
                        tileData[position] = tile.name;
                    }
                }
            }

            await Loom.ToOtherThread;

            foreach (var kvp in tileData)
            {
                string key = $"{kvp.Key.x},{kvp.Key.y}";

                Save_TileMap.TileBase.Add(key, kvp.Value);
            }

            await Loom.ToMainThread;

            Loaded_TileMap_Data.TileMaps[tilemap.name] = Save_TileMap;
            Destroy(tilemap.gameObject);
        });
    }


/// <summary>
/// ��������ʼ��һ����Ƭ��ͼ����
/// </summary>
/// <param name="_name">Ҫ��������Ƭ��ͼ��������ơ�</param>
/// <param name="simpleTile">����λ�á���ת��������Ϣ�ļ���Ƭ����</param>
/// <returns>���ش�������ʼ�������Ƭ��ͼ��Ϸ����</returns>
private GameObject InitATileMap(string _name, SimpleTile simpleTile)
{
    // ʵ����һ����Ƭ��ͼ prefab�����������ʼλ�ú���תΪ��͵�λ����
    GameObject tilemapGo = Instantiate(TileMap_Prefab, Vector3.zero, Quaternion.identity);
    
    // Ϊ��Ƭ��ͼ������������
    tilemapGo.name = _name;
    
    // ����Ƭ��ͼ����ı任����Ϊ TileMaps_Parent ���Ӷ����Ա�����������
    tilemapGo.transform.SetParent(TileMaps_Parent.transform);
    
    // ���� Save_TileMap �е���Ϣ������Ƭ��ͼ�����λ�á���ת������
    tilemapGo.transform.localPosition = simpleTile.position;
    tilemapGo.transform.localRotation = simpleTile.rotation;
    tilemapGo.transform.localScale = simpleTile.scale;
    
    // ���ش�������ʼ�������Ƭ��ͼ��Ϸ����
    return tilemapGo;
}

    private bool ParseCoordinate(string coordinateString, out int x, out int y)
    {
        x = 0;
        y = 0;
        string[] coordParts = coordinateString.Split(',');
        if (coordParts.Length == 2 &&
            int.TryParse(coordParts[0], out x) &&
            int.TryParse(coordParts[1], out y))
        {
            return true;
        }
        else
        {
            Debug.LogWarning($"Invalid tile position format: {coordinateString}");
            return false;
        }
    }

    private TileBase GetTileByName(string tileName)
    {
        return GameResManager.Instance.tileBaseDict[tileName];
    }
    #endregion
}

#region ������ 
[MemoryPackable]
[System.Serializable]
public partial class SimpleGrid
{
    [ShowInInspector]
    [Header("SimpleGrid ��Ϣ")]
    public Dictionary<string, SimpleTile> TileMaps = new Dictionary<string, SimpleTile>();

    private const float tileSize = 200;

    public SimpleTile GetTile(Vector2 position)
    {
        Vector2 tileCenter = GetTileCenter(position);
        string key = $"{(int)tileCenter.x},{(int)tileCenter.y}";

        if (TileMaps.TryGetValue(key, out SimpleTile result))
        {
            return result;
        }
        else
        {
            Debug.LogWarning($"Tile not found at {tileCenter}");
            return null;
        }
    }

    public Vector2 GetTileCenter(Vector2 position)
    {
        float centerX = Mathf.Round(position.x / tileSize) * tileSize;
        float centerY = Mathf.Round(position.y / tileSize) * tileSize;

        return new Vector2(centerX, centerY);
    }
}

[MemoryPackable]
[System.Serializable]
public partial class SimpleTile
{
    public string name;
    public Dictionary<string, string> TileBase;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    [MemoryPackConstructor]
    public SimpleTile(string name)
    {
        this.name = name;
    }
}
#endregion
