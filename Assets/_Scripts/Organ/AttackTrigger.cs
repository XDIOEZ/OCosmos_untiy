using Sirenix.OdinInspector;
using System.Collections;
using UltEvents;
using UnityEditor;
using UnityEngine;

public class AttackTrigger : MonoBehaviour, IFunction_TriggerAttack
{
    #region 事件

    private UltEvent onStartAttack; // 角色攻击时触发的事件
    private UltEvent onStayAttack;
    private UltEvent onEndAttack;
    public UltEvent OnStartAttack { get => onStartAttack; set => onStartAttack = value; }
    public UltEvent OnStayAttack { get => onStayAttack; set => onStayAttack = value; }
    public UltEvent OnEndAttack { get => onEndAttack; set => onEndAttack = value; }
    #endregion

    #region 武器数据
    [ShowInInspector]
    public WeaponData _weaponAnimationData; // 武器数据

    public Item _item; // 装备的物品

    // 保存武器数据的引用，后续所有数值都直接通过此数据获取
    public WeaponData Weapon_Data
    {
        get { return _weaponAnimationData; }
        set { _weaponAnimationData = value; }
    }

    public GameObject Weapon_GameObject; // 武器对象

    // 默认数值（当Weapon_Data为空时使用）
    [Title("默认最大攻击距离(攻击状态时武器的最大移动距离)")]
    public float MaxAttackDistance_Default = 1f;
    [Title("默认攻击速度")]
    public float AttackSpeed_Default = 20;
    [Title("默认物品返回速度")]
    public float ReturnSpeed_Default = 5f;
    [Title("默认每秒精力消耗")]
    public float StaminaCost_Default = 10f;

    // 直接从Weapon_Data获取数值，如果为空则返回默认值
    public float EffectiveMaxAttackDistance => Weapon_Data != null ? Weapon_Data.MaxAttackDistance : MaxAttackDistance_Default;
    public float EffectiveAttackSpeed => Weapon_Data != null ? Weapon_Data.AttackSpeed : AttackSpeed_Default;
    public float EffectiveReturnSpeed => Weapon_Data != null ? Weapon_Data.ReturnSpeed : ReturnSpeed_Default;
    public float EffectiveStaminaCost => Weapon_Data != null ? Weapon_Data.StaminaCost : StaminaCost_Default;
    #endregion

    #region 状态参数
    public Vector2 StartPosition; // 保存初始位置
    public Vector3 MouseTarget;
    public bool CanAttack;
    public AttackState CurrentState = AttackState.Idle; // 当前状态
    private Coroutine returnCoroutine;
    #endregion

    #region (注册攻击事件)Unity生命周期
    private void OnEnable()
    {
        OnStartAttack += StartAttack; // 注册攻击开始事件
        OnStayAttack += StayAttack;
        OnEndAttack += StopAttack;
    }

    private void Start()
    {
        StaminaManager staminaManager = transform.parent.GetComponentInChildren<StaminaManager>();
        if (staminaManager != null)
        {
            // 使用EffectiveStaminaCost保证武器数据更新时精力消耗值也跟着更新
            OnStartAttack += () => staminaManager.StartReduceStamina(EffectiveStaminaCost, "AttackTrigger");
            OnEndAttack += () => staminaManager.StopReduceStamina("AttackTrigger");
            staminaManager.OnStaminaChanged += SetCanAttack; // 注册精力值变化事件
        }
    }

    private void OnDisable()
    {
        OnStartAttack.Clear();
        OnStayAttack.Clear();
        OnEndAttack.Clear();
    }
    #endregion

    #region (设置是否可以攻击, 触发攻击)外部接口

    public void SetCanAttack(float Stamina)
    {
        if (Stamina >= 20)
        {
            CanAttack = true;
        }
        else if (Stamina <= 0)
        {
            CanAttack = false;
        }
    }
    public void TriggerAttack(KeyState keyState, Vector3 Target)
    {
        if (keyState == KeyState.Start)
        {
            if (CanAttack && CurrentState == AttackState.Idle)
            {
                MouseTarget = Target; // 同步鼠标目标位置
                OnStartAttack.Invoke(); // 触发攻击开始事件
            }
        }
        else if (keyState == KeyState.End || CanAttack == false)
        {
            if (CurrentState == AttackState.Attacking)
            {
                MouseTarget = Target; // 同步鼠标目标位置
                OnEndAttack.Invoke(); // 触发攻击结束事件
            }
        }
        else if (keyState == KeyState.Hold)
        {
            if (CanAttack && CurrentState == AttackState.Attacking)
            {
                MouseTarget = Target; // 同步鼠标目标位置
                OnStayAttack.Invoke(); // 触发攻击持续事件
            }
        }
    }

