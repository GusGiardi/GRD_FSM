using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Player - In Air Behaviour", menuName = "FSM Behaviours/Player/In Air", order = 1)]
    public class Player_InAirState : FSM_StateBehaviour
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
            _myWarrior.Move(_myWarrior.controller.moveInput);
            _myWarrior.DownThrust(_myWarrior.controller.downInput);

            //Attack Controls
            if (_myWarrior.controller.attackInputDown)
            {
                _myWarrior.DownThrust(false);
                _myFSM.SetBool("AttackCharge", true);
            }
            _myWarrior.myAnimator.SetBool("DownThrust", _myWarrior.downThrust);
        }

        public override void OnExit()
        {
            _myWarrior.myAnimator.SetBool("DownThrust", false);
        }
    }
}
