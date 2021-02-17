using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UiRepository : INotifyPropertyChanged
{
    public static UiRepository Instance
    {
        get
        {
            if (_instance == null)
                _instance = new UiRepository();
            return _instance;
        }
    }

    public bool IsStaminaTired
    {
        get => _isStaminaTired;
        set
        {
            if (value != _isStaminaTired)
            {
                _isStaminaTired = value;
                OnPropertyChanged();
            }
        }
    }  
    
    public bool IsStaminaVisible
    {
        get => _isStaminaVisible;
        set
        {
            if (value != _isStaminaVisible)
            {
                _isStaminaVisible = value;
                OnPropertyChanged();
            }
        }
    }

    public float StaminaAmount
    {
        get => _staminaAmount;
        set
        {
            if (value != _staminaAmount)
            {
                _staminaAmount = value;
                OnPropertyChanged();
            }
        }
    }

    public Vector3 StaminaViewPosition
    {
        get => _entityPositin;
        set
        {
            if (value != _entityPositin)
            {
                _entityPositin = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private static UiRepository _instance;
    private bool _isStaminaTired;
    private bool _isStaminaVisible;
    private float _staminaAmount;
    private Vector3 _entityPositin;
    private readonly Dictionary<string, PropertyChangedEventArgs> arguments = new Dictionary<string, PropertyChangedEventArgs>();


    private UiRepository() { }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (!arguments.TryGetValue(propertyName, out var argument))
        {
            argument = new PropertyChangedEventArgs(propertyName);
            arguments.Add(propertyName, argument);
        }
        PropertyChanged?.Invoke(this, argument);
    }
}
