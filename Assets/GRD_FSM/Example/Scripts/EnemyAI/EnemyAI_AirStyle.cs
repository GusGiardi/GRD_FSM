using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Enemy - Air Style Behaviour", menuName = "FSM Behaviours/Enemy/Air Style", order = 1)]
    public class EnemyAI_AirStyle : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        [SerializeField] float _minBehaviourTime = 4f;
        [SerializeField] float _maxBehaviourTime = 6f;
        private float _behaviourTime;
        private float _behaviourTimeCounter;

        [SerializeField] float _minJumpTime = 0.1f;
        [SerializeField] float _maxJumpTime = 3f;

        private bool _jump;

        private enum BehaviourState
        {
            Retreat,
            Attack
        }
        private BehaviourState _behaviourState;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myController = manager.GetComponent<EnemyAIController>();
        }

        public override void OnEnter()
        {
            _behaviourTimeCounter = 0;
            _behaviourTime = Mathf.Lerp(_minBehaviourTime, _maxBehaviourTime, Random.value);

            _behaviourState = BehaviourState.Retreat;
            _jump = false;
        }

        public override void OnUpdate()
        {
            if (_myController.myWarrior.stunned)
            {
                _myFSM.SetBool("AirStyle", false);
                _myFSM.SetBool("Stunned", true);
                return;
            }

            if (_myController.player.stunned)
            {
                _myFSM.SetBool("AirStyle", false);
                _myFSM.SetBool("PlayerStunned", true);
                return;
            }

            if (_myController.player.invincibilityTimeCounter > 0)
            {
                _myFSM.SetBool("AirStyle", false);
                _myFSM.SetBool("Retreat", true);
                return;
            }

            CountBehaviourTime();

            switch (_behaviourState)
            {
                case BehaviourState.Retreat:
                    RetreatBehaviour();
                    break;
                case BehaviourState.Attack:
                    AttackBehaviour();
                    break;
            }
        }

        private void RetreatBehaviour()
        {
            _myController.ReleaseAttack();
            _myController.StopDefending();

            _myController.MoveAwayFromPlayer();

            if (_myController.inPlayerAttackRange &&
                (_myController.playerCharginAttack || _myController.playerAttacking) &&
                _myController.myWarrior.position.y - _myController.player.position.y < 2)
            {
                _myController.Jump();

                _myController.Invoke("CancelJump", _maxJumpTime);
                return;
            }

            if (_myController.isOutOfStageBounds != 0 || 
                Mathf.Abs(_myController.playerDirection) > _myController.maxRetreatDistance)
            {
                _behaviourState = BehaviourState.Attack;
            }
        }

        private void AttackBehaviour()
        {
            if (_myController.inPlayerAttackRange &&
                _myController.playerIsFacingMe && 
                (_myController.playerCharginAttack || _myController.playerAttacking) &&
                _myController.myWarrior.position.y - _myController.player.position.y < 2)
            {
                PlayerChargingAttackBehaviour(_myController.playerDirection);
                return;
            }

            if (_myController.myWarrior.onGround)
            {
                if (_jump)
                {
                    _behaviourState = BehaviourState.Retreat;
                    _jump = false;
                    return;
                }

                if (!_myController.player.onGround && 
                    Mathf.Abs(_myController.playerDirection) <= Mathf.Abs(_myController.player.velocity.x) + 1)
                {
                    DefendDownThrustBehaviour();
                    return;
                }

                _myController.ReleaseAttack();
                _myController.StopDefending();

                _myController.MoveTowardsPlayer();

                if (_myController.playerDirection * _myController.myWarrior.velocity.x > 0 &&
                    Mathf.Abs(_myController.playerDirection) < _myController.maxRetreatDistance &&
                    Mathf.Abs(_myController.myWarrior.velocity.x) > Mathf.Abs(_myController.playerDirection))
                {
                    _myController.Jump();
                    if (_myController.playerIsInMyAttackRange)
                    {
                        _myController.Invoke("CancelJump", _minJumpTime);
                    }
                    else
                    {
                        _myController.Invoke("CancelJump", _maxJumpTime);
                    }
                }
            }
            else
            {
                _jump = true;

                _myController.ReleaseAttack();
                _myController.StopDefending();

                if (Mathf.Abs(_myController.playerDirection) > _myController.playerAttackDistance)
                {
                    if (Mathf.Abs(_myController.myWarrior.velocity.x) < Mathf.Abs(_myController.playerDirection))
                    {
                        _myController.MoveTowardsPlayer();
                    }
                    else
                    {
                        _myController.StopMoving();
                    }
                }
                else
                {
                    _myController.StopMoving();
                }
                _myController.DownThrust();
            }
        }

        private void PlayerChargingAttackBehaviour(float playerDirection)
        {
            //Looking at wrong direction
            if (!_myController.isFacingPlayer)
            {
                _myController.FacePlayer();
                return;
            }

            if (_myController.myWarrior.onGround)
            {
                _myController.ReleaseAttack();

                if (_myController.isOutOfStageBounds != 0)
                {
                    _myController.Jump();
                    _myController.Move(-_myController.isOutOfStageBounds);
                    if (_myController.playerIsInMyAttackRange)
                    {
                        _myController.Invoke("CancelJump", _minJumpTime);
                    }
                    else
                    {
                        _myController.Invoke("CancelJump", _maxJumpTime);
                    }
                }
                else
                {
                    //defend and retreat
                    _myController.Defend();
                    _myController.MoveAwayFromPlayer();
                }
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

        private void CountBehaviourTime()
        {
            _behaviourTimeCounter += Time.deltaTime;
            if (_behaviourTimeCounter >= _behaviourTime)
            {
                _myFSM.SetBool("AirStyle", false);
            }
        }
    }
}
