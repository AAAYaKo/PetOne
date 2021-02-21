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

        private readonly UiRepository _repository = UiRepository.Instance;
        private int _heartCount;
        private int _currentValue;


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
            }
        }
    }
}