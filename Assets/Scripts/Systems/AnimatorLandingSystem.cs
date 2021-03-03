using Leopotam.Ecs;
using PetOne.Components;

namespace PetOne.Systems
{
    /// <summary>
    /// Reset Animation on Landing
    /// </summary>
    internal sealed class AnimatorLandingSystem : IEcsRunSystem
    {
        // names related to animator
        private const string JUMP_PROPERTY_NAME = "Jump Rising";

        // auto-injected fields.
        private readonly EcsFilter<ViewComponent, LandedTag> _filter = null;


        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                ref var view = ref _filter.Get1(i);
                view.Animator.SetBool(JUMP_PROPERTY_NAME, false);
            }
        }
    }
}