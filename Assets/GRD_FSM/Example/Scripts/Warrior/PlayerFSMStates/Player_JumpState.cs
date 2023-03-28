using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Player - Jump Behaviour", menuName = "FSM Behaviours/Player/Jump", order = 1)]
    public class Player_JumpState : FSM_StateBehaviour
    {
        FSM_Manager _myFSM;
        WarriorScript _myWarrior;

        [SerializeField] float _jumpAntecipationTime = 0.1f;
        private float _jumpAntecipationTimeCounter;
        private bool _jumpCanceled;

        enum JumpStep
        {
            Antecipation,
            Jump
        }
        private JumpStep _currentJumpStep;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myWarrior = manager.GetComponent<WarriorScript>();
        }

        public override void OnEnter()
        {
            _jumpAntecipationTimeCounter = _jumpAntecipationTime;
            _jumpCanceled = false;
            _currentJumpStep = JumpStep.Antecipation;

            _myWarrior.myAnimator.SetBool("Jump", true);
        }

        public override void OnUpdate()
        {
            _myWarrior.Move(_myWarrior.controller.moveInput);

            switch (_currentJumpStep)
            {
                case JumpStep.Antecipation:
                    JumpAntecipationUpdate();
                    break;
                case JumpStep.Jump:
                    JumpUpdate();
                    break;
            }

            //Attack Controls
            if (_myWarrior.controller.attackInputDown)
            {
                _myWarrior.DownThrust(false);
                _myWarrior.myAnimator.SetBool("DownThrust", false);
                _myFSM.SetBool("AttackCharge", true);
            }
        }

        public override void OnFixedUpdate()
        {
            if (_currentJumpStep == JumpStep.Jump)
            {
                if (_myWarrior.rb.velocity.y > 0 && _jumpCanceled)
                {
                    _myWarrior.rb.velocity += Vector2.down * _myWarrior.cancelJumpForce * Time.fixedDeltaTime;
                }
            }
        }

        private void JumpAntecipationUpdate()
        {
            _jumpAntecipationTimeCounter -= Time.deltaTime;
            if (_jumpAntecipationTimeCounter <= 0)
            {
                _myWarrior.Jump();
                _currentJumpStep = JumpStep.Jump;
                _myWarrior.onGround = false;
            }
        }

        private void JumpUpdate()
        {
            _myWarrior.DownThrust(_myWarrior.controller.downInput);
            _myWarrior.myAnimator.SetBool("DownThrust", _myWarrior.downThrust);

            if (!_myWarrior.controller.jumpInput)
            {
                _jumpCanceled = true;
            }

            if (_myWarrior.rb.velocity.y <= 0)
            {
                _myFSM.SetTrigger("JumpEnd");
            }
        }

        public override void OnExit()
        {
            _myWarrior.myAnimator.SetBool("Jump", false);
        }
    }
}
