using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Player - Stunned Behaviour", menuName = "FSM Behaviours/Player/Stunned", order = 1)]
    public class Player_StunnedState : FSM_StateBehaviour
    {
        FSM_Manager _myFSM;
        WarriorScript _myWarrior;

        [SerializeField] float _stunnedShieldRegeneration = 1.4f;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myWarrior = manager.GetComponent<WarriorScript>();
        }

        public override void OnEnter()
        {
            _myWarrior.stunned = true;
            _myWarrior.myAnimator.SetBool("Stunned", true);
        }

        public override void OnUpdate()
        {
            _myWarrior.Move(0);
            _myWarrior.currentShield += _stunnedShieldRegeneration * Time.deltaTime;
            if (_myWarrior.currentShieldNormalized == 1)
            {
                _myFSM.SetBool("Stunned", false);
            }
        }

        public override void OnExit()
        {
            _myWarrior.stunned = false;
            _myFSM.SetBool("Stunned", false);
            _myWarrior.myAnimator.SetBool("Stunned", false);
        }
    }
}
