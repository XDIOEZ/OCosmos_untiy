using UnityEngine;

public class InertiaFollow : MonoBehaviour
{
    #region Variables
    public Rigidbody2D rb; // �������

    public Transform target; // ����Ŀ��

    public float speed = 10f; // �����ٶ�

    public float maxSpeed = 20f; // ����ٶ�����

    public float rotationSpeed = 360f; // ��ת�ٶȣ�ÿ�������

    public float stopDistance = 0.3f; // ֹͣ����

    public Transform blade; // ����

    public float CurrentSpeed; // ��ǰ�ٶ�

    private Vector3 lastTargetPosition; // ��һ֡��Ŀ��λ��

    private float targetStillTime = 0f; // Ŀ�꾲ֹʱ��

    public float stopThreshold = 1f; // ��ֹ�ж���ʱ����ֵ���룩

    public DistanceJoint2D joint2D; // ����ؽ����

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
            Debug.LogError("Rigidbody2D δ���ã�����������");
        }
        if (target == null)
        {
            Debug.LogError("Ŀ�� Transform δ���ã�����������");
        }
        if (blade == null)
        {
            Debug.LogError("���� Transform δ���ã�����������");
        }
    }

    void FixedUpdate()
    {
        FollowTarget();

        RotateBladeTowardsTarget();
        // ����С�������λ
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