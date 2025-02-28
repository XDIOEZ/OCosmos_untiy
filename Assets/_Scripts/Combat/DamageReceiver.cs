using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DamageReceiver : MonoBehaviour
{
    public IReceiveDamage entity_;

    public float hp;

    public Dictionary<string, float> DefenseDict = new Dictionary<string, float>();
    public List<Damager_CheckerPoint> Damager_CheckerPoints = new List<Damager_CheckerPoint>();

    public UltEvent<float> OnChangeHP;
    public UltEvent<float> OnChangeDefense;
    public UltEvent OnDeath;

    public WorldUI_temp ui;

    //挂接实体血量
    public Hp _Hp
    {
        get
        {
            return entity_.Hp;
        }

        set
        {
            entity_.Hp = value;
        }
    }
    //挂接实体防御
    public Defense _Defense
    {
        get
        {
            return entity_.DefenseValue;
        }

        set
        {
            entity_.DefenseValue = value;
        }
    }


    private void Start()
    {
        // 获取子对象填充列表 Damager_CheckerPoints
        Damager_CheckerPoints.Clear();
        Damager_CheckerPoints.AddRange(GetComponentsInChildren<Damager_CheckerPoint>());
        ui = GetComponentInChildren<WorldUI_temp>();
        OnChangeHP += ui.SetText;
        entity_ = GetComponentInParent<IReceiveDamage>();
    }

    public void SetDamageValue(Damage damage)
    {
        // 顿击伤害处理（钝器抗性侧重韧性）
        _Hp.value -= Mathf.Max(damage.blunt - (_Defense.defenseStrength * 0.3f + _Defense.defenseToughness * 0.7f), 0);

        // 穿刺伤害处理（均衡防御）
        _Hp.value -= Mathf.Max(damage.piercing - (_Defense.defenseStrength * 0.5f + _Defense.defenseToughness * 0.5f), 0);

        // 斩击伤害处理（侧重强度防御）
        _Hp.value -= Mathf.Max(damage.slashing - (_Defense.defenseStrength * 0.7f + _Defense.defenseToughness * 0.3f), 0);

        // 魔法伤害处理（无物理防御）
        _Hp.value -= Mathf.Max(damage.magic, 0);

        Debug.Log(" 受到 damage: " + damage + " 剩余 : " + _Hp);

        OnChangeHP.Invoke(_Hp.value);

        if (_Hp.value < 0)
        {
            Debug.Log(name + "血量耗尽");
            Die();
        }
    }

    public void SetDefense(Defense defense)
    {

    }

    public void Die()
    {
        OnDeath.Invoke();
        
    }

}
[System.Serializable]
[MemoryPackable]
public partial class Defense
{
    public float defenseStrength;
    public float defenseToughness;

    //重写+运算符
    public static Defense operator +(Defense a, Defense b)
    {
        Defense result = new Defense();
        result.defenseStrength = a.defenseStrength + b.defenseStrength;
        result.defenseToughness = a.defenseToughness + b.defenseToughness;
        return result;
    }
    //重写-运算符
    public static Defense operator -(Defense a, Defense b)
    {
        Defense result = new Defense();
        result.defenseStrength = a.defenseStrength - b.defenseStrength;
        result.defenseToughness = a.defenseToughness - b.defenseToughness;
        return result;
    }

    //重写TOSTRING方法
    public override string ToString()
    {
        return "防御强度: " + defenseStrength + " 韧性: " + defenseToughness;
    }
}
