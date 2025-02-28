using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

public class TEST : MonoBehaviour
{
    [ShowInInspector]
    public IFunction_Move move; // 用于移动的接口

    [Button("Stacy")]
    public void Stacy()
    {
        if (System.Object.ReferenceEquals(move, null))
        {
            Debug.Log("move 已被销毁！");
        }
        else
        {
            Debug.Log("move 仍然有效！");
        }
        move = null;
    }
}
