using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FSM_Behaviour("Player/Attack")]
public class Player_AttackState : FSM_StateBehaviour
{
    FSM_Manager _myFSM;
    WarriorScript _myWarrior;
    Animator _myAnimator;

    private float _attackCooldownTimeCounter;

    public override void Setup(FSM_Manager manager)
    {
        _myFSM = manager;
        _myWarrior = manager.GetComponent<WarriorScript>();
        _myAnimator = manager.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        _attackCooldownTimeCounter = _myWarrior.attackCooldownTime;
        _myAnimator.SetBool("Attack", true);
    }

    public override void OnUpdate()
    {
        _myWarrior.Move(0);
        _attackCooldownTimeCounter -= Time.deltaTime;
        if (_attackCooldownTimeCounter <= 0)
        {
            _myFSM.SetBool("Attack", false);
        }
    }

    public override void OnExit()
    {
        _myAnimator.SetBool("Attack", false);
        _myWarrior.EndAttack();
    }
}
