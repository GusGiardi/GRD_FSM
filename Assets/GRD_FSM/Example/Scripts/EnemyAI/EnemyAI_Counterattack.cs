using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Enemy - Counter Attack Behaviour", menuName = "FSM Behaviours/Enemy/Counter Attack", order = 1)]
    public class EnemyAI_Counterattack : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        [SerializeField] float _minBehaviourTime = 4f;
        [SerializeField] float _maxBehaviourTime = 10f;
        private float _behaviourTime;
        private float _behaviourTimeCounter;

        [SerializeField] float _downThrustChance = 0.3f;
        [SerializeField] float _downThrustChanceIterationTime = 0.5f;
        private float _downThrustChanceIterationTimeCounter = 0;

        private float _counterattackTimeCount = 0;
        [SerializeField] float _timeToCounterAttack = 0.1f;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myController = manager.GetComponent<EnemyAIController>();
        }

        public override void OnEnter()
        {
            _behaviourTimeCounter = 0;
            _behaviourTime = Mathf.Lerp(_minBehaviourTime, _maxBehaviourTime, Random.value);

            _downThrustChanceIterationTimeCounter = 0;
            _counterattackTimeCount = 0;
        }

        public override void OnUpdate()
        {
            if (_myController.myWarrior.stunned)
            {
                _myFSM.SetBool("Counterattack", false);
                _myFSM.SetBool("Stunned", true);
                return;
            }

            if (_myController.player.stunned)
            {
                _myFSM.SetBool("Counterattack", false);
                _myFSM.SetBool("PlayerStunned", true);
                return;
            }

            if (_myController.player.invincibilityTimeCounter > 0)
            {
                _myFSM.SetBool("Counterattack", false);
                _myFSM.SetBool("Retreat", true);
                return;
            }

            CountBehaviourTime();

            if (_myController.playerAttacking &&
                _myController.playerIsInMyAttackRange)
            {
                CounterattackBehaviour();
                return;
            }

            _counterattackTimeCount = 0;

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
                DefenseBehaviour();
            }
        }

        private void PlayerChargingAttackBehaviour(float playerDirection)
        {
            if (!_myController.myWarrior.onGround)
            {
                _myController.MoveAwayFromPlayer();
                return;
            }

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
                    _myController.Jump();
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
            if (_myController.player.velocity.y < 0)
            {
                _myController.ReleaseAttack();
                _myController.UpDefense();
                _myController.MoveAwayFromPlayer();
            }
            else
            {
                _myController.StopDefending();

                if (_myController.isOutOfStageBounds > 0)
                {
                    _myController.Move(1);
                }
                else if (_myController.isOutOfStageBounds < 0)
                {
                    _myController.Move(-1);
                }
                else
                {
                    _myController.MoveAwayFromPlayer();
                }
            }
        }

        private void DefenseBehaviour()
        {
            if (!_myController.myWarrior.onGround)
            {
                _myController.MoveAwayFromPlayer();
                return;
            }

            if (!_myController.inPlayerAttackRange)
            {
                _myController.ReleaseAttack();
                _myController.StopDefending();

                _myController.MoveTowardsPlayer();
            }
            else
            {
                if (!_myController.isFacingPlayer)
                {
                    _myController.FacePlayer();
                    return;
                }
                _myController.Defend();
                _myController.MoveAwayFromPlayer();
            }
        }

        private void CounterattackBehaviour()
        {
            if (_counterattackTimeCount < _timeToCounterAttack)
            {
                if (!_myController.isFacingPlayer)
                {
                    _myController.FacePlayer();
                    return;
                }
                _myController.Defend();
                _myController.MoveTowardsPlayer();

                _counterattackTimeCount += Time.deltaTime;
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
        }

        private void CountBehaviourTime()
        {
            _behaviourTimeCounter += Time.deltaTime;
            if (_behaviourTimeCounter >= _behaviourTime)
            {
                _myFSM.SetBool("Counterattack", false);
            }
        }
    }
}
