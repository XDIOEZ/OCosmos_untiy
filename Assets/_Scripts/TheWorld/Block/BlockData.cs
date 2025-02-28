using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MemoryPackable]
[System.Serializable]
public partial class BlockData
{
    public string name;

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public void SetTransform(Transform transform)
    {
        position.x = transform.position.x;
        position.y = transform.position.y;
        position.z = transform.position.z;

        rotation = transform.rotation.eulerAngles;

        scale = transform.localScale;
    }

    //¹¹Ôìº¯Êý
    [MemoryPackConstructor]
    public BlockData(string name)
    {
        this.name = name;
    }
}
