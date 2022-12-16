using UnityEngine;

public class CombatStateMachine : MonoBehaviour
{
    public string customName;

    private CombatState mainStateType;

    public CombatState CurrentState { get; private set; }
    private CombatState nextState;

    // Update is called once per frame
    void Update()
    {
        if (nextState != null)
        {
            SetState(nextState);
        }

        CurrentState?.OnUpdate();
    }

    private void SetState(CombatState _newState)
    {
        nextState = null;
        CurrentState?.OnExit();
        CurrentState = _newState;
        CurrentState.OnEnter(this);
    }

    public void SetNextState(CombatState _newState)
    {
        if (_newState != null)
        {
            nextState = _newState;
        }
    }

    private void LateUpdate()
    {
        CurrentState?.OnLateUpdate();
    }

    private void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate();
    }

    public void SetNextStateToMain()
    {
        nextState = mainStateType;
    }

    private void Awake()
    {
        SetNextStateToMain();

    }

    private void OnValidate()
    {
        if (mainStateType == null)
        {
            if (customName == "Combat")
            {
                mainStateType = new IdleCombatState();
            }
        }
    }
}