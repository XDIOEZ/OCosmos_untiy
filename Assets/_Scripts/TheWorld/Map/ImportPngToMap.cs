using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ImportPngToMap : MonoBehaviour
{
#region 字段定义

    // 存储所有贴图的列表
    public List<Texture2D> textures;
    
    // 存储贴图的字典
    [ShowInInspector]
    public Dictionary<string, Texture2D> texturesDictionary = new Dictionary<string, Texture2D>();
    
    // 获取地图字典的属性
    public TheWorld_ChunkDataManager Map_dic
    {
        get { return TheWorld.Instance.map_Dic; }
    }
    
    // 测试颜色列表
    public List<Color> testColor;
    
    // PNG尺寸
    public Vector2Int PngSize;
    
    // 地图数据列表
    public List<Map_Data> map_Data;

    // 设置为public的公共参数
    public float threshold = 0.1f; // 您可以根据需要调整这个值

#endregion

#region Unity生命周期方法

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PrintMapDataChunks();
        }
    }

    /// <summary>
    /// 开始导入地图数据列表
    /// </summary>
    [Button("开始mapdata列表导入")]
    public void StartImport()
    {
        map_Data = GetComponent<MapDataGenerator>().generatedMaps;
        if (map_Data != null)
        GetAllPng("Map");
    }

#endregion

#region 贴图处理方法

    /// <summary>
    /// 获取所有贴图
    /// </summary>
    /// <param name="label">标签</param>
    /// <param name="PngName">PNG名称</param>
    [Button("获取所有贴图")]
    public void GetAllPng(string label = "TheWorld,Map", string PngName = "")
    {
        var handle = Addressables.LoadAssetsAsync<Texture2D>(label, null);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                List<Texture2D> textures = new List<Texture2D>();
                textures.AddRange(op.Result);
                if (textures != null)
                {
                    foreach (var texture in textures)
                    {
                        if (texture != null)
                        {
                            string textureName = texture.name;
                            if (PngName == string.Empty)
                            {
                                texturesDictionary[textureName] = texture;
                            }
                            else if (textureName == PngName)
                            {
                                texturesDictionary[textureName] = texture;
                                break;
                            }
                        }
                    }
                    if (PngName != string.Empty && !texturesDictionary.ContainsKey(PngName))
                    {
                        Debug.LogWarning($"No texture found with name '{PngName}'.");
                    }
                }
                else
                {
                    Debug.LogError("Loaded textures array is null.");
                }
            }
            else
            {
                Debug.Log("Failed to load textures from Addressables.");
            }
        };
    }

#endregion

