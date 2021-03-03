using PetOne.Services;
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
        private PlayerStaminaModel _model;


        /// <summary>
        /// Subcribe the view model to the model
        /// </summary>
        /// <param name="model"></param>
        public void Bind(PlayerStaminaModel model)
        {
            _model = model;
            _model.PropertyChanged += OnPropertyChanged;
        }

        private void OnDestroy()
        {
            // Unsubscribe
            _model.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update view model
            switch (e.PropertyName)
            {
                case nameof(PlayerStaminaModel.Amount):
                    Amount = _model.Amount;
                    break;

                case nameof(PlayerStaminaModel.IsTired):
                    IsTired = _model.IsTired;
                    break;

                case nameof(PlayerStaminaModel.IsVisible):
                    IsVisible = _model.IsVisible;
                    break;

                case nameof(PlayerStaminaModel.Position):
                    Position = _model.Position;
                    break;

                default:
                    break;
            }
        }
    }
}