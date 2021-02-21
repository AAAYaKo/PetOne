using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Services;
using Unity.Mathematics;

namespace PetOne.Systems
{
    internal sealed class TargetSpeedPercentChangeSystem : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<InputDirection, ViewComponent, TargetSpeedPercentChangedTag> _filter = null;
        private readonly InjectData _injectData = null;


        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                float moveScale = math.length(_filter.Get1(i).Value);
                ref var view = ref _filter.Get2(i);
                var entity = _filter.GetEntity(i);
                view.TargetSpeedPercent = moveScale * (entity.Has<RunTag>() && !entity.Has<TiredTag>() ? 
                    _injectData.FastRunPercent : _injectData.SlowRunPercent);
            }
        }
    }
}