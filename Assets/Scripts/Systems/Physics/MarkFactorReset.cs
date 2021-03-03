using Leopotam.Ecs;
using PetOne.Components;
using Unity.Mathematics;

namespace PetOne.Systems
{
    /// <summary>
    /// Autho mark factor reset by velocity of body
    /// </summary>
    internal sealed class MarkFactorReset : IEcsRunSystem
    {
        // auto-injected fields.
        private readonly EcsFilter<JumpData, PhysicBody, RealTransform> _filter = null;


        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                // Reset factor if entity in air
                if (_filter.Get1(i).IsInAir)
                {
                    var body = _filter.Get2(i).Value;
                    var up = _filter.Get3(i).Value.up;
                    // Reset factor if entity is not rising
                    if (math.dot(body.velocity, up) <= 0)
                        _filter.GetEntity(i).Get<FactorResetTag>();
                }
            }
        }
    }
}