using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FSM_Behaviour("Player/AttackCharge")]
public class Player_AttackCharge : FSM_StateBehaviour
{
    FSM_Manager _myFSM;
    WarriorScript _myWarrior;
    Animator _myAnimator;

    public override void Setup(FSM_Manager manager)
    {
        _myFSM = manager;
        _myWarrior = manager.GetComponent<WarriorScript>();
        _myAnimator = manager.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        _myAnimator.SetBool("AttackCharge", true);
    }

    public override void OnExit()
    {
        _myAnimator.SetBool("AttackCharge", false);
        _myFSM.SetBool("AttackCharge", false);
    }

    public override void OnUpdate()
    {
        _myWarrior.Move(0);
        _myWarrior.ChargeAttack();
        if (!Input.GetKey(KeyCode.C))
        {
            _myFSM.SetBool("Attack", true);
        }
    }
}
