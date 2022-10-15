using UnityEngine;

namespace GRD.FSM.Examples
{
    public class TitleScreenMenu : MonoBehaviour
    {
        [SerializeField] GameManager _gameManager;

        public void StartGame()
        {
            _gameManager.StartGame();
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
