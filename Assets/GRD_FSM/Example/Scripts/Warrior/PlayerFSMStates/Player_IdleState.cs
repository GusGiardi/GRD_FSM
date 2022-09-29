using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Player/Idle")]
    public class Player_IdleState : FSM_StateBehaviour
    {
        FSM_Manager _myFSM;
        WarriorScript _myWarrior;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myWarrior = manager.GetComponent<WarriorScript>();
        }

        public override void OnUpdate()
        {
            if (_myWarrior.controller.jumpInputDown)
            {
                _myFSM.SetTrigger("Jump");
            }

            //Defense Controls
            _myWarrior.Defense(_myWarrior.controller.defenseInput);
            _myWarrior.UpDefense(_myWarrior.controller.upInput);

            //Attack Controls
            if (_myWarrior.controller.attackInputDown)
            {
                _myFSM.SetBool("AttackCharge", true);
            }

            _myWarrior.Move(_myWarrior.controller.moveInput);
        }

        public override void OnExit()
        {
            _myWarrior.Defense(false);
        }
    }
}
