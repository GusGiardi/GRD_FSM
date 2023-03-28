using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Player - Jump Recovery Behaviour", menuName = "FSM Behaviours/Player/Jump Recovery", order = 1)]
    public class Player_JumpRecoveryState : FSM_StateBehaviour
    {
        FSM_Manager _myFSM;
        WarriorScript _myWarrior;

        [SerializeField] float _jumpRecoveryTime = 0.25f;
        private float _jumpRecoveryTimeCounter;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myWarrior = manager.GetComponent<WarriorScript>();
        }

        public override void OnEnter()
        {
            _jumpRecoveryTimeCounter = _jumpRecoveryTime;
            _myWarrior.myAnimator.SetBool("Jump", true);
        }

        public override void OnUpdate()
        {
            _myWarrior.Move(0);
            _jumpRecoveryTimeCounter -= Time.deltaTime;
            if (_jumpRecoveryTimeCounter <= 0)
            {
                _myFSM.SetTrigger("JumpEnd");

            }
        }

        public override void OnExit()
        {
            _myWarrior.myAnimator.SetBool("Jump", false);
        }
    }
}
