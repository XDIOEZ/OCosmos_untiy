using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : Item
{
    public Com_ItemData _data;

    public override ItemData Item_Data 
    {
        get => _data;
        set => _data = (Com_ItemData)value;
    }
/*    public override Item_Data GetData()
    {
        Debug.Log($"��ȡ{name}������....");
        return _data;
    }
    public override void SetData(Item_Data data)
    {
        Debug.Log($"����{name}������....");
        _data = (LogData)data;
    }*/

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
/*[MemoryPackable]
[System.Serializable]
public class LogData : Item_Data
{
    
}*/
