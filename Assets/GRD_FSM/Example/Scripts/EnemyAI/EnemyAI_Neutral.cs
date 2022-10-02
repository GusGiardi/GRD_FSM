using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Enemy AI/Neutral")]
    public class EnemyAI_Neutral : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        private bool _chooseBehaviour;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myController = manager.GetComponent<EnemyAIController>();
        }

        public override void OnEnter()
        {
            _myController.StopAllCommands();
            _chooseBehaviour = false;
        }

        public override void OnUpdate()
        {
            if (!_chooseBehaviour)
            {
                _chooseBehaviour = true;
                return;
            }

            _myFSM.SetBool("GroundStyle", true);
        }
    }
}
