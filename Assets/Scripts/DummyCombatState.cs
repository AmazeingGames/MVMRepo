using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCombatState : State<CharacterManager>
{
    DummyCombatState() : base(EStateType.Combat) { }

    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);
    }

    public override void CaptureInput()
    {
       
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
    }

    public override void ChangeState()
    {
        
    }


    public override void Exit()
    {
    }

   
}
