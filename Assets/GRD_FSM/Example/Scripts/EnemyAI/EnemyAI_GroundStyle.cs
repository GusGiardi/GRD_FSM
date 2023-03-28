using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Enemy - Ground Style Behaviour", menuName = "FSM Behaviours/Enemy/Ground Style", order = 1)]
    public class EnemyAI_GroundStyle : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        [SerializeField] float _minBehaviourTime = 4f;
        [SerializeField] float _maxBehaviourTime = 10f;
        private float _behaviourTime;
        private float _behaviourTimeCounter;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myController = manager.GetComponent<EnemyAIController>();
        }

        public override void OnEnter()
        {
            _behaviourTimeCounter = 0;
            _behaviourTime = Mathf.Lerp(_minBehaviourTime, _maxBehaviourTime, Random.value);
        }

        public override void OnUpdate()
        {
            if (_myController.myWarrior.stunned)
            {
                _myFSM.SetBool("GroundStyle", false);
                _myFSM.SetBool("Stunned", true);
                return;
            }

            if (_myController.player.stunned)
            {
                _myFSM.SetBool("GroundStyle", false);
                _myFSM.SetBool("PlayerStunned", true);
                return;
            }

            if (_myController.player.invincibilityTimeCounter > 0)
            {
                _myFSM.SetBool("GroundStyle", false);
                _myFSM.SetBool("Retreat", true);
                return;
            }

            CountBehaviourTime();

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

            if (!_myController.myWarrior.onGround)
            {
                _myController.MoveAwayFromPlayer();
                _myController.DownThrust();
                return;
            }

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
            if (!_myController.myWarrior.onGround)
            {
                _myController.MoveTowardsPlayer();
                _myController.DownThrust();
                return;
            }

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

        private void CountBehaviourTime()
        {
            _behaviourTimeCounter += Time.deltaTime;
            if (_behaviourTimeCounter >= _behaviourTime)
            {
                _myFSM.SetBool("GroundStyle", false);
            }
        }
    }
}
