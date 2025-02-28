using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowManager : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public Item CameraFollowItem;

    public CinemachineVirtualCamera Vcam
    {
        get
        {
            if (vcam == null)
            {
                vcam = GetComponent<CinemachineVirtualCamera>();
            }
            return vcam;
        }

        set
        {
            vcam = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //���游����
        Vcam.Follow = transform.parent;
        //��������������,����+������
        transform.name = $"{transform.parent.name} �� Camera";

        CameraFollowItem = GetComponentInParent<Item>();

        //ʹ��transform�����ֵ ���ܸ�����Ӱ�� ���㼶���ڵ�λ�ò���
        transform.SetParent(null);
    }

    //�޸���Ұ��Χ����
    public void ChangeCameraView(float view)
    {
        Vcam.m_Lens.OrthographicSize += view;
        Debug.Log("��Ұ��Χ�޸�Ϊ��" + Vcam.m_Lens.FieldOfView);
    }
}
