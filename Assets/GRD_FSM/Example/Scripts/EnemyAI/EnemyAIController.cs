using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [DefaultExecutionOrder(-1)]
    public class EnemyAIController : WarriorInputController
    {
        [Header("My Managers")]
        [SerializeField] WarriorScript _myWarrior;
        [SerializeField] FSM_Manager _AI_FSM;
        [SerializeField] FSM_Manager _myWarriorFSM;

        [Header("Player")]
        [SerializeField] WarriorScript _player;

        [Header("Position Parameters")]
        [SerializeField] float _playerAttackDistance;
        [SerializeField] float _playerDownThrustHeight;
        [SerializeField] float _maxRetreatDistance;
        [SerializeField] Vector2 _stageBounds;

        [Header("Attack Parameters")]
        [SerializeField] float _weakAttackCharge = 0.2f;
        [SerializeField] float _strongAttackCharge = 0.7f;

        public WarriorScript myWarrior => _myWarrior;
        public WarriorScript player => _player;

        public float playerDirection => _player.position.x - _myWarrior.position.x;
        public float playerAttackDistance => _playerAttackDistance;
        public bool isFacingPlayer => (Mathf.Sign(playerDirection) < 0 ^ _myWarrior.facingRight);
        public bool playerIsFacingMe => (Mathf.Sign(playerDirection) > 0 ^ _player.facingRight);
        public bool inPlayerAttackRange =>
            Mathf.Abs(playerDirection) <= playerAttackDistance
                &&
            playerIsFacingMe;
        public bool playerIsInMyAttackRange =>
            Mathf.Abs(playerDirection) <= playerAttackDistance * 0.75f
                &&
            isFacingPlayer;
        public bool inPlayerDownThrustRange =>
            Mathf.Abs(playerDirection) <= playerAttackDistance
                &&
            _player.position.y > _myWarrior.position.y + _playerDownThrustHeight;
        public bool playerIsInMyDownThrustRange =>
            Mathf.Abs(playerDirection) <= playerAttackDistance
                &&
            _myWarrior.position.y > _player.position.y + _playerDownThrustHeight;
        public float maxRetreatDistance => _maxRetreatDistance;
        public int isOutOfStageBounds
        {
            get
            {
                if (_myWarrior.position.x < _stageBounds.x)
                    return -1;
                if (_myWarrior.position.x > _stageBounds.y)
                    return 1;
                return 0;
            }
        }

        public bool playerCharginAttack => _player.currentAttackCharge > 0;
        public bool playerAttacking => _player.attacking;
        public bool chargingAttack => _myWarriorFSM.GetCurrentStateId() == _chargingAttackStateId;
        public float attackCharge => _myWarrior.currentAttackCharge;
        public float weakAttackCharge => _weakAttackCharge;
        public float strongAttackCharge => _strongAttackCharge;

        #region FSM State IDs
        private int _idleStateId;
        private int _jumpStateId;
        private int _inAirStateId;
        private int _jumpRecoveryStateId;
        private int _attackStateId;
        private int _chargingAttackStateId;
        private int _damageStateId;
        private int _stunnedStateId;
        #endregion

        #region Input Press
        List<Action> inputList = new List<Action>();

        public void MoveInput(float direction)
        {
            _moveInput = direction;
        }

        public void PressAttack(bool press)
        {
            if (_attackInput != press)
            {
                if (press)
                {
                    _attackInputDown = true;
                }
                else
                    _attackInputUp = true;
            }
            _attackInput = press;
        }

        public void PressJump(bool press)
        {
            if (_jumpInput != press)
            {
                if (press)
                    _jumpInputDown = true;
                else
                    _jumpInputUp = true;
            }
            _jumpInput = press;
        }

        public void PressDefense(bool press)
        {
            if (_defenseInput != press)
            {
                if (press)
                    _defenseInputDown = true;
                else
                    _defenseInputUp = true;
            }
            _defenseInput = press;
        }

        public void UpInput(bool press)
        {
            _upInput = press;
        }

        public void DownInput(bool press)
        {
            _downInput = press;
        }

        public void AddInput(Action inputAction)
        {
            inputList.Add(inputAction);
        }

        public void ExecuteInputs()
        {
            foreach (Action input in inputList)
                input();
            inputList.Clear();
        }

        private void Update()
        {
            ExecuteInputs();
        }

        private void LateUpdate()
        {
            _attackInputDown = false;
            _attackInputUp = false;

            _jumpInputDown = false;
            _jumpInputUp = false;

            _defenseInputDown = false;
            _defenseInputUp = false;
        }
        #endregion

        private void Awake()
        {
            _idleStateId = _myWarriorFSM.GetStateIdByName("Idle");
            _jumpStateId = _myWarriorFSM.GetStateIdByName("Jump");
            _inAirStateId = _myWarriorFSM.GetStateIdByName("InAir");
            _jumpRecoveryStateId = _myWarriorFSM.GetStateIdByName("Jump Recovery");
            _attackStateId = _myWarriorFSM.GetStateIdByName("Attack");
            _chargingAttackStateId = _myWarriorFSM.GetStateIdByName("Charging Attack");
            _damageStateId = _myWarriorFSM.GetStateIdByName("Damage");
            _stunnedStateId = _myWarriorFSM.GetStateIdByName("Stunned");
        }

        #region Behaviour Inputs
        public void FacePlayer()
        {
            AddInput(() => PressAttack(false));
            AddInput(() => PressDefense(false));

            AddInput(() => MoveInput(Mathf.Sign(playerDirection)));
        }

        public void MoveTowardsPlayer()
        {
            AddInput(() => PressAttack(false));
            AddInput(() => MoveInput(Mathf.Sign(playerDirection)));
        }

        public void MoveAwayFromPlayer()
        {
            AddInput(() => PressAttack(false));
            AddInput(() => MoveInput(-Mathf.Sign(playerDirection)));
        }

        public void Move(float direction)
        {
            AddInput(() => MoveInput(-Mathf.Sign(direction)));
        }

        public void StopMoving()
        {
            AddInput(() => MoveInput(0));
        }

        public void Jump()
        {
            int currentStateId = _myWarriorFSM.GetCurrentStateId();
            if (currentStateId == _idleStateId ||
                currentStateId == _jumpStateId ||
                currentStateId == _inAirStateId)
            {
                AddInput(() => PressJump(true));
            }
            else
            {
                AddInput(() => PressJump(false));
            }
        }

        public void CancelJump()
        {
            AddInput(() => PressJump(false));
        }

        public void ChargeAttack()
        {
            int currentStateId = _myWarriorFSM.GetCurrentStateId();
            if (currentStateId == _idleStateId ||
                currentStateId == _jumpStateId ||
                currentStateId == _inAirStateId ||
                currentStateId == _chargingAttackStateId)
            {
                AddInput(() => PressAttack(true));
            }
            else
            {
                AddInput(() => PressAttack(false));
            }
        }

        public void ReleaseAttack()
        {
            AddInput(() => PressAttack(false));
        }

        public void DownThrust()
        {
            if (!_myWarrior.onGround)
            {
                AddInput(() => DownInput(true));
            }
            else
            {
                AddInput(() => DownInput(false));
            }
        }

        public void Defend()
        {
            AddInput(() => PressDefense(true));
            AddInput(() => UpInput(false));
        }

        public void UpDefense()
        {
            AddInput(() => PressDefense(true));
            AddInput(() => UpInput(true));
        }

        public void StopDefending()
        {
            AddInput(() => PressDefense(false));
        }

        public void StopAllCommands()
        {
            StopMoving();
            CancelJump();
            ReleaseAttack();
            StopDefending();

            AddInput(() => UpInput(false));
            AddInput(() => DownInput(false));
        }
        #endregion
    }
}
