using Leopotam.Ecs;

namespace Client
{
    sealed class AnimatorLandingSystem : IEcsRunSystem
    {
        private const string JUMP_FIELD_NAME = "Jump Rising";

        // auto-injected fields.
        private readonly EcsFilter<LandedTag, ViewComponent> _filter = null;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                ref ViewComponent view = ref _filter.Get2(i);
                view.Animator.SetBool(JUMP_FIELD_NAME, false);
            }
        }
    }
}