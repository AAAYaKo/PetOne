using System.Collections.Generic;
using UnityEngine;
using UnityMVVM.View;

namespace PetOne.Ui.View
{
    internal sealed class HealthView : ViewBase
    {
        [SerializeField] private GameObject _heartPrephab;

        public int HeartCount
        {
            set
            {
                if (_hearts.Count > value)
                {
                    int removeCount = _hearts.Count - value;
                    for (int i = 0; i < removeCount; i++)
                    {
                        int lastIndex = _hearts.Count - 1;
                        var heart = _hearts[lastIndex];
                        _hearts.RemoveAt(lastIndex);
                        Destroy(heart.gameObject);
                    }
                }
                else if (_hearts.Count < value)
                {
                    int addCount = value - _hearts.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        var instance = Instantiate(_heartPrephab, _transform);
                        if (instance.TryGetComponent(out HeartView heart))
                            _hearts.Add(heart);
                    }
                }
            }
        }

        public int CurrentValue
        {
            set
            {
                int lastFullHeart = value / 4;
                //Empty hearts
                int emptyCount = _hearts.Count - lastFullHeart;
                for (int i = 1; i < emptyCount; i++)
                {
                    int index = _hearts.Count - i;
                    _hearts[index].Filling = 0;
                }
                int fillOfLast = value % 4;
                if (emptyCount < _hearts.Count)
                    _hearts[emptyCount].Filling = fillOfLast;
            }
        }

        private List<HeartView> _hearts = new List<HeartView>();
        private RectTransform _transform;


        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }
    }
}