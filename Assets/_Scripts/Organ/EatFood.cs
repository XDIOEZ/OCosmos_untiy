using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFood : MonoBehaviour
{
    public HungryValueChange hungryValueChange;

    public void Start()
    {
        hungryValueChange = GetComponent<HungryValueChange>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // �����ﴦ��������ײ���߼�
        //�����ײ�����Ϲҽ���CanBeEat�������ִ�г�ʳ���߼�
        CanBeEat canBeEat = collision.GetComponent<CanBeEat>();
        if (collision.GetComponent<CanBeEat>())
        {
            // ��ʳ���߼�
            hungryValueChange.CurrentHungryValue += canBeEat.AddEnergyValue;
            Destroy(collision.gameObject);
        }
       
    }


}
