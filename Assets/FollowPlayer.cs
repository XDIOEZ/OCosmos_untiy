using UnityEngine;

public class SmoothFollowPlayerWithRotation : MonoBehaviour
{
    public Transform target; // 目标 Transform
    public float moveSpeed = 5f; // 移动速度
    public float stopDistance = 1f; // 停止距离

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
    }

    private void FixedUpdate()
    {
        // 如果目标不存在，直接返回
        if (target == null) return;

        // 计算目标与当前物体之间的方向向量
        Vector2 direction = target.position - transform.position;

        // 检查与目标的距离
        if (direction.magnitude > stopDistance)
        {
            // 归一化方向向量，计算移动速度
            Vector2 move = direction.normalized * moveSpeed;

            // 使用 Rigidbody2D 移动物体
            rb.velocity = move;
        }
        else
        {
            // 当距离小于停止距离时，停止移动
            rb.velocity = Vector2.zero;
        }

        // 控制物体朝向目标
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 计算角度
        rb.rotation = angle; // 设置刚体旋转角度
    }

    public void Update()
    {
        transform.position = target.position; // 跟随目标
        transform.rotation = target.rotation; // 跟随目标的旋转
    }
}
