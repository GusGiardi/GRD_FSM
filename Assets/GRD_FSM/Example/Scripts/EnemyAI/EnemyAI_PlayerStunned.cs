using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Enemy - Player Stunned Behaviour", menuName = "FSM Behaviours/Enemy/Player Stunned", order = 1)]
    public class EnemyAI_PlayerStunned : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        [SerializeField] float _maxPlayerShield = 0.9f;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myController = manager.GetComponent<EnemyAIController>();
        }

        public override void OnUpdate()
        {
            if (!_myController.player.stunned)
            {
                _myFSM.SetBool("PlayerStunned", false);
                return;
            }

            if (!_myController.myWarrior.onGround)
            {
                if (Mathf.Abs(_myController.playerDirection) < _myController.playerAttackDistance)
                {
                    _myController.MoveAwayFromPlayer();
                }
                else
                {
                    _myController.StopMoving();
                }
            }
            else
            {
                if (_myController.playerIsInMyAttackRange)
                {
                    if (!_myController.chargingAttack)
                    {
                        _myController.ChargeAttack();
                    }
                    else if (_myController.myWarrior.currentAttackCharge == 1 ||
                        _myController.player.currentShieldNormalized >= _maxPlayerShield)
                    {
                        _myController.ReleaseAttack();
                        _myFSM.SetBool("PlayerStunned", false);
                    }
                    return;
                }

                _myController.MoveTowardsPlayer();
            }
        }
    }
}
