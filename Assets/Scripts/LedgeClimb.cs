using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[CreateAssetMenu(menuName = "States/Character/Ledge/Climb")]
public class LedgeClimb : State<CharacterManager>
{
    [SerializeField] float ledgeClimbXOffsetStart;
    [SerializeField] float ledgeClimbYOffsetStart;

    [SerializeField] float ledgeClimbXOffsetEnd;
    [SerializeField] float ledgeClimbYOffsetEnd;

    float ledgeClimbLength;
    float ledgeClimbTimer;

    Vector2 ledgePositionBottom;
    Vector2 ledgePositionStart;
    Vector2 ledgePositionEnd;

    PlayerGeneral playerGeneral;
    GroundCheck groundCheck;
    Animator animator;
    Rigidbody2D rigidbody2D;
    Transform transform;

    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);

        if(groundCheck == null)
            groundCheck = parent.GetComponent<GroundCheck>();
        if(playerGeneral == null)
            playerGeneral = parent.GetComponent<PlayerGeneral>();
        if (animator == null)
            animator = parent.GetComponent<Animator>();
        if(rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();
        if(transform == null) 
            transform = parent.GetComponent<Transform>();

        ledgeClimbTimer = 0;
        ledgePositionBottom = groundCheck.GetLedgeRaycastBodyOrigin.position;

        if (playerGeneral.IsFacingRight)
        {
            ledgePositionStart = new Vector2(Mathf.Floor(ledgePositionBottom.x + groundCheck.GetLedgeRaycastLength) - ledgeClimbXOffsetStart, Mathf.Floor(ledgePositionBottom.y) + ledgeClimbYOffsetStart);
            ledgePositionEnd = new Vector2(Mathf.Floor(ledgePositionBottom.x + groundCheck.GetLedgeRaycastLength) + ledgeClimbXOffsetEnd, Mathf.Floor(ledgePositionBottom.y) + ledgeClimbYOffsetEnd);
        }
        else
        {
            ledgePositionStart = new Vector2(Mathf.Ceil(ledgePositionBottom.x - groundCheck.GetLedgeRaycastLength) + ledgeClimbXOffsetStart, Mathf.Floor(ledgePositionBottom.y) + ledgeClimbYOffsetStart);
            ledgePositionEnd = new Vector2(Mathf.Ceil(ledgePositionBottom.x - groundCheck.GetLedgeRaycastLength) - ledgeClimbYOffsetEnd, Mathf.Floor(ledgePositionBottom.y) + ledgeClimbYOffsetEnd);
        }
        transform.position = ledgePositionStart;

        animator.CrossFade(PlayerGeneral.Jump, 0);

        ledgeClimbLength = animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public override void CaptureInput()
    {
        
    }

    public override void Update()
    {
        ledgeClimbTimer += Time.deltaTime;
        Debug.DrawLine(ledgePositionStart, ledgePositionEnd);
    }

    public override void FixedUpdate()
    {
        
    }

    public override void ChangeState()
    {
        if (ledgeClimbTimer >= ledgeClimbLength)
        {
            runner.SetState(typeof(WalkState));
        }
    }

    public override void Exit()
    {
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.position = ledgePositionEnd;
    }
}
