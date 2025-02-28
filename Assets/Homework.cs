using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homework : MonoBehaviour
{
void OnCollisionEnter2D(Collision2D collision)
{
    // 处理碰撞事件的代码
    Destroy(gameObject);
}

}
