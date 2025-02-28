using UnityEngine;
using System;
using Sirenix.OdinInspector;
using MemoryPack;
using NaughtyAttributes;
using UltEvents;

public class DamageSender : MonoBehaviour, IDamageSender
{
    public IWeapon _Weapon;
    public bool isDamageModeEnabled = true; // 默认开启伤害模式
    public Collider2D _collider;
    public int damageCount = 0; // 造成的伤害次数
    public float CurrentDamageInterval = 1f; // 当前距离上次造成伤害的间隔时间，单位为秒

    private float lastDamageTime = 0f; // 记录上一次造成伤害的时间
    public UltEvent<IReceiveDamage> onDamage; // 伤害事件

    public void Awake()
    {
        InitializeComponents();
    }

    public virtual bool IsDamageModeEnabled
    {
        get => isDamageModeEnabled;
        set
        {
            _collider.enabled = value;
            isDamageModeEnabled = value;
        }
    }

    public virtual Collider2D Collider
    {
        get => _collider;
        set
        {
            if (_collider == null)
                _collider = GetComponent<Collider2D>();
            _collider = value;
        }
    }

    public virtual Damage DamageValue
    {
        get => _Weapon.WeaponDamage;
        set => _Weapon.WeaponDamage = value;
    }

    public float MinDamageInterval
    {
        get => _Weapon.MinDamageInterval;
        set => _Weapon.MinDamageInterval = value;
    }

    public virtual void InitializeComponents()
    {
        _Weapon = GetComponentInParent<IWeapon>();
        Collider = GetComponent<Collider2D>();
    }

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Damager_CheckerPoint damagePoint = collider.GetComponentInChildren<Damager_CheckerPoint>();
        if (collider.GetComponentInParent<IReceiveDamage>() != null)
        {
            onDamage.Invoke(collider.GetComponentInParent<IReceiveDamage>());
        }
    



        if (damagePoint == null)
        {
            Debug.Log("没有找到有效的检查点");
            return;
        }

        if (Time.time - lastDamageTime < MinDamageInterval)
        {
            Debug.Log("伤害间隔时间未到，无法施加伤害");
            return;
        }

        if (damageCount >= 1)
        {
            Debug.Log("已经造成过伤害，无法再次施加伤害");
            return;
        }

        if (!IsDamageModeEnabled)
        {
            Debug.Log("伤害模式未启用，无法施加伤害");
            return;
        }

        damagePoint.SetDamageValue_Point(DamageValue);
        damageCount++;
        lastDamageTime = Time.time;
       
    }
}

[System.Serializable]
[MemoryPackable]
public partial class Damage
{
    [Header("伤害类型设置")]
    [Tooltip("穿刺伤害")]
    public float piercing;
    [Tooltip("斩击伤害")]
    public float slashing;
    [Tooltip("钝击伤害")]
    public float blunt;
    [Tooltip("魔法伤害")]
    public float magic;

    public Damage(float piercing, float slashing, float blunt, float magic)
    {
        this.piercing = piercing;
        this.slashing = slashing;
        this.blunt = blunt;
        this.magic = magic;
    }

    public float DamageValue_F()
    {
        return piercing + slashing + blunt + magic;
    }
}
