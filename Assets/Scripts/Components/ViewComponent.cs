using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    struct ViewComponent
    {
        public EcsEntity Entity;
        public Animator Animator;
        public float TargetSpeedPercent;
    }
}