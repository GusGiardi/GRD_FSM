using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Player/Damage")]
    public class Player_DamageState : FSM_StateBehaviour
    {
        FSM_Manager _myFSM;
        WarriorScript _myWarrior;

        private float _damageTimeCounter;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myWarrior = manager.GetComponent<WarriorScript>();
        }

        public override void OnEnter()
        {
            _myWarrior.myAnimator.SetBool("Damage", true);
            _myWarrior.myAnimator.SetBool("DownThrust", false);

            _damageTimeCounter = _myWarrior.damageTime;
            _myWarrior.currentVelocity = 0;
            _myWarrior.takingDamage = true;
            _myWarrior.CancelAttackCharge();
        }

        public override void OnUpdate()
        {
            _damageTimeCounter -= Time.deltaTime;
            if (_damageTimeCounter <= 0)
            {
                _myFSM.SetBool("Damage", false);
                _myWarrior.myAnimator.SetBool("Damage", false);
            }
        }

        public override void OnExit()
        {
            _myWarrior.takingDamage = false;
            _myWarrior.invincibilityTimeCounter = _myWarrior.invincibilityTime;
        }
    }
}
