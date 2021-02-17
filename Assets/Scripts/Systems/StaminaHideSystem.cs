using Leopotam.Ecs;
using UnityEngine;

namespace Client
{
    sealed class StaminaHideSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<StaminaHideQuery> _filter1 = null;
        private readonly EcsFilter<StaminaHideQuery, RunTag> _filter2 = null;
        [EcsIgnoreInject] private readonly UiRepository repository = UiRepository.Instance;


        void IEcsRunSystem.Run()
        {
            float delta = Time.deltaTime;
            foreach (var i in _filter1)
            {
                ref var hide = ref _filter1.Get1(i);
                if (hide.TimeToHide <= 0)
                {
                    repository.IsStaminaVisible = false;
                    _filter1.GetEntity(i).Del<StaminaHideQuery>();
                }
                else
                    hide.TimeToHide -= delta;
            }
            foreach (var i in _filter2)
            {
                _filter2.GetEntity(i).Del<StaminaHideQuery>();
            }
        }
    }
}