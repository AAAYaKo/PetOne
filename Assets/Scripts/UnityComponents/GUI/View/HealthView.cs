using System;
using System.Collections.Generic;
using UnityEngine;
using UnityMVVM.View;

namespace PetOne.Ui.View
{
    internal sealed class HealthView : ViewBase
    {
        [SerializeField] private GameObject _heartPrephab;

        /// <summary>
        /// Count must be positive
        /// </summary>
        public int HeartCount
        {
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Count can't be less then 0");

                if (_hearts.Count > value)
                {
                    // Count excess hearts
                    int removeCount = _hearts.Count - value;
                    for (int i = 0; i < removeCount; i++)
                    {
                        // Destroy last heart
                        int lastIndex = _hearts.Count - 1;
                        var heart = _hearts[lastIndex];
                        _hearts.RemoveAt(lastIndex);
                        Destroy(heart.gameObject);
                    }
                }
                else if (_hearts.Count < value)
                {
                    // Count additional hearts
                    int addCount = value - _hearts.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        //Add new heart
                        var instance = Instantiate(_heartPrephab, _transform);
                        if (instance.TryGetComponent(out HeartView heart))
                            _hearts.Add(heart);
                    }
                }
            }
        }
        /// <summary>
        /// Value must be between max value and zero
        /// </summary>
        public int CurrentValue
        {
            set
            {
                if (value <= 0 || value > _hearts.Count * 4)
                    throw new ArgumentOutOfRangeException();
                //Get empty hearts count
                int lastNotEmptyHeart = value / 4;
                int emptyCount = _hearts.Count - lastNotEmptyHeart;
                for (int i = 1; i < emptyCount; i++)
                {
                    // Empty the hearts
                    int index = _hearts.Count - i;
                    _hearts[index].Filling = 0;
                }
                if (lastNotEmptyHeart < _hearts.Count)
                {
                    //Fill not completly empty heart
                    int fillOfLast = value % 4;
                    _hearts[lastNotEmptyHeart].Filling = fillOfLast;
                }
            }
        }

        private readonly List<HeartView> _hearts = new List<HeartView>();
        private RectTransform _transform;


        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }
    }
}