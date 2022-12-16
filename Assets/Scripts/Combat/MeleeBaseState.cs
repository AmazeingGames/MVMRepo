using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : CombatState
{
    // How long this state should be active for before moving on
    public float duration;

    protected Animator animator;
    protected ComboCharacter comboCharacter;

    // bool to check whether or not the next attack in the sequence should be played or not
    protected bool shouldCombo;

    // The attack index in the sequence of attacks
    protected int attackIndex;

    protected Collider2D hitCollider;

    private List<Collider2D> collidersDamaged;

    private GameObject HitEffectPrefab;

    // Input buffer Timer
    private float AttackPressedTimer = 0;

    public override void OnEnter(CombatStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        comboCharacter = GetComponent<ComboCharacter>();
        animator = GetComponent<Animator>();

        collidersDamaged = new List<Collider2D>();
        hitCollider = comboCharacter.hitbox;
        HitEffectPrefab = comboCharacter.Hiteffect;
        comboCharacter.onAttackStart?.Invoke();

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;

        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }

        //Determines combo
        if (Input.GetMouseButtonDown(0))
        {
            AttackPressedTimer = 1;
        }

        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0)
        {
            shouldCombo = true;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        
        comboCharacter.onAttackEnd?.Invoke();

        //Debug.Log("exited combat");
    }

    //Deals damage
    protected void Attack()
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

}
