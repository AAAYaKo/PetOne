using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using UnityEngine;

namespace PetOne.Systems
{
    internal sealed class StaminaRestorationSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<Stamina> _filter = null;
        private readonly InjectData _injectData = null;
        private readonly PlayerStaminaModel _model = null;


        void IEcsRunSystem.Run()
        {
            float amount = _injectData.StaminaAmount;
            float delta = Time.deltaTime;
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                if(!entity.Has<RunTag>() || !entity.Has<InputDirection>())
                {
                    ref var stamina = ref _filter.Get1(i);
                    if(stamina.Amount != amount)
                    {
                        stamina.Amount += stamina.RecoverySpeed * delta;
                        if(stamina.Amount >= amount)
                        {
                            stamina.Amount = amount;
                            entity.Del<TiredTag>();
                            ref var hide = ref entity.Get<StaminaHideQuery>();
                            hide.TimeToHide = _injectData.HideStaminaTime;
                            _model.IsTired = false;
                        }
                        _model.Amount = stamina.Amount;
                    }
                }
            }
        }
    }
}