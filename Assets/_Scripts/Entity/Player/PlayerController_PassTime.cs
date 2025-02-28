using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController_PassTime : MonoBehaviour
{
    Mover _mover;
    AttackTrigger _handAttack;
    [ShowInInspector]
    public Mover Mover
    {
        get
        {
            if (_mover == null)
            {
                _mover = GetComponent<Mover>();
            }
            return _mover;
        }
        set
        {
            _mover = value;
        }
    }
    [ShowInInspector]
/*    public AttackTrigger AttackTrigger
    {
        get 
        {
           
            if (_handAttack == null)
            {   
                Debug.Log("get");
                _handAttack = GetComponent<ICan_TriggerAttack>().GetHandAttack();
            }
            return _handAttack; 
        }
        set { _handAttack = value; }
    *//*}*/

    void Update()
    {
       
    }

    Vector2 GetMoveInput()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveInput.Normalize();
        return moveInput;
    }
}