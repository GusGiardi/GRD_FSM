using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    public class PlayerInputController : WarriorInputController
    {
        void Update()
        {
            _moveInput = Input.GetAxis("Horizontal");

            _attackInput = Input.GetKey(KeyCode.C);
            _attackInputDown = Input.GetKeyDown(KeyCode.C);
            _attackInputUp = Input.GetKeyUp(KeyCode.C);

            _jumpInput = Input.GetKey(KeyCode.X);
            _jumpInputDown = Input.GetKeyDown(KeyCode.X);
            _jumpInputUp = Input.GetKeyUp(KeyCode.X);

            _defenseInput = Input.GetKey(KeyCode.Z);
            _defenseInputDown = Input.GetKeyDown(KeyCode.Z);
            _defenseInputUp = Input.GetKeyUp(KeyCode.Z);

            _upInput = Input.GetKey(KeyCode.UpArrow);
            _downInput = Input.GetKey(KeyCode.DownArrow);
        }
    }
}
