using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Hides stamina GUI after delay time end if entity hasn't RunTag
    /// </summary>
    internal sealed class StaminaHideSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<StaminaHideQuery> _hide = null;
        private readonly EcsFilter<StaminaHideQuery, RunTag> _run = null;
        private readonly PlayerStaminaModel _model = null;


        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime;
            foreach (var i in _hide)
            {
                ref var hide = ref _hide.Get1(i);
                // Hide
                if (hide.TimeToHide <= 0)
                {
                    _hide.GetEntity(i).Del<StaminaHideQuery>();
                    _model.IsVisible = false;
                }
                // Tick
                else
                    hide.TimeToHide -= delta;
            }
            // No hide
            foreach (var i in _run)
                _run.GetEntity(i).Del<StaminaHideQuery>();
        }
    }
}