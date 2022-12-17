using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Character/Attack/Entry")]
public class EntryAttack : NewCombatState
{
    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);

        attackIndex = 1;
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
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void ChangeState()
    {
        if (fixedTimer < 0)
        {
            //Debug.Log($"FixedTimer ({fixedTimer}) < 0");

            if (shouldCombo)
            {
                runner.SetState(typeof(SecondAttack));
            }
            else if(jumpPressedTimer > 0)
            {
                runner.SetState(typeof(JumpState));
            }
            else
            {
                runner.SetState(typeof(WalkState));
            }
        }
    }
}
