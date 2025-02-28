using Sirenix.OdinInspector;
using System.Collections;
using UltEvents;
using UnityEditor;
using UnityEngine;

public class AttackTrigger : MonoBehaviour, IFunction_TriggerAttack
{
    #region �¼�

    private UltEvent onStartAttack; // ��ɫ����ʱ�������¼�
    private UltEvent onStayAttack;
    private UltEvent onEndAttack;
    public UltEvent OnStartAttack { get => onStartAttack; set => onStartAttack = value; }
    public UltEvent OnStayAttack { get => onStayAttack; set => onStayAttack = value; }
    public UltEvent OnEndAttack { get => onEndAttack; set => onEndAttack = value; }
    #endregion

    #region ��������
    [ShowInInspector]
    public WeaponData _weaponAnimationData; // ��������

    public Item _item; // װ������Ʒ

    // �����������ݵ����ã�����������ֵ��ֱ��ͨ�������ݻ�ȡ
    public WeaponData Weapon_Data
    {
        get { return _weaponAnimationData; }
        set { _weaponAnimationData = value; }
    }

    public GameObject Weapon_GameObject; // ��������

    // Ĭ����ֵ����Weapon_DataΪ��ʱʹ�ã�
    [Title("Ĭ����󹥻�����(����״̬ʱ����������ƶ�����)")]
    public float MaxAttackDistance_Default = 1f;
    [Title("Ĭ�Ϲ����ٶ�")]
    public float AttackSpeed_Default = 20;
    [Title("Ĭ����Ʒ�����ٶ�")]
    public float ReturnSpeed_Default = 5f;
    [Title("Ĭ��ÿ�뾫������")]
    public float StaminaCost_Default = 10f;

    // ֱ�Ӵ�Weapon_Data��ȡ��ֵ�����Ϊ���򷵻�Ĭ��ֵ
    public float EffectiveMaxAttackDistance => Weapon_Data != null ? Weapon_Data.MaxAttackDistance : MaxAttackDistance_Default;
    public float EffectiveAttackSpeed => Weapon_Data != null ? Weapon_Data.AttackSpeed : AttackSpeed_Default;
    public float EffectiveReturnSpeed => Weapon_Data != null ? Weapon_Data.ReturnSpeed : ReturnSpeed_Default;
    public float EffectiveStaminaCost => Weapon_Data != null ? Weapon_Data.StaminaCost : StaminaCost_Default;
    #endregion

    #region ״̬����
    public Vector2 StartPosition; // �����ʼλ��
    public Vector3 MouseTarget;
    public bool CanAttack;
    public AttackState CurrentState = AttackState.Idle; // ��ǰ״̬
    private Coroutine returnCoroutine;
    #endregion

    #region (ע�ṥ���¼�)Unity��������
    private void OnEnable()
    {
        OnStartAttack += StartAttack; // ע�ṥ����ʼ�¼�
        OnStayAttack += StayAttack;
        OnEndAttack += StopAttack;
    }

    private void Start()
    {
        StaminaManager staminaManager = transform.parent.GetComponentInChildren<StaminaManager>();
        if (staminaManager != null)
        {
            // ʹ��EffectiveStaminaCost��֤�������ݸ���ʱ��������ֵҲ���Ÿ���
            OnStartAttack += () => staminaManager.StartReduceStamina(EffectiveStaminaCost, "AttackTrigger");
            OnEndAttack += () => staminaManager.StopReduceStamina("AttackTrigger");
            staminaManager.OnStaminaChanged += SetCanAttack; // ע�ᾫ��ֵ�仯�¼�
        }
    }

    private void OnDisable()
    {
        OnStartAttack.Clear();
        OnStayAttack.Clear();
        OnEndAttack.Clear();
    }
    #endregion

    #region (�����Ƿ���Թ���, ��������)�ⲿ�ӿ�

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
                MouseTarget = Target; // ͬ�����Ŀ��λ��
                OnStartAttack.Invoke(); // ����������ʼ�¼�
            }
        }
        else if (keyState == KeyState.End || CanAttack == false)
        {
            if (CurrentState == AttackState.Attacking)
            {
                MouseTarget = Target; // ͬ�����Ŀ��λ��
                OnEndAttack.Invoke(); // �������������¼�
            }
        }
        else if (keyState == KeyState.Hold)
        {
            if (CanAttack && CurrentState == AttackState.Attacking)
            {
                MouseTarget = Target; // ͬ�����Ŀ��λ��
                OnStayAttack.Invoke(); // �������������¼�
            }
        }
    }

    // װ������ʱֱ�ӻ�ȡ����������
    public void SetWeapon(Item Item_)
    {
        Debug.Log("������������");
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

    #region (�̻������ء���鹥��״̬)˽�з���
    public void PerformStab(Vector2 startTarget, float speed, float maxDistance)
    {
        // ����ϵת��������и����������Ŀ��ת��Ϊ��������
        Vector2 mouseTargetLocal = transform.parent != null ?
            (Vector2)transform.parent.InverseTransformPoint(MouseTarget) :
            (Vector2)MouseTarget;

        Vector2 currentLocalPos = (Vector2)transform.localPosition;

        // �������ʼ�㵽Ŀ�����������䳤��
        Vector2 toTarget = mouseTargetLocal - startTarget;
        float targetDistance = toTarget.magnitude;

        // ��Ŀ�곬�����Χ�����ȶ�Ŀ���������
        if (targetDistance > maxDistance)
        {
            mouseTargetLocal = startTarget + toTarget.normalized * maxDistance;
        }

        // �����ƶ�����
        Vector2 direction = (mouseTargetLocal - currentLocalPos).normalized;
        // ��֡�ƶ��ľ���
        float distanceToMove = speed * Time.deltaTime;
        // �����µ�λ��
        Vector2 newPosition = currentLocalPos + direction * distanceToMove;

        // �ж�ʣ����룬��������ֱ������ΪĿ��λ�ã���ȷ�����������Χ��
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
            // �����ƶ�ʱ����Ƿ񳬹����Χ
            float currentDistance = Vector2.Distance(startTarget, newPosition);
            if (currentDistance > maxDistance)
            {
                newPosition = startTarget + (newPosition - startTarget).normalized * maxDistance;
            }
        }

        // ���±���λ��
        transform.localPosition = newPosition;

        // ���Ի��ƣ�����ʼ�㵽��ǰλ�õ����ߣ�ת��Ϊ�������꣩
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

    #region ���򶯻�״̬
    public void StartAttack()
    {
        if(_item == null)
        {
            Debug.Log("û��װ����������װ��");
            //��ȡ�Ӷ����е�Item
            _item = GetComponentInChildren<Item>();
        }

        CurrentState = AttackState.Attacking;
        StartPosition = transform.localPosition;
    }

    public void StayAttack()
    {
        // ֱ��ʹ��EffectiveAttackSpeed��EffectiveMaxAttackDistance
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
