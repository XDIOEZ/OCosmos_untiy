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
        //��ʼ�����ձ�
        colorDict = new Dictionary<TileBase, Color>();
        for (int i = 0; i < tileColorList.Count; i++)
        {
            colorDict.Add(tileColorList[i], colorList[i]);
        }
           
    }
    public void UpdateMiniMap()
    {

        //TODO ����tile��ȡ������������Tilemap

       
        foreach (GameObject tile in tileSave.Loaded_TileMap_GameObject.Values)
        {
            tile.GetComponent<Tilemap>();

            //TODO �������TilemapΪ���� ����TileMap�ĳߴ�Ϊ200*200 ��
         
            //TODO ������ȡTile�鲢���ݶ��ձ���ʾ��Ӧ��ɫ��ֵ��Image�� Image �Ĵ�СΪ600*600

            //TODO ��player ��������Ϊ��������   ����ʾ����Χ��Tile��

            //�Ӷ�ʵ��һ��С��ͼ


        }
    }

}
