using System;

namespace PetOne.Services
{
    internal sealed class PlayerHealthModel : BaseModel
    {
        /// <summary>
        /// Value must be aliquot to 4. Clamp current value to max
        /// </summary>
        public int Max
        {
            get => _max;
            set
            {
                if (value != _max)
                {
                    // Exeption
                    if (value % 4 != 0)
                        throw new ArgumentException("Value must be aliquod to 4");
                    // Clamp
                    if (value < Current)
                        Current = value;
                    // Update
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
                if (value != _current)
                {
                    // Exeption
                    if (value > _max || value < 0)
                        throw new ArgumentOutOfRangeException();
                    // Update
                    _current = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _max;
        private int _current;
    }
}
