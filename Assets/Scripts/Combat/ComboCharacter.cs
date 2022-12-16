using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCharacter : MonoBehaviour
{

    [SerializeField] public Collider2D hitbox;
    [SerializeField] public GameObject Hiteffect;

    CombatStateMachine meleeStateMachine;
    StateRunner<CharacterManager> parentStateRunner;

    public Action onAttackStart;
    public Action onAttackEnd;

    // Start is called before the first frame update
    void Start()
    {
        meleeStateMachine = GetComponent<CombatStateMachine>();
        parentStateRunner = GetComponent<StateRunner<CharacterManager>>();
    }

    // Update is called once per frame
    void Update()
    {
        //Enters Combat
        if (Input.GetMouseButton(0) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState) && parentStateRunner.CurrentState.type != State<CharacterManager>.EStateType.Jump)
        {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
    }
}
