using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntryState : CombatState
{
    public override void OnEnter(CombatStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        CombatState nextState = (CombatState)new GroundEntryState();
        stateMachine.SetNextState(nextState);
    }
}
