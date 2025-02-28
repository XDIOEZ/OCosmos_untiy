using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class m_MiniMap : MonoBehaviour
{
    public Image MiniMap;
    public TileSave tileSave;
    public Transform player;
    public List<TileBase> tileColorList;
    public List<Color> colorList;
    public Dictionary<TileBase, Color> colorDict;

    public void Start()
    {
        //初始化对照表
        colorDict = new Dictionary<TileBase, Color>();
        for (int i = 0; i < tileColorList.Count; i++)
        {
            colorDict.Add(tileColorList[i], colorList[i]);
        }
           
    }
    public void UpdateMiniMap()
    {

        //TODO 遍历tile获取距离玩家最近的Tilemap

       
        foreach (GameObject tile in tileSave.Loaded_TileMap_GameObject.Values)
        {
            tile.GetComponent<Tilemap>();

            //TODO 以最近的Tilemap为中心 单个TileMap的尺寸为200*200 及
         
            //TODO 遍历获取Tile块并根据对照表显示对应颜色赋值到Image上 Image 的大小为600*600

            //TODO 以player 所在区块为中心区块   ，显示其周围的Tile块

            //从而实现一个小地图


        }
    }

}
