using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapCDManager : MonoBehaviour
{
    public TileSave tileSave;
    // Start is called before the first frame update
    void Start()
    {
        tileSave = GetComponent<TileSave>();
    }


}
