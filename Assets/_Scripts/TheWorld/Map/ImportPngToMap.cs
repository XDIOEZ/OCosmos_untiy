using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ImportPngToMap : MonoBehaviour
{
#region �ֶζ���

    // �洢������ͼ���б�
    public List<Texture2D> textures;
    
    // �洢��ͼ���ֵ�
    [ShowInInspector]
    public Dictionary<string, Texture2D> texturesDictionary = new Dictionary<string, Texture2D>();
    
    // ��ȡ��ͼ�ֵ������
    public TheWorld_ChunkDataManager Map_dic
    {
        get { return TheWorld.Instance.map_Dic; }
    }
    
    // ������ɫ�б�
    public List<Color> testColor;
    
    // PNG�ߴ�
    public Vector2Int PngSize;
    
    // ��ͼ�����б�
    public List<Map_Data> map_Data;

    // ����Ϊpublic�Ĺ�������
    public float threshold = 0.1f; // �����Ը�����Ҫ�������ֵ

#endregion

#region Unity�������ڷ���

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PrintMapDataChunks();
        }
    }

    /// <summary>
    /// ��ʼ�����ͼ�����б�
    /// </summary>
    [Button("��ʼmapdata�б���")]
    public void StartImport()
    {
        map_Data = GetComponent<MapDataGenerator>().generatedMaps;
        if (map_Data != null)
        GetAllPng("Map");
    }

#endregion

#region ��ͼ������

    /// <summary>
    /// ��ȡ������ͼ
    /// </summary>
    /// <param name="label">��ǩ</param>
    /// <param name="PngName">PNG����</param>
    [Button("��ȡ������ͼ")]
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

#region ��ͼ���ݴ�����

    /// <summary>
    /// ��������
    /// </summary>
    [Button("��������")]
    public void Test()
    {
        map_Data = GetComponent<MapDataGenerator>().generatedMaps;
        foreach (var mapData in map_Data)
        {
            ImportPngToMapData(mapData);
        }
    }

    /// <summary>
    /// ��ӡ��ͼ���ݿ���Ϣ
    /// </summary>
    private void PrintMapDataChunks()
    {
        int i = 0;  // ��ʼ�� i
        int j = 0;  // ��ʼ�� j

        for (int index = 0; index < map_Data.Count; index++)
        {
            var mapData = this.map_Data[index];

            // ʹ�� XDTool ��ӡ�ֵ�
            XDTool.PrintDic<string, ChunkData>(mapData.AllChunksData_Dic);

            // �����ض� Chunk �ļ�
            string chunkKey = $"Chunk_{i * 400}_{j * 400}";

            // ��� Chunk �Ƿ�������ֵ���
            if (mapData.AllChunksData_Dic.ContainsKey(chunkKey))
            {
                // ʹ�� XDTool ��ӡ blockDatas �б�
                XDTool.PrintList<BlockData>(mapData.AllChunksData_Dic[chunkKey].blockDatas);
            }
            else
            {
                // ��� Chunk �����ڣ������������Ϣ
                Debug.LogError($"Chunk {chunkKey} does not exist in map_Data {mapData.MapName}");
            }

            // �������� i �� j
            i++;
            if (i >= TheWorld.Instance.WorldDataJson.WorldSize.x / TheWorld.Instance.WorldDataJson.MapTileSize.x)
            {
                i = 0;
                j++;
            }
        }
    }

    /// <summary>
    /// ����PNG����ͼ����
    /// </summary>
    public void Import()
    {
        map_Data = GetComponent<MapDataGenerator>().generatedMaps;

        int i = 1;  // ��ʼ�� i
        int j = 1;  // ��ʼ�� j
        for (int indexXx = 0; indexXx < map_Data.Count; indexXx++)
        {
            // ��ȡ��ͼ����������
            Color[] colors = textures[indexXx].GetPixels();
            int width = textures[indexXx].width;
            int height = textures[indexXx].height;

            PngSize = new Vector2Int(width, height);

            int _Index = 0;
            Vector2Int ChunkSize = Map_dic.ChunkSize;

            // Ԥ������ɫ��Χ
            float[] thresholds = new float[3];
            for (int k = 0; k < 3; k++)
            {
                thresholds[k] = (testColor[k].r - threshold) * (testColor[k].r - threshold) +
                                (testColor[k].g - threshold) * (testColor[k].g - threshold) +
                                (testColor[k].b - threshold) * (testColor[k].b - threshold);
            }

            // ʹ�ò��д������ÿ������
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _Index = y * width + x;

                    // ��ȡ��ǰ���ص���ɫ
                    Color color = colors[_Index];

                    // ���������ĸ�Chunk
                    int _x = (x / ChunkSize.x) * ChunkSize.x;
                    int _y = (y / ChunkSize.y) * ChunkSize.y;

                    string IndexTOChunk = $"Chunk_{_x + i * width}_{_y + j * height}";

                    // ��ȡ��ǰChunk������
                    List<BlockData> _blockData = map_Data[indexXx].AllChunksData_Dic[IndexTOChunk].blockDatas;

                    // ��������
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
    /// ����PNG���ض���ͼ����
    /// </summary>
    /// <param name="map_Data">��ͼ����</param>
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

        // ����ÿ������
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int _Index = y * Width + x;

                // ��ȡ��ǰ���ص���ɫ
                Color color = colors[_Index];

                // ���������ĸ�Chunk
                int _x = (x / Map_dic.ChunkSize.x) * Map_dic.ChunkSize.x;
                int _y = (y / Map_dic.ChunkSize.y) * Map_dic.ChunkSize.y;

                string IndexTOChunk = $"Chunk_" +
                    $"{_x + map_Data.MapCenter.x - TheWorld.Instance.WorldDataJson.MapTileSize.x / 2}" +
                    $"_{_y + map_Data.MapCenter.y - TheWorld.Instance.WorldDataJson.MapTileSize.y / 2}";

                // ��ȡ��ǰChunk������
                List<BlockData> _blockData = map_Data.AllChunksData_Dic[IndexTOChunk].blockDatas;

                // ��������
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
