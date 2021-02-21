using UnityEngine;
using UnityEngine.UI;
using UnityMVVM.View;

namespace PetOne.Ui.View
{
    internal sealed class StaminaView : ViewBase
    {
        [SerializeField] private Color normal;
        [SerializeField] private Color tired;
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _staminaTransform;
        [SerializeField] private Vector3 offcet;

        public float Amount
        {
            set
            {
                _image.fillAmount = value;
            }
        }

        public Vector3 Position
        {
            set
            {
                _staminaTransform.position = value + offcet;
            }
        }

        public bool IsTired
        {
            set
            {
                _image.color = value switch
                {
                    true => tired,
                    false => normal
                };
            }
        }
    }
}