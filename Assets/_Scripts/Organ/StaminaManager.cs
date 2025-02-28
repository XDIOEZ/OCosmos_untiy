using Sirenix.OdinInspector;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class StaminaManager : Organ, IStamina
{
    #region �����ֶ�

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

    #region ����

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

    #region Unity�������ڷ���

    private void OnEnable()
    {
        OnStaminaReduce += StartReduceStamina;
        OnStaminaRecovery += StartRecoverStamina;

        OnEnterZeroStamina += () => { Debug.Log("��ɫ����ֵ�ľ�����ʼ���ľ�������"); };
        OnStayZeroStamina += () => { Debug.Log("��ɫ����ֵ�ľ����������ľ�������"); };
        OnExitZeroStamina += () => { Debug.Log("��ɫ����ֵ��ʼ�ָ���"); };
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

    #region ��������

    [Button("��ʼ��������")]
    public void StartReduceStamina(float reductionSpeed, string reductionType)
    {
        if (reductionType == null)
        {//���reductionTypeΪ�գ���ʹ��ʱ�����Ϊ��ֵ�Եļ�
            reductionType = Time.time.ToString();
        }
        staminaReductionRates[reductionType] = reductionSpeed;
        allReduceSpeed += reductionSpeed;
    }

    [Button("ֹͣ��������")]
    public void StopReduceStamina(string reductionType)
    {
        //����ֵ��ֵ���Ƿ����
        if (!staminaReductionRates.ContainsKey(reductionType))
        { 
            Debug.Log("�ֵ��в����ڸü�ֵ�ԣ�");
            return;
        }
        allReduceSpeed -= staminaReductionRates[reductionType];
        //����ֵ��ֵ��
        staminaReductionRates.Remove(reductionType);
    }

    [Button("��ʼ�����ָ�")]
    public void StartRecoverStamina(float recoverySpeed, string recoveryType)
    {
        staminaRecoveryRates[recoveryType] = recoverySpeed;
        allRecoverySpeed += recoverySpeed;
    }

    [Button("ֹͣ�����ָ�")]
    public void StopRecoverStamina(string recoveryType)
    {
        allRecoverySpeed -= staminaRecoveryRates[recoveryType];
    }

    #endregion

    #region ˽�з���

    private float ClampStamina(float value)
    {
        return Mathf.Clamp(value, 0f, MaxStamina);
    }
#if UNITY_EDITOR
    [Button("��ӡ�����¼��ĳ�Ա��")]
    //��ӡ�����¼��ĳ�Ա��
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