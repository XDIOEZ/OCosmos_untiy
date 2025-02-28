using UnityEngine;

public class InertiaFollow : MonoBehaviour
{
    #region Variables
    public Rigidbody2D rb; // 刚体组件

    public Transform target; // 跟随目标

    public float speed = 10f; // 基础速度

    public float maxSpeed = 20f; // 最大速度限制

    public float rotationSpeed = 360f; // 旋转速度（每秒度数）

    public float stopDistance = 0.3f; // 停止距离

    public Transform blade; // 剑刃

    public float CurrentSpeed; // 当前速度

    private Vector3 lastTargetPosition; // 上一帧的目标位置

    private float targetStillTime = 0f; // 目标静止时间

    public float stopThreshold = 1f; // 静止判定的时间阈值（秒）

    public DistanceJoint2D joint2D; // 距离关节组件

    public Transform weaponUser;
    #endregion

    #region MonoBehaviour Methods

    public void FollowUser()
    {
        
       joint2D.connectedAnchor = (Vector2)weaponUser.position;
        Debug.Log("Weapon User Position: " + weaponUser.position);

    }
    void Start()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D 未设置！请设置它。");
        }
        if (target == null)
        {
            Debug.LogError("目标 Transform 未设置！请设置它。");
        }
        if (blade == null)
        {
            Debug.LogError("剑刃 Transform 未设置！请设置它。");
        }
    }

    void FixedUpdate()
    {
        FollowTarget();

        RotateBladeTowardsTarget();
        // 保留小数点后两位
        CurrentSpeed = Mathf.Round(rb.velocity.magnitude * 100) / 100;

        FollowUser();
    }
    #endregion

    #region Custom Methods

    private void FollowTarget()
    {
        Vector3 currentTargetPosition = target.position;
        Vector2 direction = (currentTargetPosition - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, currentTargetPosition);

        if (Vector3.Distance(currentTargetPosition, lastTargetPosition) < 0.01f)
        {
            targetStillTime += Time.deltaTime;
        }
        else
        {
            targetStillTime = 0f;
        }

        if (targetStillTime >= stopThreshold)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (distance > stopDistance)
        {
            float targetSpeed = Mathf.Clamp(speed, 0f, maxSpeed);
            Vector2 desiredVelocity = direction * targetSpeed;
            Vector2 force = desiredVelocity - rb.velocity;
            force = Vector2.ClampMagnitude(desiredVelocity, maxSpeed);
            rb.AddForce(force, ForceMode2D.Force);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        lastTargetPosition = currentTargetPosition;
    }
    private void RotateBladeTowardsTarget()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        blade.rotation = Quaternion.Lerp(
            blade.rotation,
            Quaternion.Euler(0, 0, targetAngle),
            rotationSpeed * Time.deltaTime / 360f
        );
    }
    #endregion
}