using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tree_Apple : Trees,IReceiveDamage
{
    public AppleTreeData _data;
    public DamageReceiver _damageReceiver;

    public override ItemData Item_Data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = (AppleTreeData)value;
        }
    }

    public Defense DefenseValue
    {
        get
        {
            return _data._entityData.defense;
        }

        set
        {
            _data._entityData.defense = value;
        }
    }
    public Hp Hp
    {
        get
        {
            return _data._entityData.hp;
        }

        set
        {
            _data._entityData.hp = value;
        }
    }
    void OnEnable()
    {
        _damageReceiver = GetComponentInChildren<DamageReceiver>();
    }
    void Start()
    {
        _damageReceiver.OnDeath += Die;
    }

    void FixedUpdate()
    {

    }
    void Update()
    {

    }

    public override void Die()
    {
       
            base.DropItemByList(_data.DropItemAddressAndNum);
        

        // 打印死亡信息
        Debug.Log("苹果树死亡");
        // 销毁自身
        base.Die();
    }

/*    public override Item_Data GetData()
    {
        Debug.Log($"获取{name}数据中....");
        
        return _data;
    }   
    public override void SetData(Item_Data data)
    {
        Debug.Log($"设置{name}数据中....");
        _data = (AppleTreeData)data;
    }*/

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    public void ReceiveDamage(float damage)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update


    // Update is called once per frame

}
[MemoryPackable]
[System.Serializable]
public partial class AppleTreeData : ItemData, IEntity_Data
{
    [FoldoutGroup("苹果树数据", expanded: true)]
    // 掉落物品地址
    public List<DropItem> DropItemAddressAndNum = new List<DropItem>
    {
        new DropItem("Apple", 1)
    };
  
    [ShowInInspector,FoldoutGroup("苹果树数据", expanded: true)]
    public EntityData _entityData { get; set; }

}

public interface IEntity_Data
{
    EntityData _entityData { get; set; }
}
[MemoryPackable]
[System.Serializable]
public partial class DropItem
{
    [Title("掉落物品")]
    public string itemName;
    public int amount;
    [MemoryPackConstructor]
    public DropItem(string itemName,int amount)
    {
        this.itemName = itemName;
        this.amount = amount;
    }
}
