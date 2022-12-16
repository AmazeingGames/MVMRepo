using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Jump Variables
    [Header("Jump")]
    [SerializeField] float jumpForce; //

    [SerializeField] float groundRaycastLength; //
    [SerializeField] LayerMask groundLayer; //
    [SerializeField] Transform groundRaycastOrigin;//

    //Buffer Time
    [SerializeField] float jumpRememberTime; //
    //Hang Time
    [SerializeField] float groundRememberTime; //
    [SerializeField] float cutJumpHeight; //

    public bool IsGrounded { get; private set; } //
    bool releaseJump; //
    float jumpRemember; //
    float groundRemember; //

    #endregion Jump Variables

    #region Movement Variables
    [Header("Movement")]

    [SerializeField] float moveSpeed; //
    [SerializeField] float acceleration; //
    [SerializeField] float deceleration; //
    [SerializeField] float velocityPower; //
    [SerializeField] float friction; //

    public bool IsFacingRight { get; private set; } = true;
    public bool canMove = true;

    float moveInput; //
    #endregion Movement Variables

    Rigidbody2D rb2d; //

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); //
    }

    // Update is called once per frame
    void Update()
    {
        #region Jump

        IsGrounded = Physics2D.Raycast(groundRaycastOrigin.position, Vector2.down, groundRaycastLength, groundLayer); //
        Debug.DrawRay(groundRaycastOrigin.position, Vector2.down * groundRaycastLength, Color.red); //

        jumpRemember -= Time.deltaTime; //
        groundRemember -= Time.deltaTime; //

        if (jumpRemember < 0)
            jumpRemember = 0;

        if (groundRemember < 0)
            groundRemember = 0;


        if (Input.GetButtonDown("Jump")) //
        {
            //Debug.Log("pressed jump");
            jumpRemember = jumpRememberTime; //
        } 

        if (IsGrounded) //
        {
            //Debug.Log("is grounded");
            groundRemember = groundRememberTime; //
        }

        if (Input.GetButtonUp("Jump")) //
        {
            releaseJump = true; //
        }
        //Debug.Log($"jumpRemember = {jumpRemember}");
        //Debug.Log($"groundRemember = {groundRemember}");
        #endregion Jump

        #region Movement
        moveInput = Input.GetAxisRaw("Horizontal"); //

        var wasFacingRight = IsFacingRight; 

        if (rb2d.velocity.x > 0 && moveInput > 0)
            IsFacingRight = true;
        else if(rb2d.velocity.x < 0 && moveInput < 0)
            IsFacingRight = false;

        if (wasFacingRight != IsFacingRight)
            transform.localScale = new Vector2 (transform.localScale.x * -1, transform.localScale.y);
        //Debug.Log($"moveInput = {moveInput}");
        #endregion Movement

        #region Movement Settings
        if (Input.GetKeyDown("1"))
        {
            SetDefaultSettings();
        }
        if (Input.GetKeyDown("2"))
        {
            SetHallowKnightSettings();
        }
        if (Input.GetKeyDown("3"))
        {
            SetCelesteSettings();
        }
        #endregion
    }

    void FixedUpdate()
    {
        #region Jump
        //Jump
        
        if (jumpRemember > 0 && groundRemember > 0) //
        {
            //Debug.Log("jump");

            jumpRemember = 0; //
            groundRemember = 0; //
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce); //
        }

        //Variable Jump Height
        if (releaseJump) // 
        {
            releaseJump = false; //

            if (rb2d.velocity.y > 0) //
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * cutJumpHeight); //
            }
        }
        #endregion Jump

        #region Movement
        float targetSpeed = moveInput * moveSpeed; //

        float speedDiference = targetSpeed - rb2d.velocity.x; //

        float accelerationRate = (Mathf.Abs(targetSpeed) > .01f) ? acceleration : deceleration; //

        float movement = Mathf.Pow(Mathf.Abs(speedDiference) * accelerationRate, velocityPower) * Mathf.Sign(speedDiference); //

        if (canMove)
        {
            rb2d.AddForce(movement * Vector2.right); //
        }

        //Friction
        if (groundRemember > 0 && Mathf.Abs(moveInput) < .01f) //
        {
            float amount = Mathf.Min(Mathf.Abs(rb2d.velocity.x), Mathf.Abs(friction)); //

            amount *= Mathf.Sign(rb2d.velocity.x); //

            rb2d.AddForce(Vector2.right * -amount, ForceMode2D.Impulse); //
        }
        #endregion Movement
    }

    public void Stop()
    {
        rb2d.velocity = new Vector3(0, rb2d.velocity.y, 0);
    }

    //
    #region Settings
    
    void SetDefaultSettings()
    {
        Debug.Log("Default Settings");
        SetSettings(9f, 1.5f, 1f, 2f, .2f, 8, .35f, .1f, .2f, 1f);
    }

    void SetHallowKnightSettings()
    {
        Debug.Log("Hallowknight Settings");
        SetSettings(9, 9, 9, 1.2f, .2f, 12, .1f, .15f, .1f, 1.9f);
    }

    void SetCelesteSettings()
    {
        Debug.Log("Celeste Settings");
        SetSettings(9, 13, 16, .96f, .22f, 13, .4f, .15f, .1f, .2f);
    }

    void SetSettings(float _moveSpeed, float _acceleration, float _deceleration, float _velocityPower, float _frictionAmount, float _jumpForce, float _jumpCutMultiplier, float _groundRememberTime, float _jumpRememberTime, float _fallGravityMultiplier)
    {
        moveSpeed = _moveSpeed;
        acceleration = _acceleration;
        deceleration = _deceleration;
        velocityPower = _velocityPower;
        friction = _frictionAmount;
        jumpForce = _jumpForce;
        cutJumpHeight = _jumpCutMultiplier;
        jumpRememberTime = _jumpRememberTime;
        groundRememberTime = _groundRememberTime;
        
    }

    #endregion Settings
}
