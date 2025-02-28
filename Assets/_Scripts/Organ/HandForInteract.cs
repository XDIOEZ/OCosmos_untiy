using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandForInteract : MonoBehaviour, IInteract
{
    [ShowInInspector]
    public IInteract Intractable_go;
    public void OnTriggerEnter2D(Collider2D collision)
    {
         Intractable_go = collision.GetComponent<IInteract>();
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        Interact_Cancel();
        Intractable_go = null;
    }

    public void Interact_Start()
    {
        if (Intractable_go!= null)
        {
            Intractable_go.Interact_Start();
        }
    }

    public void Interact_Cancel()
    {
        if (Intractable_go!= null)
        {
            Intractable_go.Interact_Cancel();
        }
    }

    public void Interact_Update()
    {
        throw new System.NotImplementedException();
    }
}
