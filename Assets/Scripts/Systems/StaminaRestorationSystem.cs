using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using UnityEngine;

namespace PetOne.Systems
{
    /// <summary>
    /// Restore stamina if it's not spent
    /// </summary>
    internal sealed class StaminaRestorationSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<Stamina>.Exclude<RunTag> _filter = null;
        private readonly InjectData _injectData = null;
        private readonly PlayerStaminaModel _model = null;


        void IEcsRunSystem.Run()
        {
            float amount = _injectData.StaminaAmount;
            float delta = Time.deltaTime;
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                ref var stamina = ref _filter.Get1(i);

                if (stamina.Amount != amount)
                {
                    // Restore stamina
                    stamina.Amount += stamina.RecoverySpeed * delta;
                    if (stamina.Amount >= amount)
                    {
                        stamina.Amount = amount;
                        // Not tired
                        entity.Del<TiredTag>();
                        _model.IsTired = false;
                        // Hide stamina
                        ref var hide = ref entity.Get<StaminaHideQuery>();
                        hide.TimeToHide = _injectData.HideStaminaTime;
                    }
                    _model.Amount = stamina.Amount;
                }
            }
        }
    }
}