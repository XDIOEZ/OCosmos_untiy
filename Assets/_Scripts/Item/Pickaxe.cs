using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : Weapon, IWeapon
{

    public PickaxeToolData stonePicaxeData;

    public DamageSender damageSender;
    public override ItemData Item_Data
    {
        get
        {
            return stonePicaxeData;
        }

        set
        {
            stonePicaxeData = (PickaxeToolData)value;
        }
    }

    public Damage WeaponDamage
    {
        get
        {
            return stonePicaxeData._damage;
        }

        set
        {
            stonePicaxeData._damage = value;
        }
    }

    public float MinDamageInterval
    {
        get
        {
            return stonePicaxeData.MaxDamageInterval;
        }

        set
        {
            stonePicaxeData.MaxDamageInterval = value;
        }
    }

    public new void Start()
    {
        damageSender = GetComponentInChildren<DamageSender>();
        damageSender.onDamage += Attack;
        base.Start();
    }
    public new void OnDisable()
    {
        base.OnDisable();
    }

    public  void Attack(IReceiveDamage BeAttacker)
    {
        BeAttacker.ReceiveDamage(WeaponDamage.DamageValue_F());

    }
    public override void StartAttack()
    {
        damageSender.IsDamageModeEnabled = true;
        damageSender.damageCount = 0;
        // Debug.Log("Stone Axe Attack");
    }

    public override void StayAttack()
    {
        // Debug.Log("Stone Axe Stay Attack");
    }

    public override void StopAttack()
    {
        damageSender.IsDamageModeEnabled = false;
        damageSender.damageCount = 1;
        // Debug.Log("Stone Axe Stop Attack");
    }

}
[System.Serializable]
[MemoryPackable]
public partial class PickaxeToolData : WeaponData
{

}
