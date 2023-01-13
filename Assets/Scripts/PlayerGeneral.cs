using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class PlayerGeneral : MonoBehaviour
{
    [SerializeField] float timeBetweenLedgeGrabs;
    public float ledgeGrabTimer { get; set; } = -1;

    public bool IsFacingRight { get; private set; }

    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int LedgeHang = Animator.StringToHash("LedgeHang");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int Run = Animator.StringToHash("Run");

    public State<CharacterManager> lastState { get; private set; }

    Rigidbody2D rigidBody2D;
    CharacterManager characterManager;

    void Awake()
    {
        if (rigidBody2D == null)
            rigidBody2D = GetComponent<Rigidbody2D>();
        if (characterManager == null) 
            characterManager = GetComponent<CharacterManager>();
    }

    private void Start()
    {
        lastState = characterManager.CurrentState;
    }

    private void Update()
    {
        ledgeGrabTimer -= Time.deltaTime;

        //Move this into states themself to avoid any unwanted flippage
        if ((IsFacingRight && transform.localScale.x < 0) || !IsFacingRight && transform.localScale.x > 0)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
        
        if (lastState.GetType() != characterManager.CurrentState.GetType())
        {
            if (lastState.GetType() == typeof(LedgeHold) && characterManager.CurrentState.GetType() == typeof(WalkState))
            {
                ledgeGrabTimer = timeBetweenLedgeGrabs;
            }

            lastState = characterManager.CurrentState;
        }

        if (ledgeGrabTimer < 0)
        {
            ledgeGrabTimer = 0;
        }

        //Debug.Log($"is LedgeGrabTimer greater than 0 : {ledgeGrabTimer > 0}");

    }

    void FixedUpdate()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && rigidBody2D.velocity.x >= 0)
        {
            IsFacingRight = true;
        }

        if (Input.GetAxisRaw("Horizontal") < 0 && rigidBody2D.velocity.x <= 0)
        {
            IsFacingRight = false;
        }
    }
}
