using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Item,IWeapon
{
    public IDamageSender damageSender;
    public Rigidbody2D rb2d;
    public AmmoData ammoData;
    [Header("射线检测排除")]
    public LayerMask layerMask;

    public void Start()
    {
        DamageSender = GetComponentInChildren<IDamageSender>();
                rb2d = GetComponent<Rigidbody2D>();
        previousPosition = transform.position;
    }
    public Vector2 currentPosition;//当前帧位置
    public Vector2 previousPosition;//上一帧位置

    public void FixedUpdate()
    {
        //子弹当前帧位置
         currentPosition = transform.position;
        //子弹当前帧方向
        Vector2 direction = currentPosition - previousPosition;
        //子弹一帧移动距离
        float distance = direction.magnitude;

        if (distance > 0)
        {
            Debug.DrawRay(previousPosition, direction*2, Color.red);

            //避免Ammo图层物体阻挡射线
            RaycastHit2D _hit = Physics2D.Raycast(previousPosition, direction , distance*2, layerMask);

          

          
            

            if (_hit.collider != null)
            {
                // 在此处添加您的碰撞处理逻辑
                damageSender.OnTriggerEnter2D(_hit.collider);
                Debug.Log("Ammo hit " + _hit.collider.name);
            }
            //速度小于0.5时销毁
            if (rb2d.velocity.magnitude < 0.5f)
            {
                Destroy(gameObject);
            }
            previousPosition = currentPosition;
        }

    }

    public IDamageSender DamageSender
    {
        get
        {
            if (damageSender == null)
            {
                damageSender = GetComponentInChildren<IDamageSender>();
            }
            return damageSender;
        }
        set
        {
            damageSender = value;
        }
    }

    public override ItemData Item_Data
    {
        get
        {
            return ammoData;
        }

        set
        {
            ammoData = (AmmoData)value;
        }
    }
    public Damage WeaponDamage
    {
        get
        {
            return ammoData.damage;
        }

        set
        {
            ammoData.damage = value;
        }
    }

    public float MinDamageInterval 
    {
        get
        {
            return ammoData.MinDamageInterval;
        }

        set
        {
            ammoData.MinDamageInterval = value;
        }
    }

    public override void Use()
    {
        DamageSender.IsDamageModeEnabled = true;
    }
}
/*
    public override Item_Data GetData()
    {
        Debug.LogWarning("Ammo.GetData() is not implemented");
        throw new System.NotImplementedException();
    }

    public override void SetData(Item_Data data)
    {
        Debug.LogWarning("Ammo.GetData() is not implemented");
        throw new System.NotImplementedException();
    }*/
