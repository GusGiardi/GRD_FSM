using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Game Manager - Main Menu Behaviour", menuName = "FSM Behaviours/Game Manager/Main Menu Behaviour", order = 1)]
    public class GameManager_MainMenuState : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private GameManager _gameManager;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _gameManager = manager.GetComponent<GameManager>();
        }

        public override void OnEnter()
        {
            _gameManager.ActiveMainMenu(true);
            _gameManager.DisableWarriors();
        }

        public override void OnExit()
        {
            _gameManager.ActiveMainMenu(false);
        }
    }
}
