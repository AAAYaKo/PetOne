using UnityEngine;
using UnityEngine.UI;
using UnityMVVM.View;

namespace PetOne.Ui.View
{
    internal sealed class StaminaView : ViewBase
    {
        [SerializeField] private Color _normal;
        [SerializeField] private Color _tired;
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _staminaTransform;
        [SerializeField] private Vector3 _offcet;

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
                _staminaTransform.position = value + _offcet;
            }
        }
        // TODO: rewrite to BoolToColorConverter
        public bool IsTired
        {
            set
            {
                _image.color = value switch
                {
                    true => _tired,
                    false => _normal
                };
            }
        }
    }
}