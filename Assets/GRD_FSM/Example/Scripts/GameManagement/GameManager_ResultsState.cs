using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Game Manager - Results Behaviour", menuName = "FSM Behaviours/Game Manager/Results Behaviour", order = 1)]
    public class GameManager_ResultsState : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private GameManager _gameManager;

        private float _waitTimeCounter;
        private float _resultsTimeCounter;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _gameManager = manager.GetComponent<GameManager>();
        }

        public override void OnEnter()
        {
            _waitTimeCounter = 0;
            _resultsTimeCounter = 0;
        }

        public override void OnUpdate()
        {
            if (_waitTimeCounter < _gameManager.timeToShowResults)
            {
                _waitTimeCounter += Time.deltaTime;
                if (_waitTimeCounter >= _gameManager.timeToShowResults)
                {
                    ShowResults();
                }
                return;
            }

            _resultsTimeCounter += Time.deltaTime;
            if (_resultsTimeCounter >= _gameManager.winnerMessageTime)
            {
                _myFSM.SetBool("PlayGame", false);
            }
        }

        public override void OnExit()
        {
            HideResults();
        }

        private void ShowResults()
        {
            if (_gameManager.player1Warrior.currentHP <= 0 &&
                _gameManager.player2Warrior.currentHP <= 0)
            {
                _gameManager.drawMessage.SetActive(true);
            }
            else if (_gameManager.player1Warrior.currentHP <= 0)
            {
                _gameManager.blueWinsMessage.SetActive(true);
            }
            else if (_gameManager.player2Warrior.currentHP <= 0)
            {
                _gameManager.redWinsMessage.SetActive(true);
            }
        }

        private void HideResults()
        {
            _gameManager.drawMessage.SetActive(false);
            _gameManager.blueWinsMessage.SetActive(false);
            _gameManager.redWinsMessage.SetActive(false);
        }
    }
}
