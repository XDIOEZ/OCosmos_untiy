#region 命名空间引用
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
    #region 公共变量
    //是否写入完毕
    public bool isSaveToSimpleGrid = false;

[Header("Prefab 资源和网格")]
public GameObject TileMap_Prefab; // TileMap 的预制体
public GameObject TileMaps_Parent; // 包含所有 TileMap 的父对象

[Header("保存和加载")]
public string address = "Assets/_Data/TileMap/TileMapData.bin"; // 保存 TileMap 数据的地址

[Header("TileMap 属性")]
public Transform player; // 玩家对象
public int tileLoadSpeed = 100; // 每帧加载的 Tile 数量
public int LoadRange = 100; // 加载范围
public float UnloadRate = 1.5f; // 卸载系数率
[SortingLayer]
public string TileMapLayer; // TileMap 的排序层

[Header("已实例化的 TileMap")]
[ShowInInspector]
public Dictionary<string, GameObject> Loaded_TileMap_GameObject = new Dictionary<string, GameObject>(); // 管理子对象的字典，Key 为名称，Value 为 GameObject
    #endregion

    #region 私有变量
    [Header("全部在内存中的Tile数据")]
    public SimpleGrid loaded_TileMap_Data; // 保存加载的Tile数据

    // 新增一个HashSet来跟踪正在加载中的Tilemap
    private HashSet<string> loadingTileMaps = new HashSet<string>();

    public SimpleGrid Loaded_TileMap_Data { get => loaded_TileMap_Data; set => loaded_TileMap_Data = value; }
    #endregion

    #region Unity生命周期
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

    #region 加载 填充 删除

    /// <summary>
