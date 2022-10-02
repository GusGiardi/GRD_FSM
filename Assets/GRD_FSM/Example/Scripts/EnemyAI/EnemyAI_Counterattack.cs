using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Enemy AI/Counterattack")]
    public class EnemyAI_Counterattack : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        private const float _downThrustChance = 0.3f;
        private const float _downThrustChanceIterationTime = 0.5f;
        private float _downThrustChanceIterationTimeCounter = 0;
        private const float _jumpTime = 0.4f;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myController = manager.GetComponent<EnemyAIController>();
        }

        public override void OnEnter()
        {
            _downThrustChanceIterationTimeCounter = 0;
        }

        public override void OnUpdate()
        {
            if (_myController.playerCharginAttack || _myController.playerAttacking)
            {
                PlayerChargingAttackBehaviour(_myController.playerDirection);
            }
            else if (!_myController.player.onGround && _myController.inPlayerDownThrustRange)
            {
                DefendDownThrustBehaviour();
            }
            else
            {
            }
        }

        private void PlayerChargingAttackBehaviour(float playerDirection)
        {
            if (!_myController.playerIsFacingMe)
            {
                if (!_myController.playerIsInMyAttackRange)
                {
                    _myController.ReleaseAttack();
                    _myController.StopDefending();
                    _myController.CancelJump();

                    _myController.MoveTowardsPlayer();
                }
                else
                {
                    if (!_myController.chargingAttack)
                    {
                        _myController.ChargeAttack();
                    }
                    else if (Mathf.Abs(_myController.player.position.y - _myController.myWarrior.position.y) <= 1)
                    {
                        _myController.ReleaseAttack();
                    }
                }
                return;
            }

            if (_myController.inPlayerAttackRange)
            {
                if (!_myController.isFacingPlayer)
                {
                    _myController.FacePlayer();
                    return;
                }

                _myController.ReleaseAttack();
                _downThrustChanceIterationTimeCounter += Time.deltaTime;
                float downThrust = 0;
                if (_downThrustChanceIterationTimeCounter > _downThrustChanceIterationTime)
                {
                    _downThrustChanceIterationTimeCounter = 0;
                    downThrust = 1 - Random.value;
                }

                if (_myController.isOutOfStageBounds != 0 || downThrust > 1 - _downThrustChance)
                {
                    _myFSM.SetBool("AirStyle", true);
                    _myFSM.SetBool("Counterattack", false);
                }
                else
                {
                    //defend and retreat
                    _myController.Defend();
                    _myController.MoveAwayFromPlayer();
                }
            }
        }

        private void DefendDownThrustBehaviour()
        {

        }
    }
}
