using UnityEngine;
using UnityEngine.UI;

namespace PetOne.Ui.View
{
    internal sealed class HeartView : MonoBehaviour
    {
        private const float QUATER = 0.25f;
        [SerializeField] private Image _fill;

        /// <summary>
        /// The Value must be between 0 and 4
        /// </summary>
        public int Filling
        {
            set
            {
                _fill.fillAmount = value * QUATER;
            }
        }
    }
}