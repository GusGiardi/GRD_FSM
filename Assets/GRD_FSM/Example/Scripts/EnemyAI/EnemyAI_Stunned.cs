using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Enemy - Stunned Behaviour", menuName = "FSM Behaviours/Enemy/Stunned", order = 1)]
    public class EnemyAI_Stunned : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myController = manager.GetComponent<EnemyAIController>();
        }

        public override void OnUpdate()
        {
            if (_myController.myWarrior.invincibilityTimeCounter > 0)
            {
                //Damage received
                _myFSM.SetBool("Stunned", false);
            }

            if (_myController.myWarrior.stunned)
                return;

            if (!_myController.isFacingPlayer)
            {
                _myController.FacePlayer();
                return;
            }

            if (_myController.playerIsInMyAttackRange)
            {
                if (!_myController.chargingAttack)
                {
                    _myController.ChargeAttack();
                }
                else
                {
                    _myController.ReleaseAttack();
                    _myFSM.SetBool("Stunned", false);
                }
            }
            else
            {
                _myFSM.SetBool("Stunned", false);
            }
        }
    }
}
