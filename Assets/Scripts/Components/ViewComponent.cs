using Leopotam.Ecs;
using UnityEngine;

namespace PetOne.Components
{
    internal struct ViewComponent
    {
        public EcsEntity Entity;
        public Animator Animator;
        public float TargetSpeedPercent;
    }
}