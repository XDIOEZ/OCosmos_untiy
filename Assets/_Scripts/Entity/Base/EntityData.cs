using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[MemoryPackable]
public  partial class EntityData
{
    #region ˽���ֶ�
    [Title("ʵ������")]
    // ��ǰֵ�ֶΣ���ʼ��ΪĬ��ֵ��
    // ����
    public float stamina = 100;
    // ����
    public Hp hp  = new Hp(100);
    //������     
    public Defense defense ;
    //�ٶ�
    public float speed = 8;
    //����
    public float Power = 10;
    // ���ֵ�ֶ�
    public float maxStamina = 100;
    public Hp maxHP = new Hp(100);
    public float maxDefense = 10;
    public float maxSpeed = 8;
    public float maxPower = 10;
    /// <summary>
    /// ��ɫ����
    /// </summary>
    /// <param name="stamina">����</param>
    /// <param name="hp">Ѫ��</param>
    /// <param name="defense">����</param>
    /// <param name="speed">�ƶ��ٶ�</param>
    /// <param name="hungerValue">������</param>
    /// <param name="power">����</param>
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
/*    // Ĭ��ֵ�ֶ�
    private float defaultStamina = 100;
    private float defaultHP = 100;
    private float defaultDefense = 10;
    private float defaultSpeed = 8;
    private float defaultHungerValue = 100;
    private float defaultPower = 10;
*/
    #endregion
    /*
        #region Ĭ��ֵ

        [ShowInInspector, FoldoutGroup("Ĭ��ֵ")]
        public float DefaultStamina
        {
            get => defaultStamina;
            private set => defaultStamina = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("Ĭ��ֵ")]
        public float DefaultHP
        {
            get => defaultHP;
            private set => defaultHP = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("Ĭ��ֵ")]
        public float DefaultDefenseValue
        {
            get => defaultDefense;
            private set => defaultDefense = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("Ĭ��ֵ")]
        public float DefaultSpeed
        {
            get => defaultSpeed;
            private set => defaultSpeed = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("Ĭ��ֵ")]
        public float DefaultHungerValue
        {
            get => defaultHungerValue;
            private set => defaultHungerValue = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("Ĭ��ֵ")]
        public float DefaultPower
        {
            get => defaultPower;
            private set => defaultPower = Mathf.Max(0, value);
        }

        #endregion

        #region ���ֵ

        [ShowInInspector, FoldoutGroup("���ֵ")]
        public float MaxStamina
        {
            get => maxStamina;
            private set => maxStamina = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("���ֵ")]
        public float MaxHP
        {
            get => maxHP;
            private set => maxHP = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("���ֵ")]
        public float MaxDefenseValue
        {
            get => maxDefense;
            private set => maxDefense = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("���ֵ")]
        public float MaxSpeed
        {
            get => maxSpeed;
            private set => maxSpeed = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("���ֵ")]
        public float MaxHungerValue
        {
            get => maxEnergy;
            private set => maxEnergy = Mathf.Max(0, value);
        }

        [ShowInInspector, FoldoutGroup("���ֵ")]
        public float MaxPower
        {
            get => maxPower;
            private set => maxPower = Mathf.Max(0, value);
        }

        #endregion

        #region ��ǰ����

        [ShowInInspector, FoldoutGroup("��ǰ����")]
        public float Stamina
        {
            get => stamina;
            set => stamina = Mathf.Clamp(value, 0, MaxStamina);
        }

        [ShowInInspector, FoldoutGroup("��ǰ����")]
        public float Hp
        {
            get => hp;
            set => hp = Mathf.Clamp(value, 0, MaxHP);
        }

        [ShowInInspector, FoldoutGroup("��ǰ����")]
        public float DefenseValue
        {
            get => defense;
            set => defense = Mathf.Clamp(value, 0, MaxDefenseValue);
        }

        [ShowInInspector, FoldoutGroup("��ǰ����")]
        public float Speed
        {
            get => speed;
            set => speed = Mathf.Clamp(value, 0, MaxSpeed);
        }

        [ShowInInspector, FoldoutGroup("��ǰ����")]
        public float HungerValue
        {
            get => Food_Energy;
            set => Food_Energy = Mathf.Clamp(value, 0, MaxHungerValue);
        }

        [ShowInInspector, FoldoutGroup("��ǰ����")]
        public float Power
        {
            get => Power;
            set => Power = Mathf.Clamp(value, 0, MaxPower);
        }

        #endregion
    */
    /*    #region ���캯��
        /// <summary>
        /// ��ɫ����
        /// </summary>
        /// <param name="defaultStamina"> Ĭ������ֵ</param>
        /// <param name="defaultHp"> Ĭ������ֵ</param>
        /// <param name="defaultDefenseValue"> Ĭ�Ϸ���ֵ</param>
        /// <param name="defaultSpeed"> Ĭ���ٶ�</param>
        /// <param name="defaultHungerValue"> Ĭ�ϼ���ֵ</param>
        /// <param name="defaultPower"> Ĭ������</param>
        public Data(float defaultStamina, float defaultHp, float defaultDefenseValue,
                          float defaultSpeed, float defaultHungerValue, float defaultPower)
        {
            DefaultStamina = defaultStamina;
            DefaultHP = defaultHp;
            DefaultDefenseValue = defaultDefenseValue;
            DefaultSpeed = defaultSpeed;
            DefaultHungerValue = defaultHungerValue;
            DefaultPower = defaultPower;

            ResetMaxValues(); // ��ʼ�����ֵ
            ResetValuesToMax();    // ��ʼ����ǰֵ
        }

        #endregion
    */
    /*    #region ����

        /// <summary>
        /// ������������ֵΪĬ��ֵ
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
        /// �ָ��������Ե����ֵ
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
        /// �������ֵΪĬ��ֵ
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
