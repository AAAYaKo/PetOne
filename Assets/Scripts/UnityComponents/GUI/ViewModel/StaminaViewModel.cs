using System.ComponentModel;
using UnityEngine;
using UnityMVVM.ViewModel;

namespace PetOne.Ui.ViewModel
{
    internal sealed class StaminaViewModel : ViewModelBase
    {
        public bool IsTired
        {
            get => _isTired;
            set
            {
                if (value != _isTired)
                {
                    _isTired = value;
                    NotifyPropertyChanged(nameof(IsTired));
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    NotifyPropertyChanged(nameof(IsVisible));
                }
            }
        }

        public float Amount
        {
            get => _amount;
            set
            {
                if (value != _amount)
                {
                    _amount = value;
                    NotifyPropertyChanged(nameof(Amount));
                }
            }
        }

        public Vector3 Position
        {
            get => _position;
            set
            {
                if (value != _position)
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
        private readonly UiRepository _repository = UiRepository.Instance;


        private void OnEnable()
        {
            _repository.PropertyChanged += OnStaminaAmountChanged;
        }

        private void OnDisable()
        {
            _repository.PropertyChanged -= OnStaminaAmountChanged;
        }

        private void OnStaminaAmountChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                default:
                    break;
                case nameof(_repository.StaminaViewPosition):
                    Position = _repository.StaminaViewPosition;
                    break;
                case nameof(_repository.StaminaAmount):
                    Amount = _repository.StaminaAmount;
                    break;
                case nameof(_repository.IsStaminaTired):
                    IsTired = _repository.IsStaminaTired;
                    break;
                case nameof(_repository.IsStaminaVisible):
                    IsVisible = _repository.IsStaminaVisible;
                    break;
            }
        }
    }
}