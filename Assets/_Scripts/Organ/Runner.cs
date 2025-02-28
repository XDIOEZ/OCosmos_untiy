using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
public interface IRunner
{
    public void SwitchToRun(bool isRun);
}

public class Runner : Organ, IRunner
{
    public float RunSpeed = 5f;

    public Mover _mover;
    public UltEvent OnRunStart;
    public UltEvent OnRunStop;
    public bool IsRun { get; private set; } = false;
    public Mover _Mover
    {
        get
        {
            if (_mover == null)
            {
                _mover = XDTool.GetComponentInChildrenAndParent<Mover>(gameObject);
            }
            return _mover;
        }
    }

    private void OnEnable()
    {
        OnRunStart+=StartRun;
        OnRunStop += StopRun;
    }
    private void OnDisable()
    {
        OnRunStart-=StartRun;
        OnRunStop -= StopRun;
    }

    public void StartRun()
    {
        _Mover.AddSpeedChange(("跑步速度", ValueChangeType.Add,0), +RunSpeed);
    }

    public void StopRun()
    {
        _Mover.RemoveSpeedChange(("跑步速度", ValueChangeType.Add, 0));
    }

    public void SwitchToRun(bool isRun)
    {
        if (isRun)
        {
            OnRunStart.Invoke();
        }
        else
        {
            OnRunStop.Invoke();
        }
    }
}
