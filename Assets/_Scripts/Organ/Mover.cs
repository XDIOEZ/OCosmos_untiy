using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Mover 类用于处理游戏对象的移动逻辑。
/// </summary>
public class Mover : Organ, IFunction_Move
#region 字段和属性
{
    [Header("移动设置")]
    public float _moveSpeed = 5f; // 移动速度
    public float _defaultMoveSpeed = 5f; // 默认移动速度
    public float slowDownSpeed = 5f; // 速度衰减速率
    public float endSpeed = 0.1f; // 停止时的速度
    private Vector2 _lastDirection = Vector2.zero; // 上次移动的方向
    private Coroutine _slowDownCoroutine;
    public bool IsMoving;
    public UltEvent OnMoveEnd;
    public UltEvent OnMoveStay;
    public UltEvent OnMoveStart;
[ShowInInspector]
    public Dictionary<(string, ValueChangeType,float), float> SpeedChangeDict = new Dictionary<(string,ValueChangeType,float), float>();
/*    private Dictionary<string, float> speedChangeDict_Multiply = new Dictionary<string, float>();*/

    private Rigidbody2D _rb;
    public Rigidbody2D Rb
    {
        get => _rb ??= XDTool.GetComponentInChildrenAndParent<Rigidbody2D>(gameObject);
        private set => _rb = value;
    }

    public float Speed
    {
        get => Rb ? Rb.velocity.magnitude : 0f;
        set => Rb.velocity = Rb.velocity.normalized * value;
    }
    public float MoveSpeed
    {
        get
        {
            return _moveSpeed;
        }
        set
        {
            _moveSpeed = value;
        }
    }
    #endregion

    #region 字典控制

    private void RecalculateMoveSpeed()
    {
        // 检查默认速度是否有效
        if (_defaultMoveSpeed <= 0)
        {
            Debug.Log($"无效的默认速度: {_defaultMoveSpeed}");
            return;
        }

        // 重置速度
        MoveSpeed = _defaultMoveSpeed;

        // 使用列表存储字典中的元素，并根据优先级排序
        var changeList = new List<(string, ValueChangeType, float, float)>();

        // 将字典内容转化为列表，添加优先级信息（Item3）
        foreach (var item in SpeedChangeDict)
        {
            changeList.Add((item.Key.Item1, item.Key.Item2, item.Key.Item3, item.Value));
        }

        // 根据优先级（Item3）排序列表，优先级小的先处理
        changeList.Sort((a, b) => a.Item3.CompareTo(b.Item3));

        // 按排序后的顺序执行加法和乘法
        foreach (var item in changeList)
        {
            var changeType = item.Item2;
            var value = item.Item4;

            // 处理加法变化
            if (changeType == ValueChangeType.Add)
            {
                if (value < 0)
                {
                    Debug.Log($"无效的加法变化值: {value}");
                    return;
                }
                MoveSpeed += value;
            }
            // 处理乘法变化
            else if (changeType == ValueChangeType.Multiply)
            {
                if (value <= 0)
                {
                    Debug.Log($"无效的乘法变化值: {value}");
                    return;
                }
                MoveSpeed *= value;
            }
        }

        Debug.Log($"当前速度: {MoveSpeed}");
    }



    /*    public void AddSpeedChange(string changeSign, ValueChangeType changeType, float Value)
        {
            //先检测是否已经存在
            if (SpeedChangeDict.ContainsKey(changeSign) || SpeedChangeDict_Multiply.ContainsKey(changeSign))
            {
                Debug.LogWarning("已经存在相同的改变类型！");
                return;
            }
            if (changeType == ValueChangeType.Add)
            {
                *//* MoveSpeed += Value;*//*
                SpeedChangeDict[changeSign] = Value;
            }

            if (changeType == ValueChangeType.Multiply)
            {
                *//* MoveSpeed *= Value;*//*
                SpeedChangeDict_Multiply[changeSign] = Value;
            }
            RecalculateMoveSpeed();
        }*/
    public void AddSpeedChange((string, ValueChangeType, float) Sign_Type_Priority, float Value)
    {
        //先检测是否已经存在
        if (SpeedChangeDict.ContainsKey(Sign_Type_Priority))
        {
            //Debug.LogWarning("已经存在相同的改变类型！");
            return;
        }
        if (Sign_Type_Priority.Item2 == ValueChangeType.Add)
        {
           
            SpeedChangeDict[Sign_Type_Priority] = Value;
        }
        if (Sign_Type_Priority.Item2 == ValueChangeType.Multiply)
        {
            SpeedChangeDict[Sign_Type_Priority] = Value;
        }
        RecalculateMoveSpeed();
    }

public void RemoveSpeedChange((string, ValueChangeType, float) Sign_Type_Priority)
{

    if (SpeedChangeDict.ContainsKey(Sign_Type_Priority))
    {
        /*MoveSpeed -= speedChangeDict_Add[changeSign];*/
        SpeedChangeDict.Remove(Sign_Type_Priority);
    }
    RecalculateMoveSpeed();
}


    #endregion


    /*    private IStamina _stamina;
        [ShowInInspector]
        public IStamina StaminaManager => _stamina ??= XDTool.GetComponentInChildrenAndParent<IStamina>(gameObject);
    */
    private void Start()
    {
        if (Rb == null)
        {
            Rb = GetComponentInParent<Rigidbody2D>();
        }
        AddSpeedChange(("测试" ,ValueChangeType.Add,0), +_defaultMoveSpeed);
    }

    /// <summary>
    /// 控制移动方向与速度。
    /// </summary>
    /// <param name="direction">移动方向</param>
    public void Move(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            Rb.velocity = Vector2.zero;
            if (IsMoving)
            {
            IsMoving = false;
            OnMoveEnd?.Invoke();
            }
           
            return;
        }

        if(IsMoving == false)
        {
            IsMoving = true;
            OnMoveStart?.Invoke();
        }

        if (direction != _lastDirection)
        {
            direction.Normalize();
            _lastDirection = direction;
        }

        Vector2 adjustedVelocity = direction * MoveSpeed;
        Rb.velocity = adjustedVelocity;
    }

    /// <summary>
    /// 启动减速协程。
    /// </summary>
    private void StartSlowDownRoutine()
    {
        if (_slowDownCoroutine != null)
        {
            StopCoroutine(_slowDownCoroutine);
        }
        _slowDownCoroutine = StartCoroutine(SlowDownRoutine());
    }

    /// <summary>
    /// 减速逻辑。
    /// </summary>
    private IEnumerator SlowDownRoutine()
    {
        while (Rb.velocity.magnitude > endSpeed)
        {
            Rb.velocity = Vector2.Lerp(Rb.velocity, Vector2.zero, slowDownSpeed * Time.deltaTime);
            yield return null;
        }
        Rb.velocity = Vector2.zero;
    }

    private void OnDestroy()
    {
        _rb = null;
    }


}
public enum ValueChangeType
{
    Add,
    Multiply,
}
