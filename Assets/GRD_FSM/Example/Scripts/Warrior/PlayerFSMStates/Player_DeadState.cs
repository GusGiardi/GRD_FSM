using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Player/Dead")]
    public class Player_DeadState : FSM_StateBehaviour
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
            _myWarrior.myAnimator.SetBool("Dead", true);
            _myWarrior.myAnimator.SetBool("DownThrust", false);
        }

        public override void OnUpdate()
        {
            _myWarrior.Move(0);
            if (_myWarrior.currentHP > 0)
            {
                _myFSM.SetBool("Dead", false);
            }
        }

        public override void OnExit()
        {
            _myWarrior.myAnimator.SetBool("Dead", false);
        }
    }
}
