using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Enemy - Neutral Behaviour", menuName = "FSM Behaviours/Enemy/Neutral", order = 1)]
    public class EnemyAI_Neutral : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        private bool _chooseBehaviour;

        [SerializeField] float _groundStyleChance = 0.3f;
        [SerializeField] float _airStyleChance = 0.55f;
        [SerializeField] float _counterAttackChance = 0.8f;
        [SerializeField] float _strongAirChance = 0.2f;

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
