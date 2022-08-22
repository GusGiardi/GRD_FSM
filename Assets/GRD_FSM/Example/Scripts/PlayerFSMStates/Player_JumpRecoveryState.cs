using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FSM_Behaviour("Player/JumpRecovery")]
public class Player_JumpRecovery : FSM_StateBehaviour
{
    FSM_Manager _myFSM;
    WarriorScript _myWarrior;

    private float _jumpRecoveryTimeCounter;

    public override void Setup(FSM_Manager manager)
    {
        _myFSM = manager;
        _myWarrior = manager.GetComponent<WarriorScript>();
    }

    public override void OnEnter()
    {
        _jumpRecoveryTimeCounter = _myWarrior.jumpRecoveryTime;
    }

    public override void OnUpdate()
    {
        _myWarrior.Move(0);
        _jumpRecoveryTimeCounter -= Time.deltaTime;
        if (_jumpRecoveryTimeCounter <= 0)
        {
            _myFSM.SetTrigger("JumpEnd");

        }
    }
}
