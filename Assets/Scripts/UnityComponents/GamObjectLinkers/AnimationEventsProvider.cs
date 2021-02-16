using System;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventsProvider : MonoBehaviour
    {
        public event Action AttackEnded;

        private void OnAttackEnded() => AttackEnded?.Invoke();
    }
}