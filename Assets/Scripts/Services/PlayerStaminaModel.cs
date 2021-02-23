using UnityEngine;

namespace PetOne.Services
{
    internal sealed class PlayerStaminaModel : BaseModel
    {
        public float Amount
        {
            get => _amount;
            set
            {
                if(value != _amount)
                {
                    _amount = value;
                    NotifyPropertyChanged(nameof(Amount));
                }
            }
        }
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if(value != _isVisible)
                {
                    _isVisible = value;
                    NotifyPropertyChanged(nameof(IsVisible));
                }
            }
        }
        public bool IsTired
        {
            get => _isTired;
            set
            {
                if(value != _isTired)
                {
                    _isTired = value;
                    NotifyPropertyChanged(nameof(IsTired));
                }
            }
        }
        public Vector3 Position
        {
            get => _position;
            set
            {
                if(value != _position)
                {
                    _position = value;
                    NotifyPropertyChanged(nameof(Position));
                }
            }
        }
        private bool _isTired;
        private bool _isVisible;
        private float _amount;
        private Vector3 _position;
    }
}
