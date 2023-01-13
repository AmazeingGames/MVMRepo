 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Character/Walk")]
public class WalkState : State<CharacterManager>
{
    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float velocityPower;
    [SerializeField] float friction;
    [SerializeField] float runAnimationVelocityCutOff;

    //Jump Buffer
    [SerializeField] float jumpRememberTime;
    //Hang Time
    [SerializeField] float groundRememberTime;

    bool pressedJump;
    float jumpRemember;
    float groundRemember;

    float moveInput;

    Rigidbody2D rigidbody2D;
    GroundCheck groundCheck;
    Animator animator;
    PlayerGeneral playerGeneral;

    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);

        if (groundCheck == null)
            groundCheck = parent.GetComponentInChildren<GroundCheck>();
        if (rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = parent.GetComponent<Animator>();
        if (playerGeneral == null)
            playerGeneral = parent.GetComponent<PlayerGeneral>();
    }

    public override void CaptureInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            pressedJump = true;
        }
    }

    public override void Update()
    {
        jumpRemember -= Time.deltaTime;
        groundRemember -= Time.deltaTime;

        if (groundCheck.IsGrounded())
        {
            groundRemember = groundRememberTime;

            if (Mathf.Abs(moveInput) > 0)
            {
                animator.CrossFade(PlayerGeneral.Run, 0);
            }
            //Stops run animation only if velocity is less than a specified value (.1 - .5) and player is not pressing run 
            else if(Mathf.Abs(rigidbody2D.velocity.x) < runAnimationVelocityCutOff)
            {
                animator.CrossFade(PlayerGeneral.Idle, 0);
            }
        }
        else
        {
            animator.CrossFade(PlayerGeneral.Jump, 0);
        }

        if (pressedJump)
        {
            pressedJump = false;
            jumpRemember = jumpRememberTime;
        }
    }

    public override void FixedUpdate()
    {
        MovePlayer();
        ApplyFriction();
    }

    void MovePlayer()
    {
        float targetSpeed = moveInput * moveSpeed;

        float speedDiference = targetSpeed - rigidbody2D.velocity.x;

        float accelerationRate = (Mathf.Abs(targetSpeed) > .01f) ? acceleration : deceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDiference) * accelerationRate, velocityPower) * Mathf.Sign(speedDiference);

        rigidbody2D.AddForce(movement * Vector2.right);
    }

    void ApplyFriction()
    {
        if (groundRemember > 0 && Mathf.Abs(moveInput) < .01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rigidbody2D.velocity.x), Mathf.Abs(friction));

            amount *= Mathf.Sign(rigidbody2D.velocity.x);

            rigidbody2D.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    public override void ChangeState()
    {
        if (jumpRemember > 0 && groundRemember > 0)
        {
            runner.SetState(typeof(JumpState));
        }

        if (Input.GetMouseButtonDown(0))
        {
            runner.SetState(typeof(EntryAttack));
        }

        if (!groundCheck.IsGrounded() && groundCheck.IsFacingLedge() && playerGeneral.ledgeGrabTimer <= 0)
        {
            runner.SetState(typeof(LedgeHold));
        }
    }

    public override void Exit()
    {
        jumpRemember = 0;
        groundRemember = 0;
    }
}
