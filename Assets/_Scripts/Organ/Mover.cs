using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Mover �����ڴ�����Ϸ������ƶ��߼���
/// </summary>
public class Mover : Organ, IFunction_Move
#region �ֶκ�����
{
    [Header("�ƶ�����")]
    public float _moveSpeed = 5f; // �ƶ��ٶ�
    public float _defaultMoveSpeed = 5f; // Ĭ���ƶ��ٶ�
    public float slowDownSpeed = 5f; // �ٶ�˥������
    public float endSpeed = 0.1f; // ֹͣʱ���ٶ�
    private Vector2 _lastDirection = Vector2.zero; // �ϴ��ƶ��ķ���
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

    #region �ֵ����

    private void RecalculateMoveSpeed()
    {
        // ���Ĭ���ٶ��Ƿ���Ч
        if (_defaultMoveSpeed <= 0)
        {
            Debug.Log($"��Ч��Ĭ���ٶ�: {_defaultMoveSpeed}");
            return;
        }

        // �����ٶ�
        MoveSpeed = _defaultMoveSpeed;

        // ʹ���б�洢�ֵ��е�Ԫ�أ����������ȼ�����
        var changeList = new List<(string, ValueChangeType, float, float)>();

        // ���ֵ�����ת��Ϊ�б�������ȼ���Ϣ��Item3��
        foreach (var item in SpeedChangeDict)
        {
            changeList.Add((item.Key.Item1, item.Key.Item2, item.Key.Item3, item.Value));
        }

        // �������ȼ���Item3�������б����ȼ�С���ȴ���
        changeList.Sort((a, b) => a.Item3.CompareTo(b.Item3));

        // ��������˳��ִ�мӷ��ͳ˷�
        foreach (var item in changeList)
        {
            var changeType = item.Item2;
            var value = item.Item4;

            // ����ӷ��仯
            if (changeType == ValueChangeType.Add)
            {
                if (value < 0)
                {
                    Debug.Log($"��Ч�ļӷ��仯ֵ: {value}");
                    return;
                }
                MoveSpeed += value;
            }
            // ����˷��仯
            else if (changeType == ValueChangeType.Multiply)
            {
                if (value <= 0)
                {
                    Debug.Log($"��Ч�ĳ˷��仯ֵ: {value}");
                    return;
                }
                MoveSpeed *= value;
            }
        }

        Debug.Log($"��ǰ�ٶ�: {MoveSpeed}");
    }



    /*    public void AddSpeedChange(string changeSign, ValueChangeType changeType, float Value)
        {
            //�ȼ���Ƿ��Ѿ�����
            if (SpeedChangeDict.ContainsKey(changeSign) || SpeedChangeDict_Multiply.ContainsKey(changeSign))
            {
                Debug.LogWarning("�Ѿ�������ͬ�ĸı����ͣ�");
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
        //�ȼ���Ƿ��Ѿ�����
        if (SpeedChangeDict.ContainsKey(Sign_Type_Priority))
        {
            //Debug.LogWarning("�Ѿ�������ͬ�ĸı����ͣ�");
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
        AddSpeedChange(("����" ,ValueChangeType.Add,0), +_defaultMoveSpeed);
    }

    /// <summary>
    /// �����ƶ��������ٶȡ�
    /// </summary>
    /// <param name="direction">�ƶ�����</param>
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
    /// ��������Э�̡�
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
    /// �����߼���
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
