using UnityEngine;

namespace GRD.FSM.Examples
{
    public class HowToPlayMenu : MonoBehaviour
    {
        [SerializeField] GameObject[] _pages;
        private int _currentPage;
        [SerializeField] GameObject _previousPageButton;
        [SerializeField] GameObject _nextPageButton;

        public void OpenHowToPlay()
        {
            _currentPage = 0;
            GoToPage(_currentPage);
            gameObject.SetActive(true);
        }

        public void PreviousPage()
        {
            _currentPage--;
            if (_currentPage < 0)
                _currentPage = 0;
            GoToPage(_currentPage);
        }

        public void NextPage()
        {
            _currentPage++;
            if (_currentPage > _pages.Length - 1)
                _currentPage = _pages.Length - 1;
            GoToPage(_currentPage);
        }

        private void GoToPage(int pageIndex)
        {
            for (int i = 0; i < _pages.Length; i++)
            {
                _pages[i].SetActive(i == pageIndex);
            }

            _previousPageButton.SetActive(pageIndex != 0);
            _nextPageButton.SetActive(pageIndex != _pages.Length-1);
        }
    }
}
