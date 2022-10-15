using UnityEngine;
using UnityEngine.UI;

namespace GRD.FSM.Examples
{
    public class PlayerBar : MonoBehaviour
    {
        [SerializeField] WarriorScript _myWarrior;
        [SerializeField] Image _healthBar;
        [SerializeField] Image _shieldBar;

        void Update()
        {
            _healthBar.fillAmount = _myWarrior.currentHPNormalized;
            _shieldBar.fillAmount = _myWarrior.currentShieldNormalized;
        }
    }
}
