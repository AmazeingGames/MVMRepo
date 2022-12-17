using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Character/Attack/Penultimate")]
public class PenultimateAttack : NewCombatState
{
    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);

        attackIndex = 3;
        rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);

        animator.SetTrigger($"Attack{attackIndex}");
        Debug.Log($"Player Attack {attackIndex} Fired!");
    }

    public override void CaptureInput()
    {
        base.CaptureInput();
    }

    public override void Update()
    {
        base.Update();

        if (fixedTimer < 0)
        {
            //Debug.Log($"FixedTimer ({fixedTimer}) < 0");

            if (shouldCombo)
            {
                runner.SetState(typeof(FinalAttack));
            }
            else if (jumpPressedTimer > 0)
            {
                runner.SetState(typeof(JumpState));
            }
            else
            {
                runner.SetState(typeof(WalkState));
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void ChangeState()
    {
        base.ChangeState();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
