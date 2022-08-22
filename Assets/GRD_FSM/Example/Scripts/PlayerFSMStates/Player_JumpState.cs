using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FSM_Behaviour("Player/Jump")]
public class Player_JumpState : FSM_StateBehaviour
{
    FSM_Manager _myFSM;
    WarriorScript _myWarrior;
    Animator _myAnimator;

    private float _jumpAntecipationTimeCounter;
    private float _jumpRecoveryTimeCounter;
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
        _myAnimator = manager.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        _jumpAntecipationTimeCounter = _myWarrior.jumpAntecipationTime;
        _jumpRecoveryTimeCounter = _myWarrior.jumpRecoveryTime;
        _jumpCanceled = false;
        _currentJumpStep = JumpStep.Antecipation;
    }

    public override void OnUpdate()
    {
        _myWarrior.Move(Input.GetAxis("Horizontal"));

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
        if (Input.GetKeyDown(KeyCode.C))
        {
            _myWarrior.DownThrust(false);
            _myAnimator.SetBool("DownThrust", false);
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
        _myWarrior.DownThrust(Input.GetKey(KeyCode.DownArrow));
        _myAnimator.SetBool("DownThrust", _myWarrior.downThrust);

        if (!Input.GetKey(KeyCode.X))
        {
            _jumpCanceled = true;
        }

        if (_myWarrior.rb.velocity.y <= 0)
        {
            _myFSM.SetTrigger("JumpEnd");
        }
    }
}
