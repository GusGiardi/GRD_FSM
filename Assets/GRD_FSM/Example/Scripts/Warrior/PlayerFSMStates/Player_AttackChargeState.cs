using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Player/AttackCharge")]
    public class Player_AttackCharge : FSM_StateBehaviour
    {
        FSM_Manager _myFSM;
        WarriorScript _myWarrior;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myWarrior = manager.GetComponent<WarriorScript>();
        }

        public override void OnEnter()
        {
            _myWarrior.myAnimator.SetBool("AttackCharge", true);
        }

        public override void OnExit()
        {
            _myWarrior.myAnimator.SetBool("AttackCharge", false);
            _myFSM.SetBool("AttackCharge", false);
        }

        public override void OnUpdate()
        {
            _myWarrior.Move(0);
            _myWarrior.ChargeAttack();
            if (!_myWarrior.controller.attackInput)
            {
                _myFSM.SetBool("Attack", true);
            }
        }
    }
}
