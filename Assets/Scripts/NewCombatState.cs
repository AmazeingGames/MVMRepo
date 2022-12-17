using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class NewCombatState : State<CharacterManager>
{
    [SerializeField] protected float duration;
    [SerializeField] protected float jumpBufferTime;
    [SerializeField] protected float attackBufferTime;

    protected bool pressedAttack;
    protected float pressedAttackTimer;

    protected bool shouldCombo;
    protected int attackIndex;
    protected float fixedTimer;

    protected bool pressedJump;
    protected float jumpPressedTimer;

    protected Animator animator;
    protected Collider2D hitCollider;
    protected Rigidbody2D rigidbody2D;

    List<Collider2D> collidersDamaged;
    GameObject HitEffectPrefab;

    public override void Enter(CharacterManager parent)
    {
        base.Enter(parent);

        if (animator == null)
            animator = parent.GetComponent<Animator>();
        if (hitCollider == null)
            hitCollider = parent.GetComponent<PlayerCombat>().hitbox;
        if(rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();

        fixedTimer = duration;
        shouldCombo = false;
    }

    public override void CaptureInput()
    {
        pressedAttack = Input.GetMouseButtonDown(0);
        pressedJump = Input.GetButtonDown("Jump");
    }

    public override void Update()
    {
        pressedAttackTimer -= Time.deltaTime;
        jumpPressedTimer -= Time.deltaTime;

        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            DealDamage();
        }
        
        if (pressedAttack)
        {
            pressedAttackTimer = attackBufferTime;
        }

        if (pressedJump)
        {
            jumpPressedTimer = jumpBufferTime;
        }

        if (animator.GetFloat("AttackWindow.Open") > 0f && pressedAttackTimer > 0)
        {
            shouldCombo = true;
        }
    }

    public override void FixedUpdate()
    {
        fixedTimer -= Time.deltaTime;
        jumpPressedTimer -= Time.deltaTime;
    }

    protected void DealDamage()
    {
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {

            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                // Only check colliders with a valid Team Componnent attached
                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    GameObject.Instantiate(HitEffectPrefab, collidersToDamage[i].transform);
                    Debug.Log("Enemy Has Taken:" + attackIndex + "Damage");
                    collidersDamaged.Add(collidersToDamage[i]);
                }
            }
        }
    }

    public override void ChangeState()
    {
        
    }

    public override void Exit()
    {
        
    }
}
