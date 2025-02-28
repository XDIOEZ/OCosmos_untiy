using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class WorkerData :ItemData
{
    [Header("存储插槽")]
    [ShowInInspector]
    public List<Inventory_Data> Inventory_Data_List = new List<Inventory_Data>(3);

}