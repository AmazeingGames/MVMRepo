using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Character/Jump")]
public class JumpState : State<CharacterManager>
{
    JumpState() : base(EStateType.Jump) { }

    [Header("Jumping/Falling")]
    //[SerializeField] float jumpRememberTime;
    [Range(0, 1)]
    [SerializeField] float cutJumpHeight;
    [SerializeField] float jumpForce;
    [SerializeField] float fallGravityMultiplier;
    [Range(0, 1)]
    [SerializeField] float hangtimeGravityMultiplier;
    [SerializeField] float hangtimeSpeedThreshold;
    [SerializeField] float maxFallSpeed;
    [SerializeField] float jumpBufferTime;

    [Header("Horizontal Movement")]
    [SerializeField] float velocityPower;
    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float hangtimeAccelerationMultiplier;
    [SerializeField] float hangtimeMaxSpeedMultiplier;

    GroundCheck groundCheck;
    Rigidbody2D rigidbody2D;

    bool hasLeftTheGround;
    bool releaseJump;
    bool inHangTime;

    bool pressedJump; 
    float jumpRemember;


    bool AtJumpApex => Mathf.Abs(rigidbody2D.velocity.y) < hangtimeSpeedThreshold;

    float rigidbodyGravity;

    float moveInput;

    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);

        if (groundCheck == null)
            groundCheck = parent.GetComponentInChildren<GroundCheck>();
        if (rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();

        jumpRemember = 0;
        hasLeftTheGround = false;
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
        rigidbodyGravity = rigidbody2D.gravityScale;
    }

    public override void CaptureInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonUp("Jump"))
        {
            releaseJump = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            pressedJump = true;
        }
    }
    public override void Update()
    {
        if (!groundCheck.IsGrounded())
        {
            hasLeftTheGround = true;
        }

        if (releaseJump)
        {
            releaseJump = false;

            if (rigidbody2D.velocity.y > 0)
            {
                ReduceJumpVelocity();
            }
        }

        if (AtJumpApex)
        {
            DecreaseGravityScaleAtApex();
        }
        else 
        {
            inHangTime = false;

            if (rigidbody2D.velocity.y < 0)
            {
                IncreaseFallingGravityScale();
                ClampFallSpeed();
            }
        }

        if (pressedJump)
        {
            pressedJump = false;
            jumpRemember = jumpBufferTime;
        }

        Debug.Log($"Is JumpRemember greater than 0: {jumpRemember > 0}");
        jumpRemember -= Time.deltaTime;
        //Debug.Log($"JumpRemember: {jumpRemember}");

    }

    void DecreaseGravityScaleAtApex()
    {
        inHangTime = true;
        rigidbody2D.gravityScale = rigidbodyGravity * hangtimeGravityMultiplier;
    }

    void IncreaseFallingGravityScale()
    {
        rigidbody2D.gravityScale = rigidbodyGravity * fallGravityMultiplier;
    }

    void ClampFallSpeed()
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, Mathf.Max(rigidbody2D.velocity.y, -maxFallSpeed));
    }

    void ReduceJumpVelocity()
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y * cutJumpHeight);
    }

    public override void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float targetSpeed = moveInput * moveSpeed;
        if (inHangTime)
            targetSpeed *= hangtimeMaxSpeedMultiplier;

        float speedDiference = targetSpeed - rigidbody2D.velocity.x;

        float accelerationRate = (Mathf.Abs(targetSpeed) > .01f) ? acceleration : deceleration;
        if (inHangTime)
            accelerationRate *= hangtimeAccelerationMultiplier;

        float movement = Mathf.Pow(Mathf.Abs(speedDiference) * accelerationRate, velocityPower) * Mathf.Sign(speedDiference);

        rigidbody2D.AddForce(movement * Vector2.right);
    }

    public override void ChangeState()
    {
        if (hasLeftTheGround && groundCheck.IsGrounded())
        {
            if (jumpRemember > 0)
            {
                runner.SetState(typeof(JumpState));
            }
            else
            {
                runner.SetState(typeof(WalkState));
            }
        }
    }

    public override void Exit()
    {
        rigidbody2D.gravityScale = rigidbodyGravity;
        jumpRemember = 0;
    }
}