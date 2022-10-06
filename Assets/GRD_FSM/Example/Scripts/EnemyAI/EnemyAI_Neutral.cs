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

        private const float _groundStyleChance = 0.3f;
        private const float _airStyleChance = 0.55f;
        private const float _counterAttackChance = 0.8f;
        private const float _strongAirChance = 0.2f;

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

            float rnd = Random.value;
            if (rnd <= _groundStyleChance)
            {
                _myFSM.SetBool("GroundStyle", true);
            }
            else if (rnd <= _airStyleChance)
            {
                _myFSM.SetBool("AirStyle", true);
            }
            else if (rnd <= _counterAttackChance)
            {
                _myFSM.SetBool("Counterattack", true);
            }
            else
            {
                _myFSM.SetBool("StrongAirAttack", true);
            }
        }
    }
}
