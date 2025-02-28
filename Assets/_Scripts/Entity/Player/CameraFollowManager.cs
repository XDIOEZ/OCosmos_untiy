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
        //跟随父对象
        Vcam.Follow = transform.parent;
        //重命名物体名字,本体+父对象
        transform.name = $"{transform.parent.name} 的 Camera";

        CameraFollowItem = GetComponentInParent<Item>();

        //使此transform组件数值 不受父对象影响 但层级窗口的位置不变
        transform.SetParent(null);
    }

    //修改视野范围方法
    public void ChangeCameraView(float view)
    {
        Vcam.m_Lens.OrthographicSize += view;
        Debug.Log("视野范围修改为：" + Vcam.m_Lens.FieldOfView);
    }
}
