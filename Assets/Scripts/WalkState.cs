 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Character/Walk")]
public class WalkState : State<CharacterManager>
{
    WalkState() : base(EStateType.Walk)
    {
       
    }

    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float velocityPower;
    [SerializeField] float friction;

    //Jump Buffer
    [SerializeField] float jumpRememberTime;
    //Hang Time
    [SerializeField] float groundRememberTime;

    float jumpRemember;
    float groundRemember;

    float moveInput;

    Rigidbody2D rigidbody2D;
    GroundCheck groundCheck;
    Animator animator;

    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);

        if (groundCheck == null)
            groundCheck = parent.GetComponentInChildren<GroundCheck>();
        if (rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = parent.GetComponent<Animator>();
    }

    public override void CaptureInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            //Debug.Log("pressed jump");
            jumpRemember = jumpRememberTime;
        }
    }

    public override void Update()
    {
        jumpRemember -= Time.deltaTime;
        groundRemember -= Time.deltaTime;

        if (groundCheck.IsGrounded())
        {
            groundRemember = groundRememberTime;
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

    }

    public override void Exit()
    {
        jumpRemember = 0;
        groundRemember = 0;
    }
}
