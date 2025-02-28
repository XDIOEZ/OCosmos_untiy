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

    //�ҽ�ʵ��Ѫ��
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
    //�ҽ�ʵ�����
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
        // ��ȡ�Ӷ�������б� Damager_CheckerPoints
        Damager_CheckerPoints.Clear();
        Damager_CheckerPoints.AddRange(GetComponentsInChildren<Damager_CheckerPoint>());
        ui = GetComponentInChildren<WorldUI_temp>();
        OnChangeHP += ui.SetText;
        entity_ = GetComponentInParent<IReceiveDamage>();
    }

    public void SetDamageValue(Damage damage)
    {
        // �ٻ��˺������������Բ������ԣ�
        _Hp.value -= Mathf.Max(damage.blunt - (_Defense.defenseStrength * 0.3f + _Defense.defenseToughness * 0.7f), 0);

        // �����˺��������������
        _Hp.value -= Mathf.Max(damage.piercing - (_Defense.defenseStrength * 0.5f + _Defense.defenseToughness * 0.5f), 0);

        // ն���˺���������ǿ�ȷ�����
        _Hp.value -= Mathf.Max(damage.slashing - (_Defense.defenseStrength * 0.7f + _Defense.defenseToughness * 0.3f), 0);

        // ħ���˺����������������
        _Hp.value -= Mathf.Max(damage.magic, 0);

        Debug.Log(" �ܵ� damage: " + damage + " ʣ�� : " + _Hp);

        OnChangeHP.Invoke(_Hp.value);

        if (_Hp.value < 0)
        {
            Debug.Log(name + "Ѫ���ľ�");
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

    //��д+�����
    public static Defense operator +(Defense a, Defense b)
    {
        Defense result = new Defense();
        result.defenseStrength = a.defenseStrength + b.defenseStrength;
        result.defenseToughness = a.defenseToughness + b.defenseToughness;
        return result;
    }
    //��д-�����
    public static Defense operator -(Defense a, Defense b)
    {
        Defense result = new Defense();
        result.defenseStrength = a.defenseStrength - b.defenseStrength;
        result.defenseToughness = a.defenseToughness - b.defenseToughness;
        return result;
    }

    //��дTOSTRING����
    public override string ToString()
    {
        return "����ǿ��: " + defenseStrength + " ����: " + defenseToughness;
    }
}
