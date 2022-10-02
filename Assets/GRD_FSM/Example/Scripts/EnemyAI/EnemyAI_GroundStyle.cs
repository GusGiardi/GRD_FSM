using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Enemy AI/Ground Style")]
    public class EnemyAI_GroundStyle : FSM_StateBehaviour
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
            if (_myController.playerIsFacingMe && (_myController.playerCharginAttack || _myController.playerAttacking))
            {
                PlayerChargingAttackBehaviour(_myController.playerDirection);
            }
            else if (!_myController.player.onGround && _myController.inPlayerDownThrustRange)
            {
                DefendDownThrustBehaviour();
            }
            else
            {
                AttackBehaviour(_myController.playerDirection);
            }
        }

        private void PlayerChargingAttackBehaviour(float playerDirection)
        {
            _myController.ReleaseAttack();

            if (_myController.inPlayerAttackRange)
            {
                //Looking at wrong direction
                if (!_myController.isFacingPlayer)
                {
                    _myController.FacePlayer();
                    return;
                }

                //defend and retreat
                _myController.Defend();
                _myController.MoveAwayFromPlayer();
            }
            else
            {
                if (!_myController.isFacingPlayer)
                {
                    _myController.FacePlayer();
                    return;
                }

                _myController.StopMoving();
                _myController.StopDefending();
            }
        }

        private void DefendDownThrustBehaviour()
        {
            _myController.ReleaseAttack();
            _myController.UpDefense();
            _myController.MoveAwayFromPlayer();
        }

        private void AttackBehaviour(float playerDirection)
        {
            if (!_myController.playerIsInMyAttackRange)
            {
                _myController.StopDefending();
                _myController.MoveTowardsPlayer();
            }
            else
            {
                if (!_myController.chargingAttack)
                {
                    _myController.ChargeAttack();
                }
                else if (_myController.attackCharge >= _myController.weakAttackCharge)
                {
                    _myController.ReleaseAttack();
                }
            }
        }
    }
}
