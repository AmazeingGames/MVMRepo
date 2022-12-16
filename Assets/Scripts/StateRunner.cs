using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace StateMachine
{
    public abstract class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
    {
        bool ranFixedUpdate = false;
        bool ranUpdate = false;

        [SerializeField] List<State<T>> states;
        public State<T> CurrentState { get; private set; }

        protected virtual void Awake()
        {
            Debug.Log("Ran Awake");

            SetState(states[0].GetType());
        }

        public void SetState(Type newStateType)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            CurrentState = states.First(s => s.GetType() == newStateType);
            CurrentState.Enter(parent: GetComponent<T>());
        }

        void Update()
        {
            if (!ranUpdate)
            {
                Debug.Log("Ran Update");
                ranUpdate = true;
            }

            CurrentState.CaptureInput();
            CurrentState.Update();
            CurrentState.ChangeState();
        }

        void FixedUpdate()
        {
            if (!ranFixedUpdate)
            {
                Debug.Log("Ran Fixed Update");
                ranFixedUpdate = true;
            }

            CurrentState.FixedUpdate();
        }
    }
} 