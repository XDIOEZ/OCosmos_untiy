using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

public class TEST : MonoBehaviour
{
    [ShowInInspector]
    public IFunction_Move move; // �����ƶ��Ľӿ�

    [Button("Stacy")]
    public void Stacy()
    {
        if (System.Object.ReferenceEquals(move, null))
        {
            Debug.Log("move �ѱ����٣�");
        }
        else
        {
            Debug.Log("move ��Ȼ��Ч��");
        }
        move = null;
    }
}
