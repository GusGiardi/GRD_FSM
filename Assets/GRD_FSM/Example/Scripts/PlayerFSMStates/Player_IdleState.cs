using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FSM_Behaviour("Player/Idle")]
public class Player_IdleState : FSM_StateBehaviour
{
    FSM_Manager _myFSM;
    WarriorScript _myWarrior;

    public override void Setup(FSM_Manager manager)
    {
        _myFSM = manager;
        _myWarrior = manager.GetComponent<WarriorScript>();
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            _myFSM.SetTrigger("Jump");
        }

        //Defense Controls
        _myWarrior.Defense(Input.GetKey(KeyCode.Z));
        _myWarrior.UpDefense(Input.GetKey(KeyCode.UpArrow));

        //Attack Controls
        if (Input.GetKeyDown(KeyCode.C))
        {
            _myFSM.SetBool("AttackCharge", true);
        }

        _myWarrior.Move(Input.GetAxis("Horizontal"));
    }

    public override void OnExit()
    {
        _myWarrior.Defense(false);
    }
}
