using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnChunk : BasePanel
{
    void Start()
    {
        GetControl<Button>("SpawnButton").onClick.AddListener(ClickStart);
    }
    void ClickStart()
    {
        Debug.Log("Click Start");
        Debug.Log("Spawn Chunk");
    }
}