#region 地图数据处理方法

    /// <summary>
    /// 遍历导入
    /// </summary>
    [Button("遍历导入")]
    public void Test()
    {
        map_Data = GetComponent<MapDataGenerator>().generatedMaps;
        foreach (var mapData in map_Data)
        {
            ImportPngToMapData(mapData);
        }
    }

    /// <summary>
    /// 打印地图数据块信息
    /// </summary>
    private void PrintMapDataChunks()
    {
        int i = 0;  // 初始化 i
        int j = 0;  // 初始化 j

        for (int index = 0; index < map_Data.Count; index++)
        {
            var mapData = this.map_Data[index];

            // 使用 XDTool 打印字典
            XDTool.PrintDic<string, ChunkData>(mapData.AllChunksData_Dic);

            // 构建特定 Chunk 的键
            string chunkKey = $"Chunk_{i * 400}_{j * 400}";

            // 检查 Chunk 是否存在于字典中
            if (mapData.AllChunksData_Dic.ContainsKey(chunkKey))
            {
                // 使用 XDTool 打印 blockDatas 列表
                XDTool.PrintList<BlockData>(mapData.AllChunksData_Dic[chunkKey].blockDatas);
            }
            else
            {
                // 如果 Chunk 不存在，则输出错误信息
                Debug.LogError($"Chunk {chunkKey} does not exist in map_Data {mapData.MapName}");
            }

            // 更新索引 i 和 j
            i++;
            if (i >= TheWorld.Instance.WorldDataJson.WorldSize.x / TheWorld.Instance.WorldDataJson.MapTileSize.x)
            {
                i = 0;
                j++;
            }
        }
    }

    /// <summary>
    /// 导入PNG到地图数据
    /// </summary>
    public void Import()
    {
        map_Data = GetComponent<MapDataGenerator>().generatedMaps;

        int i = 1;  // 初始化 i
        int j = 1;  // 初始化 j
        for (int indexXx = 0; indexXx < map_Data.Count; indexXx++)
        {
            // 获取贴图的像素数据
            Color[] colors = textures[indexXx].GetPixels();
            int width = textures[indexXx].width;
            int height = textures[indexXx].height;

            PngSize = new Vector2Int(width, height);

            int _Index = 0;
            Vector2Int ChunkSize = Map_dic.ChunkSize;

            // 预计算颜色范围
            float[] thresholds = new float[3];
            for (int k = 0; k < 3; k++)
            {
                thresholds[k] = (testColor[k].r - threshold) * (testColor[k].r - threshold) +
                                (testColor[k].g - threshold) * (testColor[k].g - threshold) +
                                (testColor[k].b - threshold) * (testColor[k].b - threshold);
            }

            // 使用并行处理遍历每个像素
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _Index = y * width + x;

                    // 获取当前像素的颜色
                    Color color = colors[_Index];

                    // 计算属于哪个Chunk
                    int _x = (x / ChunkSize.x) * ChunkSize.x;
                    int _y = (y / ChunkSize.y) * ChunkSize.y;

                    string IndexTOChunk = $"Chunk_{_x + i * width}_{_y + j * height}";

                    // 获取当前Chunk的数据
                    List<BlockData> _blockData = map_Data[indexXx].AllChunksData_Dic[IndexTOChunk].blockDatas;

                    // 更新索引
                    int relativeX = x % ChunkSize.x;
                    int relativeY = y % ChunkSize.y;

                    int chunkIndex = relativeY * ChunkSize.x + relativeX;

                    float distance = (color.r - testColor[0].r) * (color.r - testColor[0].r) +
                                    (color.g - testColor[0].g) * (color.g - testColor[0].g) +
                                    (color.b - testColor[0].b) * (color.b - testColor[0].b);

                    if (distance < thresholds[0])
                    {
                        _blockData[chunkIndex].name = "Water";
                    }
                    else if (distance < thresholds[1])
                    {
                        _blockData[chunkIndex].name = "Sand";
                    }
                    else if (distance < thresholds[2])
                    {
                        _blockData[chunkIndex].name = "Ice";
                    }
                    else
                    {
                        _blockData[chunkIndex].name = "Grass";
                    }
                }
            }
            i++;
            if (i >= TheWorld.Instance.WorldDataJson.WorldSize.x / TheWorld.Instance.WorldDataJson.MapTileSize.x)
            {
                i = 0;
                j++;
            }
        }
    }

    /// <summary>
    /// 导入PNG到特定地图数据
    /// </summary>
    /// <param name="map_Data">地图数据</param>
    public void ImportPngToMapData(Map_Data map_Data)
    {
        string MapName = map_Data.MapName;
        Color[] colors = texturesDictionary[MapName].GetPixels();
        int Width = texturesDictionary[MapName].width;
        int Height = texturesDictionary[MapName].height;

        float[] thresholds = new float[3];
        for (int k = 0; k < 3; k++)
        {
            thresholds[k] = (testColor[k].r - threshold) * (testColor[k].r - threshold) +
                            (testColor[k].g - threshold) * (testColor[k].g - threshold) +
                            (testColor[k].b - threshold) * (testColor[k].b - threshold);
        }

        // 遍历每个像素
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int _Index = y * Width + x;

                // 获取当前像素的颜色
                Color color = colors[_Index];

                // 计算属于哪个Chunk
                int _x = (x / Map_dic.ChunkSize.x) * Map_dic.ChunkSize.x;
                int _y = (y / Map_dic.ChunkSize.y) * Map_dic.ChunkSize.y;

                string IndexTOChunk = $"Chunk_" +
                    $"{_x + map_Data.MapCenter.x - TheWorld.Instance.WorldDataJson.MapTileSize.x / 2}" +
                    $"_{_y + map_Data.MapCenter.y - TheWorld.Instance.WorldDataJson.MapTileSize.y / 2}";

                // 获取当前Chunk的数据
                List<BlockData> _blockData = map_Data.AllChunksData_Dic[IndexTOChunk].blockDatas;

                // 更新索引
                int relativeX = x % Map_dic.ChunkSize.x;
                int relativeY = y % Map_dic.ChunkSize.y;

                int chunkIndex = relativeY * Map_dic.ChunkSize.x + relativeX;

                float distance = (color.r - testColor[0].r) * (color.r - testColor[0].r) +
                                (color.g - testColor[0].g) * (color.g - testColor[0].g) +
                                (color.b - testColor[0].b) * (color.b - testColor[0].b);

                if (distance < thresholds[0])
                {
                    _blockData[chunkIndex].name = "Water";
                }
                else if (distance < thresholds[1])
                {
                    _blockData[chunkIndex].name = "Sand";
                }
                else if (distance < thresholds[2])
                {
                    _blockData[chunkIndex].name = "Ice";
                }
                else
                {
                    _blockData[chunkIndex].name = "Grass";
                }
            }
        }
    }

#endregion
}
