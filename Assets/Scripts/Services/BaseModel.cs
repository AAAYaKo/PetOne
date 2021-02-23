using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PetOne.Services
{
    internal abstract class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (x, y) => { };
        private readonly Dictionary<string, PropertyChangedEventArgs> _arguments = new Dictionary<string, PropertyChangedEventArgs>();


        protected void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            if (!_arguments.TryGetValue(name, out var argument))
            {
                argument = new PropertyChangedEventArgs(name);
                _arguments.Add(name, argument);
            }
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