/// 加载目标周围的TileMap
/// </summary>
/// <param name="position_">目标位置</param>
    public void TryLoadNearTileMap(Vector2 position_)
{
    // 获取指定位置的Tile
    SimpleTile simpleTile = Loaded_TileMap_Data.GetTile(position_);

    // 如果指定位置的Tile不存在，创建一个新的Tilemap
    if (simpleTile == null)
    {
        Debug.LogWarning("尝试创建新Tilemap");

        // 获取指定位置所在Tile的中心点坐标
        Vector2 createPos = Loaded_TileMap_Data.GetTileCenter(position_);

        // 将中心点坐标转换为字符串，作为Tilemap的名称
        string nameV2 = $"{createPos.x},{createPos.y}";

        // 创建一个新的SimpleTile实例
        simpleTile = new SimpleTile(nameV2);

        // 创建Tilemap的GameObject
        GameObject tilemapGo = InitATileMap(nameV2, simpleTile);

        // 设置Tilemap的Transform属性
        tilemapGo.transform.localPosition = createPos;
        tilemapGo.transform.localRotation = Quaternion.identity;
        tilemapGo.transform.localScale = Vector3.one;

        // 将创建的Tilemap添加到管理字典中
        Loaded_TileMap_GameObject.Add(simpleTile.name, tilemapGo);

        // 将新的SimpleTile添加到已加载的Tile数据中
        Loaded_TileMap_Data.TileMaps.Add(simpleTile.name, simpleTile);
    }
    else // 如果指定位置的Tile已存在，加载已有的Tilemap
    {
        // 如果Tilemap已经存在于管理字典中，直接返回
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
    /// 异步加载已经存在在字典中的Tilemap
    /// </summary>
    /// <param name="tilemapGo">Tilemap的GameObject</param>
    /// <param name="simpleTile_">SimpleTile对象</param>
    private IEnumerator FillTileToMapAsync(GameObject tilemapGo, SimpleTile simpleTile_)
{
    // 将Tilemap名称添加到正在加载的集合中
    loadingTileMaps.Add(simpleTile_.name);

    // 初始化处理计数器
    int processCounter = 0;

    // 获取Tilemap组件
    Tilemap tilemap = tilemapGo.GetComponent<Tilemap>();

    // 如果TileBase为空，初始化TileBase并完成加载
    if (simpleTile_.TileBase == null)
    {
        simpleTile_.TileBase = new Dictionary<string, string>();
        Loaded_TileMap_GameObject.Add(simpleTile_.name, tilemapGo);
        loadingTileMaps.Remove(simpleTile_.name);
        Debug.Log($"Tilemap '{simpleTile_.name}' 异步加载完成");
        yield return null;
    }

    // 遍历TileBase字典中的每个条目
    foreach (var tileEntry in simpleTile_.TileBase)
    {
        // 解析坐标字符串
        if (!ParseCoordinate(tileEntry.Key, out int x, out int y))
            continue;

        // 创建Vector3Int表示单元格位置
        Vector3Int cellPosition = new(x, y, 0);

        // 从缓存中获取TileBase对象
        TileBase tile = GetTileByName(tileEntry.Value);

        // 如果TileBase对象不为空，设置Tilemap的Tile
        if (tile != null)
        {
            tilemap.SetTile(cellPosition, tile);
        }

        // 每处理一定数量的Tile后，等待一帧
        if (++processCounter % tileLoadSpeed == 0)
            yield return null;
    }

    // 将Tilemap添加到管理字典中
    Loaded_TileMap_GameObject[simpleTile_.name] = tilemapGo;

    // 从正在加载的集合中移除Tilemap名称
    loadingTileMaps.Remove(simpleTile_.name);

    // 输出加载完成的日志
   // Debug.Log($"Tilemap '{simpleTile_.name}' 异步加载完成");
}

    [Button("清除不在目标周围的TileMap")]
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
    #region 加载和卸载方法
    public void Load_A_TileMap(SimpleTile simpleTile)
    {
        // 创建Tilemap的GameObject
        GameObject tilemapGo = InitATileMap(simpleTile.name, simpleTile);

        // 将创建的Tilemap添加到管理字典中
        Loaded_TileMap_GameObject.Add(simpleTile.name, tilemapGo);

        // 异步填充Tilemap的数据
        StartCoroutine(FillTileToMapAsync(tilemapGo, simpleTile));
    }
    public void Destroy_A_TileMap(string _name)
    {
        ExportMapAsync(Loaded_TileMap_GameObject[_name].GetComponent<Tilemap>());
        Loaded_TileMap_GameObject.Remove(_name);
    }
    #endregion

    #region 读写方法
    [Button ("从磁盘读取TileMap数据")]
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

    [Button ("保存当前所有已经实例化的TileMap&&写入到磁盘")]
    /// <summary>
    /// 修复并保存TileMap数据
    /// </summary>
    public void Save_TileMaps_TODisk()
    {
        SaveAllTileMap();
        // 序列化loadedTileData对象为字节数组
        byte[] bytes = MemoryPackSerializer.Serialize(Loaded_TileMap_Data);

        // 检查保存地址是否为空
        if (string.IsNullOrEmpty(address))
        {
            Debug.LogError("Save address is empty.");
            return;
        }

        // 获取保存地址的目录路径
        string directory = Path.GetDirectoryName(address);

        // 如果目录不存在，则创建目录
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 将字节数组写入指定的文件路径
        File.WriteAllBytes(address, bytes);

        // 输出保存成功的日志
        Debug.Log($"Level saved to: {address}");
    }
    public void SaveAllTileMap()
    {
        // 先获取所有 Key 的列表，避免遍历时修改 Dictionary 导致错误
        List<string> keys = new List<string>(Loaded_TileMap_GameObject.Keys);

        foreach (var key in keys)
        {
            // 确保 key 仍然存在，避免 `KeyNotFoundException`
            if (Loaded_TileMap_GameObject.ContainsKey(key))
            {
                Destroy_A_TileMap(key);
            }
        }
    }

    #endregion

    #region 辅助方法 

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
/// 创建并初始化一个瓦片地图对象。
/// </summary>
/// <param name="_name">要创建的瓦片地图对象的名称。</param>
/// <param name="simpleTile">包含位置、旋转和缩放信息的简单瓦片对象。</param>
/// <returns>返回创建并初始化后的瓦片地图游戏对象。</returns>
private GameObject InitATileMap(string _name, SimpleTile simpleTile)
{
    // 实例化一个瓦片地图 prefab，并设置其初始位置和旋转为零和单位矩阵
    GameObject tilemapGo = Instantiate(TileMap_Prefab, Vector3.zero, Quaternion.identity);
    
    // 为瓦片地图对象设置名称
    tilemapGo.name = _name;
    
    // 将瓦片地图对象的变换设置为 TileMaps_Parent 的子对象，以便进行网格管理
    tilemapGo.transform.SetParent(TileMaps_Parent.transform);
    
    // 根据 Save_TileMap 中的信息设置瓦片地图对象的位置、旋转和缩放
    tilemapGo.transform.localPosition = simpleTile.position;
    tilemapGo.transform.localRotation = simpleTile.rotation;
    tilemapGo.transform.localScale = simpleTile.scale;
    
    // 返回创建并初始化后的瓦片地图游戏对象
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

#region 辅助类 
[MemoryPackable]
[System.Serializable]
public partial class SimpleGrid
{
    [ShowInInspector]
    [Header("SimpleGrid 信息")]
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
