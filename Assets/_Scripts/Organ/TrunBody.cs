using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunBody : Organ, IFunction_TurnBody
{
    public TrunBody_Data data; // 可以在 Inspector 中赋值

    void Awake()
    {
        if (data == null)
        {
            data = new TrunBody_Data();
        }
    }

    public void TurnBodyToDirection(Vector2 targetDirection)
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 mouseDirection = new Vector2(worldMousePosition.x - transform.parent.position.x, 0).normalized;

        if (Mathf.Sign(mouseDirection.x) == Mathf.Sign(targetDirection.x))
        {
            data.isTurning = true;

            if (targetDirection.x > 0)
            {
                data.Direction = Vector2.right;
            }
            else
            {
                data.Direction = Vector2.left;
            }

            float targetAngle = data.Direction == Vector2.right ? 0f : 180f;
            float currentAngle = transform.parent.rotation.eulerAngles.y;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, data.rotationSpeed * Time.deltaTime);
            transform.parent.rotation = Quaternion.Euler(0f, newAngle, 0f);

            if (Mathf.Abs(newAngle - targetAngle) < 1f)
            {
                data.isTurning = false;
            }
        }
        else
        {
            data.isTurning = false;
        }
    }
}


[System.Serializable]
public class TrunBody_Data : Organ_Data
{
    public float rotationSpeed; // 旋转速度
    public Vector2 Direction; // 当前方向
    public bool isTurning; // 是否正在转身
}
