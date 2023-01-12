using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Character/Jump")]
public class JumpState : State<CharacterManager>
{
    JumpState() : base() { }

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

    bool hasLeftTheGround;
    bool releaseJump;
    bool inHangTime;

    bool isFalling => rigidbody2D.velocity.y < 0;

    bool pressedJump; 
    float jumpRemember;

    bool AtJumpApex => Mathf.Abs(rigidbody2D.velocity.y) < hangtimeSpeedThreshold;

    float rigidbodyGravity;

    float moveInput;

    GroundCheck groundCheck;
    Rigidbody2D rigidbody2D;
    PlayerGeneral playerGeneral;
    Animator animator;

    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);

        if (groundCheck == null)
            groundCheck = parent.GetComponentInChildren<GroundCheck>();
        if (rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();
        if (playerGeneral == null)
            playerGeneral = parent.GetComponent<PlayerGeneral>();
        if(animator == null)
            animator = parent.GetComponent<Animator>();

        //animator.CrossFade(PlayerGeneral.Jump, 0);

        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (currentClipInfo != null && currentClipInfo.Length >= 1)
        {
            string currentClipName = currentClipInfo[0].clip.name;

            if (currentClipName != "Jump")
            {
                animator.CrossFade(PlayerGeneral.Jump, 0);
            }   
        }
        else
        {
            animator.CrossFade(PlayerGeneral.Jump, 0);
        }


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
        jumpRemember -= Time.deltaTime;

        if (!groundCheck.IsGrounded())
        {
            hasLeftTheGround = true;
        }

        if (pressedJump)
        {
            pressedJump = false;
            jumpRemember = jumpBufferTime;
        }

        if (releaseJump)
        {
            releaseJump = false;

            if (!isFalling)
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

            if (isFalling)
            {
                IncreaseFallingGravityScale();
                ClampFallSpeed();
            }
        }
        //Debug.Log($"Is JumpRemember greater than 0: {jumpRemember > 0}");
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
        else if (isFalling && groundCheck.IsFacingLedge() && playerGeneral.ledgeGrabTimer <= 0)
        {
            runner.SetState(typeof(LedgeHold));
        }
    }

    public override void Exit()
    {
        rigidbody2D.gravityScale = rigidbodyGravity;
        jumpRemember = 0;
    }
}
