using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Weapon
{
    #region �ֶ� | Fields 
    public PrefabLauncher launcher;
    public GunData gunData; // ʹ��GunData��װ���� 
    #endregion

    #region �������� | Life Cycle 
    public new void Start()
    {
        launcher ??= GetComponent<PrefabLauncher>();

        base.Start();
    }
    new void OnEnable()
    {
        base.OnEnable();
    }
    new void OnDisable()
    {
        base.OnDisable();
    }
    #endregion

    #region �����߼� | Attack Logic 
    public override void StartAttack()
    {
        if (gunData.WeaponFireMode == FireMode.Single)
        {
            launcher.LaunchPrefab(transform.right, gunData.speed, transform.rotation);
        }
    }

    public override void StayAttack()
    {
        if (gunData.WeaponFireMode == FireMode.Automatic)
        {
            gunData.timeSinceLastFire += Time.deltaTime;
            if (gunData.timeSinceLastFire >= 1f / gunData.fireRate)
            {
                launcher.LaunchPrefab(transform.right, gunData.speed, transform.rotation);
                gunData.timeSinceLastFire = 0f;
            }
        }
    }

    public override void StopAttack()
    {
        // ʵ��ֹͣ�������߼���������Ҫ��
    }
    #endregion

    #region ���ݽӿ� | Data Management 
/*    public override Item_Data GetData()
    {
        Debug.Log("��ȡgunData��....");
        return gunData;
    }*/
    #endregion 
}
[MemoryPackable]
[System.Serializable]
public  partial class GunData : WeaponData
{
    public float speed = 50f;
    public float fireRate = 10f;
    public float timeSinceLastFire = 0f;
    public FireMode WeaponFireMode;
    public GunType gunType;
}

public enum FireMode
{
    Single,
    Automatic,
    SemiAuto
}
public enum GunType
{
    Pistol,
    Shotgun,
    SMG,
    Sniper,
    AutomaticRifle,
}