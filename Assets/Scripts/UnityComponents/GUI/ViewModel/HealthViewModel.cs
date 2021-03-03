using PetOne.Services;
using System.ComponentModel;
using UnityMVVM.ViewModel;

namespace PetOne.Ui.ViewModel
{
    internal sealed class HealthViewModel : ViewModelBase
    {
        public int HeartCount
        {
            get => _heartCount;
            set
            {
                if (value != _heartCount)
                {
                    _heartCount = value;
                    NotifyPropertyChanged(nameof(HeartCount));
                }
            }
        }
        public int CurrentValue
        {
            get => _currentValue;
            set
            {
                if (value != CurrentValue)
                {
                    _currentValue = value;
                    NotifyPropertyChanged(nameof(CurrentValue));
                }
            }
        }

        private int _heartCount;
        private int _currentValue;
        private PlayerHealthModel _model;


        /// <summary>
        /// Subscribe the view model to model
        /// </summary>
        /// <param name="model"></param>
        public void Bind(PlayerHealthModel model)
        {
            _model = model;
            _model.PropertyChanged += OnPropertyChanged;
        }

        private void OnDestroy()
        {
            _model.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update view model
            switch (e.PropertyName)
            {
                case nameof(_model.Max):
                    HeartCount = _model.Max / 4;
                    break;

                case nameof(_model.Current):
                    CurrentValue = _model.Current;
                    break;

                default:
                    break;
            }
        }
    }
}