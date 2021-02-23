using System;

namespace PetOne.Services
{
    internal sealed class PlayerHealthModel : BaseModel
    {
        public int Max
        {
            get => _max;
            set
            {
                if (value % 4 != 0)
                    throw new ArgumentException("Value must be aliquod to 4");
                if (value < Current)
                    Current = value;
                if (value != _max)
                {
                    _max = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Current
        {
            get => _current;
            set
            {
                if (value > _max || value < 0)
                    throw new ArgumentOutOfRangeException();
                if (value != _current)
                {
                    _current = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _max;
        private int _current;
    }
}
