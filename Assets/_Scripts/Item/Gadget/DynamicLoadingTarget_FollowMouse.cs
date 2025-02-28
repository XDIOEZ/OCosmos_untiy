using UnityEngine;

public class DynamicLoadingTarget_FollowMouse : ChunkLoader
{
    public GameObject FirstTile_P;
    public Vector2 offset; // 定义偏移量，ChunkSize.x 表示水平偏移，ChunkSize.y 表示垂直偏移
    private Transform[] child;
    private Vector3 lastPosition;

    public bool useFirstChild = false; // 用于决定是使用第一个子对象还是最后一个子对象
    //添加bool决定是否 为true时仅在编辑器模式下使用 跟随鼠标的动态加载目标 为false时在运行时也可以跟随使用
    public bool FollowInPlayMode = true; // 用于决定是否仅在编辑器模式下使用 跟随鼠标的动态加载目标
    void Start()
    {
        SendLoadingData();
        lastPosition = transform.position; // 初始化保存上一次位置
        //FollowInPlayMode = false; // 仅在编辑器模式下使用 跟随鼠标的动态加载目标
        FirstTile_P = GameObject.Find("Tilemap"); // 找到 FirstTile_P 对象
        if (FirstTile_P == null)
        {
            Debug.Log("Can't find FirstTile_P");
        }
        
    }

    void Update()
    {
        if (FollowInPlayMode&&FirstTile_P != null)
        {
            // 检查 FirstTile_P 是否有子对象
            child = FirstTile_P.GetComponentsInChildren<Transform>();

            if (child.Length > 1) // 确保有子对象
            {
                // 根据 useFirstChild 的值选择第一个或最后一个子对象
                Transform targetChild = useFirstChild ? child[1] : child[child.Length - 1];
                Vector3 targetPosition = targetChild.position + new Vector3(-offset.x, offset.y, 0); // 向西北偏移

                transform.position = targetPosition; // 更新当前对象位置
            }

            // 保存上一次的位置
            lastPosition = transform.position;
        }

    }
}
