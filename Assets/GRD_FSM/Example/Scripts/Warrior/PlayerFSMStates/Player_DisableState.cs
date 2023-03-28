using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Player - Disable Behaviour", menuName = "FSM Behaviours/Player/Disable", order = 1)]
    public class Player_DisableState : FSM_StateBehaviour
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
            _myFSM.SetBool("AttackCharge", false);
            _myFSM.SetBool("Attack", false);
            _myFSM.SetBool("Stunned", false);
        }

        public override void OnUpdate()
        {
            _myWarrior.Move(0);
            _myWarrior.DownThrust(false);
            _myWarrior.myAnimator.SetBool("DownThrust", _myWarrior.downThrust);
        }
    }
}
