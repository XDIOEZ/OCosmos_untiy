using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PrefabLauncher : MonoBehaviour
{
    public GameObject prefab; // 要发射的预制体
    public ObjectPool<GameObject> pool; // 对象池
    public Transform spawnPoint; // 发射点
/*    float speed = 5f; // 发射速度*/
    

    void Awake()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
            },
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            maxSize: 10 // 设置最大池大小
        );
    }

    public void LaunchPrefab(Vector2 direction, float speed, Quaternion rotation)
    {
        Debug.Log("LaunchPrefab");
        GameObject instance = pool.Get(); // 从池中获取对象
        instance.transform.position = spawnPoint.position; // 设置发射点位置
        instance.transform.rotation = rotation; // 设置旋转方向

        // 获取 Rigidbody2D 组件
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 使用旋转的方向计算速度
            Vector2 launchDirection = rotation * Vector2.right; // 将子弹方向设置为右方向的旋转结果
            rb.velocity = launchDirection.normalized * speed; // 设置速度
        }

        instance.GetComponent<Item>().Use(); // 使用弹药
    }
}