    // 装备武器时直接获取武器的数据
    public void SetWeapon(Item Item_)
    {
        Debug.Log("设置武器数据");
        if (Item_ is Weapon)
        {
            Weapon weapon_ = (Weapon)Item_;
            Weapon_GameObject = weapon_.gameObject;
            OnStartAttack += weapon_.StartAttack;
            OnEndAttack += weapon_.StopAttack;
            OnStayAttack += weapon_.StayAttack;
            Weapon_Data = weapon_.Item_Data as WeaponData;
        }
        else
        {
            Weapon_Data = null;
        }
    }

    public void RemoveWeapon(Item Item_)
    {
        if (Item_ is Weapon)
        {
            Weapon weapon_ = (Weapon)Item_;
            OnEndAttack.Invoke();
            OnStartAttack -= weapon_.StartAttack;
            OnEndAttack -= weapon_.StopAttack;
            OnStayAttack -= weapon_.StayAttack;
        }
        Weapon_GameObject = null;
        Weapon_Data = null;
    }

    #endregion

    #region (刺击、返回、检查攻击状态)私有方法
    public void PerformStab(Vector2 startTarget, float speed, float maxDistance)
    {
        // 坐标系转换：如果有父对象，则将鼠标目标转换为本地坐标
        Vector2 mouseTargetLocal = transform.parent != null ?
            (Vector2)transform.parent.InverseTransformPoint(MouseTarget) :
            (Vector2)MouseTarget;

        Vector2 currentLocalPos = (Vector2)transform.localPosition;

        // 计算从起始点到目标点的向量及其长度
        Vector2 toTarget = mouseTargetLocal - startTarget;
        float targetDistance = toTarget.magnitude;

        // 若目标超出最大范围，则先对目标进行限制
        if (targetDistance > maxDistance)
        {
            mouseTargetLocal = startTarget + toTarget.normalized * maxDistance;
        }

        // 计算移动方向
        Vector2 direction = (mouseTargetLocal - currentLocalPos).normalized;
        // 本帧移动的距离
        float distanceToMove = speed * Time.deltaTime;
        // 计算新的位置
        Vector2 newPosition = currentLocalPos + direction * distanceToMove;

        // 判断剩余距离，若过近则直接设置为目标位置（并确保不超出最大范围）
        float remainingDistance = Vector2.Distance(currentLocalPos, mouseTargetLocal);
        if (remainingDistance < 0.1f)
        {
            float finalDistance = Vector2.Distance(startTarget, mouseTargetLocal);
            newPosition = finalDistance > maxDistance ?
                startTarget + (mouseTargetLocal - startTarget).normalized * maxDistance :
                mouseTargetLocal;
        }
        else
        {
            // 常规移动时检查是否超过最大范围
            float currentDistance = Vector2.Distance(startTarget, newPosition);
            if (currentDistance > maxDistance)
            {
                newPosition = startTarget + (newPosition - startTarget).normalized * maxDistance;
            }
        }

        // 更新本地位置
        transform.localPosition = newPosition;

        // 调试绘制，从起始点到当前位置的连线（转换为世界坐标）
        Debug.DrawLine(
            transform.parent.TransformPoint(startTarget),
            transform.parent.TransformPoint(newPosition),
            Color.red
        );
    }

    private IEnumerator ReturnToStartPositionCoroutine(Vector2 startTarget, float backSpeed)
    {

        while (Vector2.Distance(startTarget, transform.localPosition) >= 0.1f)
        {
            float distanceToMoveBack = backSpeed * Time.deltaTime;
            Vector2 directionBack = (startTarget - (Vector2)transform.localPosition).normalized;
            transform.localPosition = (Vector2)transform.localPosition + directionBack * distanceToMoveBack;
            yield return null;
        }

        transform.localPosition = startTarget;
        CurrentState = AttackState.Idle;
    }

    public void UseItem()
    {
    }

    public void StartReturningToStartPosition(Vector2 startTarget, float backSpeed)
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
        returnCoroutine = StartCoroutine(ReturnToStartPositionCoroutine(startTarget, backSpeed));
    }
    #endregion

    #region 程序动画状态
    public void StartAttack()
    {
        if(_item == null)
        {
            Debug.Log("没有装备武器尝试装备");
            //获取子对象中的Item
            _item = GetComponentInChildren<Item>();
        }

        CurrentState = AttackState.Attacking;
        StartPosition = transform.localPosition;
    }

    public void StayAttack()
    {
        // 直接使用EffectiveAttackSpeed和EffectiveMaxAttackDistance
        PerformStab(StartPosition, EffectiveAttackSpeed, EffectiveMaxAttackDistance);
    }

    public void StopAttack()
    {
        if (_item != null && _weaponAnimationData == null)
        {
            _item.Use();
        }
#if UNITY_EDITOR
        if (!gameObject.activeInHierarchy) return;
#endif
        CurrentState = AttackState.Returning;
        StartReturningToStartPosition(StartPosition, EffectiveReturnSpeed);
    }
    #endregion
}

public enum AttackState
{
    Idle,
    Attacking,
    Returning
}
