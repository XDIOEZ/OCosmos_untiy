using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    // ��ǰ����е���Ʒ����   
    [ShowInInspector]
    public ItemData _ItemData = null; // �ؼ��޸�

    public ItemTag CanAcceptItemType = new ItemTag();

    public float SlotMaxVolume = 128f;

    public int Index = -1;

    //��������
    public Inventory Belong_Inventory = null;

    public ItemSlot_UI UI = null;


    public bool IsFull
    {
        get
        {   
            if (_ItemData == null)
            {
                return false;
            }
            return _ItemData.CurrentVolume >= SlotMaxVolume;
        }
    }
    #region ��ɾ��
    /// <summary>
    /// ���������һ����Ʒ��
    /// �����Ϊ�գ���ֱ�ӷ��ø���Ʒ��
    /// �������������ͬID����Ʒ����������ѵ�������
    /// </summary>
    /// <param name="itemData">Ҫ��ӵ���Ʒ���ݡ�</param>
    public void AddItem(ItemData itemData)
    {
        if (_ItemData == null)
        {
            // ���Ϊ�գ�ֱ�ӷ�����Ʒ
            _ItemData = itemData;
        }
        else if (_ItemData.ID == itemData.ID)
        {
            // �����������ͬID����Ʒ�����Ӷѵ�����
            _ItemData.Stack.amount += itemData.Stack.amount;
        }
        // ��������в�ͬID����Ʒ���򲻽����κβ���
    }

    /// <summary>
    /// ���ٲ���еĲ��ֻ�ȫ����Ʒ��
    /// ����������С�ڶѵ�����������ٶѵ���������ղ�ۡ�
    /// </summary>
    /// <param name="destroyCount">Ҫ���ٵ���Ʒ������</param>
    public void DestroyItem(float destroyCount)
    {
        if (_ItemData != null)
        {
            if (_ItemData.Stack.amount > destroyCount)
            {
                // ��������С�ڶѵ����������ٶѵ�
                _ItemData.Stack.amount -= destroyCount;
            }
            else
            {
                // �����������ڻ���ڶѵ���������ղ��
                _ItemData = null;
            }
        }
    }

    /// <summary>
    /// ����һ����۽�����Ʒ��
    /// ����һ�����������Ʒ��
    /// - ����ǰ���Ϊ�գ�����һ����۵���Ʒ������ǰ��ۡ�
    /// - ����ǰ���������ͬID����Ʒ���ϲ��ѵ�������
    /// </summary>
    /// <param name="ItemSlot_Input">Ҫ�����Ĳ�۶���</param>
    public void Change(ItemSlot _ItemData_Input)
    {/*
        if (_ItemData == null)
        {
            // ��ǰ���Ϊ�գ�ֱ�ӽ������۵���Ʒ������ǰ���
            _ItemData = _ItemData_Input._ItemData;
            _ItemData_Input._ItemData = null; // ���������
        }
        else if (_ItemData.ID == _ItemData_Input._ItemData.ID&&_ItemData.ID!=0&&_ItemData_Input._ItemData.ID!=0)//����Id��Ϊ0������ͬID
        {
            // �����������ͬID����Ʒ���ϲ��ѵ�����
            _ItemData.Stack.amount += _ItemData_Input._ItemData.Stack.amount;
            _ItemData_Input._ItemData = null; // ���������
        }
        else
        {*/
        // ��ǰ��۵���Ʒ�������۵���Ʒ��ͬ��������Ʒ
        ItemData temp = _ItemData;
        _ItemData = _ItemData_Input._ItemData;
        _ItemData_Input._ItemData = temp;
        /* }*/
    }
    #endregion

    public void ResetData()
    {
        _ItemData =null;
        Belong_Inventory.onUIChanged.Invoke(Index);
      //  Debug.Log("ResetData");
    }

    public  ItemSlot(Inventory Inventory_)
    {
        Belong_Inventory = Inventory_;
    }





}