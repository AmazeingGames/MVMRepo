using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> : ScriptableObject where T : MonoBehaviour
{
    public enum EStateType { Walk, Jump, Combat, LedgeHold, LedgeClimb }

    protected T runner;

    public virtual void Enter(T parent)
    {
        runner = parent;
    }

    public abstract void CaptureInput();

    public abstract void Update();

    public abstract void FixedUpdate();

    public abstract void ChangeState();

    public abstract void Exit(); 
}
