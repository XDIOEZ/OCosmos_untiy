using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAxe : Weapon, IWeapon
{
    
    public AxeToolData stoneAxeData;

    public DamageSender damageSender;
    public override ItemData Item_Data
    {
        get
        {
            return stoneAxeData;
        }

        set
        {
            stoneAxeData = (AxeToolData)value;
        }
    }

    public Damage WeaponDamage
    {
        get
        {
            return stoneAxeData._damage;
        }

        set
        {
            stoneAxeData._damage = value;
        }
    }

    public float MinDamageInterval
    {
        get
        {
          return  stoneAxeData.MaxDamageInterval;
        }

        set
        {
            stoneAxeData.MaxDamageInterval = value;
        }
    }

    public new  void Start()
    {
        damageSender = GetComponentInChildren<DamageSender>();
        base.Start();
    }
    public new void OnDisable()
    {
        base.OnDisable();
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



    // Update is called once per frame
    void Update()
    {
        
    }
}


[System.Serializable]
[MemoryPackable]
public partial class AxeToolData : WeaponData
{

}
