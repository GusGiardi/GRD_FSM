using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Player/Stunned")]
    public class Player_StunnedState : FSM_StateBehaviour
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
            _myWarrior.myAnimator.SetBool("Stunned", true);
        }

        public override void OnUpdate()
        {
            _myWarrior.Move(0);
            _myWarrior.currentShield += _myWarrior.stunnedShieldRegeneration * Time.deltaTime;
            if (_myWarrior.currentShieldNormalized == 1)
            {
                _myFSM.SetBool("Stunned", false);
            }
        }

        public override void OnExit()
        {
            _myFSM.SetBool("Stunned", false);
            _myWarrior.myAnimator.SetBool("Stunned", false);
        }
    }
}
