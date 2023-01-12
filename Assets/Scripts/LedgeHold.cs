using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Character/Ledge/Hold")]
public class LedgeHold : State<CharacterManager>
{
    [SerializeField] Vector3 hangPositionOffset;
    [SerializeField] float timeBetweenLedgeGrabs;

    public Action droppedFromLedge;
    public float TimerBetweenGrabs { get; set; }

    float moveInput;
    bool shouldClimb;
    bool shouldDrop;

    Rigidbody2D rigidbody2D;
    Animator animator;
    Transform transform;


    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);
        
        if(rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();
        if(animator == null)
            animator = parent.GetComponent<Animator>();
        if(transform == null)
            transform = parent.transform;


        TimerBetweenGrabs = 0;

        animator.CrossFade(PlayerGeneral.LedgeHang, 0);

        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        shouldClimb = false;
        shouldDrop = false;

        transform.position += hangPositionOffset;
    }

    public override void CaptureInput()
    {
        moveInput = Input.GetAxisRaw("Vertical");    
    }

    public override void Update()
    {
        if (moveInput > 0)
        {
            shouldClimb = true;
        }
        else if (moveInput < 0)
        {
            shouldDrop = true;
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void ChangeState()
    {
        if (shouldClimb)
        {
            runner.SetState(typeof(LedgeClimb));
        }

        if (shouldDrop)
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

            runner.SetState(typeof(WalkState));
        }
    }

    public override void Exit()
    {
    }
}
