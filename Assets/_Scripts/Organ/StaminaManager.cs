using Sirenix.OdinInspector;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class StaminaManager : Organ, IStamina
{
    #region 公开字段

    [ShowInInspector]
    public Dictionary<string, float> staminaReductionRates = new Dictionary<string, float>();
    [ShowInInspector]
    public Dictionary<string, float> staminaRecoveryRates = new Dictionary<string, float>();

    private bool isAtZero = false;
    public float maxStamina = 100f;
    public float DefaultStamina = 100f;
    public float currentStamina = 100f;

    public float allRecoverySpeed = 1f;
    public float allReduceSpeed = 0f;

    public UltEvent<float, string> onStaminaReduce = new UltEvent<float, string>();
    public UltEvent<float, string> onStaminaRecovery = new UltEvent<float, string>();
    public UltEvent OnEnterZeroStamina;
    public UltEvent OnStayZeroStamina;
    public UltEvent OnExitZeroStamina;
    public UltEvent<float> OnStaminaChanged;



    #endregion

    #region 属性

    public UltEvent<float, string> OnStaminaReduce { get => onStaminaReduce; set => onStaminaReduce = value; }
    public UltEvent<float, string> OnStaminaRecovery { get => onStaminaRecovery; set => onStaminaRecovery = value; }

    public float CurrentStamina
    {
        get => currentStamina;
        set
        {
            OnStaminaChanged?.Invoke(value);
            if (value <= 0)
            {
                if (!isAtZero)
                {
                    OnEnterZeroStamina.Invoke();
                    isAtZero = true;
                }
            }
            else
            {
                if (isAtZero)
                {
                    OnExitZeroStamina.Invoke();
                    isAtZero = false;
                }
            }
            currentStamina = ClampStamina(value);
        }
    }

    public float MaxStamina
    {
        get => maxStamina;
        set => maxStamina = value;
    }
    public bool IsAtZero
    {
        get
        {
            return isAtZero;
        }

        set
        {
            isAtZero = value;
        }
    }

    #endregion

    #region Unity生命周期方法

    private void OnEnable()
    {
        OnStaminaReduce += StartReduceStamina;
        OnStaminaRecovery += StartRecoverStamina;

        OnEnterZeroStamina += () => { Debug.Log("角色耐力值耗尽！开始消耗精力上限"); };
        OnStayZeroStamina += () => { Debug.Log("角色耐力值耗尽！持续消耗精力上限"); };
        OnExitZeroStamina += () => { Debug.Log("角色耐力值开始恢复！"); };
    }

    private void OnDisable()
    {
        OnStaminaReduce -= StartReduceStamina;
        OnStaminaRecovery -= StartRecoverStamina;
    }

    private void Update()
    {
        float ValueSpeed = (allRecoverySpeed  - allReduceSpeed) * Time.deltaTime;
        CurrentStamina += ValueSpeed;

        if (CurrentStamina <= 0)
        {
            OnStayZeroStamina.Invoke();
        }
    }

    #endregion

    #region 公开方法

    [Button("开始耐力消耗")]
    public void StartReduceStamina(float reductionSpeed, string reductionType)
    {
        if (reductionType == null)
        {//如果reductionType为空，则使用时间戳作为键值对的键
            reductionType = Time.time.ToString();
        }
        staminaReductionRates[reductionType] = reductionSpeed;
        allReduceSpeed += reductionSpeed;
    }

    [Button("停止耐力消耗")]
    public void StopReduceStamina(string reductionType)
    {
        //检测字典键值对是否存在
        if (!staminaReductionRates.ContainsKey(reductionType))
        { 
            Debug.Log("字典中不存在该键值对！");
            return;
        }
        allReduceSpeed -= staminaReductionRates[reductionType];
        //清除字典键值对
        staminaReductionRates.Remove(reductionType);
    }

    [Button("开始耐力恢复")]
    public void StartRecoverStamina(float recoverySpeed, string recoveryType)
    {
        staminaRecoveryRates[recoveryType] = recoverySpeed;
        allRecoverySpeed += recoverySpeed;
    }

    [Button("停止耐力恢复")]
    public void StopRecoverStamina(string recoveryType)
    {
        allRecoverySpeed -= staminaRecoveryRates[recoveryType];
    }

    #endregion

    #region 私有方法

    private float ClampStamina(float value)
    {
        return Mathf.Clamp(value, 0f, MaxStamina);
    }
#if UNITY_EDITOR
    [Button("打印订阅事件的成员们")]
    //打印订阅事件的成员们
    private void PrintEventMembers()
    {
        Debug.Log("OnStaminaReduce: " + (", ", OnStaminaReduce.ParameterTypes));
        Debug.Log("OnStaminaRecovery: " + string.Join(", ", OnStaminaRecovery.PersistentCallsList));
        Debug.Log("OnEnterZeroStamina: " + string.Join(", ", OnEnterZeroStamina.PersistentCallsList));
        Debug.Log("OnStayZeroStamina: " + string.Join(", ", OnStayZeroStamina.PersistentCallsList));
        Debug.Log("OnExitZeroStamina: " + string.Join(", ", OnExitZeroStamina.PersistentCallsList));
        Debug.Log("OnStaminaChanged: " + string.Join(", ", OnStaminaChanged.PersistentCallsList));
    }
#endif
    #endregion

}