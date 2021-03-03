using Leopotam.Ecs;
using PetOne.Linkers;
using UnityEngine;

namespace PetOne.Components
{
    internal struct ViewComponent
    {
        public EcsEntity Entity;
        public Animator Animator;
        public AnimationEventsProvider EventsProvider;
        public float TargetSpeedPercent;
    }
}