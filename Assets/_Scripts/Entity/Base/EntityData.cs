using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[MemoryPackable]
public  partial class EntityData
{
    #region 私有字段
    [Title("实体数据")]
    // 当前值字段（初始化为默认值）
    // 体力
    public float stamina = 100;
    // 生命
    public Hp hp  = new Hp(100);
    //防御力     
    public Defense defense ;
    //速度
    public float speed = 8;
    //力量
    public float Power = 10;
    // 最大值字段
    public float maxStamina = 100;
    public Hp maxHP = new Hp(100);
    public float maxDefense = 10;
    public float maxSpeed = 8;
    public float maxPower = 10;
    /// <summary>
    /// 角色数据
    /// </summary>
    /// <param name="stamina">精力</param>
    /// <param name="hp">血量</param>
    /// <param name="defense">防御</param>
    /// <param name="speed">移动速度</param>
    /// <param name="hungerValue">饥饿度</param>
    /// <param name="power">力量</param>
/*    public Data(float stamina, float hp, float defense, float speed, float Food_Energy, float Power)
    {
        maxStamina = stamina;
        maxHP = hp;
        maxDefense = defense;
        maxSpeed = speed;
        maxEnergy = Food_Energy;
        maxPower = Power;

        ResetValuesToMax();
    }*/

    public void ResetValuesToMax()
    {
        stamina = maxStamina;
        hp = maxHP;
        speed = maxSpeed;
        Power = maxPower;
    }
/*    // 默认值字段
    private float defaultStamina = 100;
    private float defaultHP = 100;
    private float defaultDefense = 10;
    private float defaultSpeed = 8;
    private float defaultHungerValue = 100;
    private float defaultPower = 10;
*/
    #endregion
    /*
        #region 默认值

        [ShowInInspector, FoldoutGroup("默认值")]
        public float DefaultStamina
        {
            get => defaultStamina;
            private set => defaultStamina = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("默认值")]
        public float DefaultHP
        {
            get => defaultHP;
            private set => defaultHP = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("默认值")]
        public float DefaultDefenseValue
        {
            get => defaultDefense;
            private set => defaultDefense = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("默认值")]
        public float DefaultSpeed
        {
            get => defaultSpeed;
            private set => defaultSpeed = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("默认值")]
        public float DefaultHungerValue
        {
            get => defaultHungerValue;
            private set => defaultHungerValue = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("默认值")]
        public float DefaultPower
        {
            get => defaultPower;
            private set => defaultPower = Mathf.Max(0, value);
        }

        #endregion

        #region 最大值

        [ShowInInspector, FoldoutGroup("最大值")]
        public float MaxStamina
        {
            get => maxStamina;
            private set => maxStamina = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("最大值")]
        public float MaxHP
        {
            get => maxHP;
            private set => maxHP = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("最大值")]
        public float MaxDefenseValue
        {
            get => maxDefense;
            private set => maxDefense = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("最大值")]
        public float MaxSpeed
        {
            get => maxSpeed;
            private set => maxSpeed = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("最大值")]
        public float MaxHungerValue
        {
            get => maxEnergy;
            private set => maxEnergy = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("最大值")]
        public float MaxPower
        {
            get => maxPower;
            private set => maxPower = Mathf.Max(0, value);
        }

        #endregion

        #region 当前属性

        [ShowInInspector, FoldoutGroup("当前属性")]
        public float Stamina
        {
            get => stamina;
            set => stamina = Mathf.Clamp(value, 0, MaxStamina);
        }

        [ShowInInspector, FoldoutGroup("当前属性")]
        public float Hp
        {
            get => hp;
            set => hp = Mathf.Clamp(value, 0, MaxHP);
        }

        [ShowInInspector, FoldoutGroup("当前属性")]
        public float DefenseValue
        {
            get => defense;
            set => defense = Mathf.Clamp(value, 0, MaxDefenseValue);
        }

        [ShowInInspector, FoldoutGroup("当前属性")]
        public float Speed
        {
            get => speed;
            set => speed = Mathf.Clamp(value, 0, MaxSpeed);
        }

        [ShowInInspector, FoldoutGroup("当前属性")]
        public float HungerValue
        {
            get => Food_Energy;
            set => Food_Energy = Mathf.Clamp(value, 0, MaxHungerValue);
        }

        [ShowInInspector, FoldoutGroup("当前属性")]
        public float Power
        {
            get => Power;
            set => Power = Mathf.Clamp(value, 0, MaxPower);
        }

        #endregion
    */
    /*    #region 构造函数
        /// <summary>
        /// 角色数据
        /// </summary>
        /// <param name="defaultStamina"> 默认体力值</param>
        /// <param name="defaultHp"> 默认生命值</param>
        /// <param name="defaultDefenseValue"> 默认防御值</param>
        /// <param name="defaultSpeed"> 默认速度</param>
        /// <param name="defaultHungerValue"> 默认饥饿值</param>
        /// <param name="defaultPower"> 默认力量</param>
        public Data(float defaultStamina, float defaultHp, float defaultDefenseValue,
                          float defaultSpeed, float defaultHungerValue, float defaultPower)
        {
            DefaultStamina = defaultStamina;
            DefaultHP = defaultHp;
            DefaultDefenseValue = defaultDefenseValue;
            DefaultSpeed = defaultSpeed;
            DefaultHungerValue = defaultHungerValue;
            DefaultPower = defaultPower;

            ResetMaxValues(); // 初始化最大值
            ResetValuesToMax();    // 初始化当前值
        }

        #endregion
    */
    /*    #region 方法

        /// <summary>
        /// 重置所有属性值为默认值
        /// </summary>
        public void ResetValuesToMax()
        {
            Stamina = DefaultStamina;
            Hp = DefaultHP;
            DefenseValue = DefaultDefenseValue;
            Speed = DefaultSpeed;
            HungerValue = DefaultHungerValue;
            Power = DefaultPower;
        }

        /// <summary>
        /// 恢复所有属性到最大值
        /// </summary>
        public void RestoreToMaxValues()
        {
            Stamina = MaxStamina;
            Hp = MaxHP;
            DefenseValue = MaxDefenseValue;
            Speed = MaxSpeed;
            HungerValue = MaxHungerValue;
            Power = MaxPower;
        }

        /// <summary>
        /// 重置最大值为默认值
        /// </summary>
        public void ResetMaxValues()
        {
            MaxStamina = DefaultStamina;
            MaxHP = DefaultHP;
            MaxDefenseValue = DefaultDefenseValue;
            MaxSpeed = DefaultSpeed;
            MaxHungerValue = DefaultHungerValue;
            MaxPower = DefaultPower;
        }

        #endregion*/
}
