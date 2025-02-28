using UnityEngine;

public class DynamicLoadingTarget_FollowMouse : ChunkLoader
{
    public GameObject FirstTile_P;
    public Vector2 offset; // ����ƫ������ChunkSize.x ��ʾˮƽƫ�ƣ�ChunkSize.y ��ʾ��ֱƫ��
    private Transform[] child;
    private Vector3 lastPosition;

    public bool useFirstChild = false; // ���ھ�����ʹ�õ�һ���Ӷ��������һ���Ӷ���
    //���bool�����Ƿ� Ϊtrueʱ���ڱ༭��ģʽ��ʹ�� �������Ķ�̬����Ŀ�� Ϊfalseʱ������ʱҲ���Ը���ʹ��
    public bool FollowInPlayMode = true; // ���ھ����Ƿ���ڱ༭��ģʽ��ʹ�� �������Ķ�̬����Ŀ��
    void Start()
    {
        SendLoadingData();
        lastPosition = transform.position; // ��ʼ��������һ��λ��
        //FollowInPlayMode = false; // ���ڱ༭��ģʽ��ʹ�� �������Ķ�̬����Ŀ��
        FirstTile_P = GameObject.Find("Tilemap"); // �ҵ� FirstTile_P ����
        if (FirstTile_P == null)
        {
            Debug.Log("Can't find FirstTile_P");
        }
        
    }

    void Update()
    {
        if (FollowInPlayMode&&FirstTile_P != null)
        {
            // ��� FirstTile_P �Ƿ����Ӷ���
            child = FirstTile_P.GetComponentsInChildren<Transform>();

            if (child.Length > 1) // ȷ�����Ӷ���
            {
                // ���� useFirstChild ��ֵѡ���һ�������һ���Ӷ���
                Transform targetChild = useFirstChild ? child[1] : child[child.Length - 1];
                Vector3 targetPosition = targetChild.position + new Vector3(-offset.x, offset.y, 0); // ������ƫ��

                transform.position = targetPosition; // ���µ�ǰ����λ��
            }

            // ������һ�ε�λ��
            lastPosition = transform.position;
        }

    }
}
