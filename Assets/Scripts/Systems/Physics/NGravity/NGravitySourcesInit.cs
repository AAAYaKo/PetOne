using Leopotam.Ecs;
using System.Collections.Generic;

namespace Client
{
    sealed class NGravitySourcesInit : IEcsPreInitSystem
    {
        // auto-injected fields.
        private readonly EcsWorld _world = null;
        private readonly NGravitySourceTag[] _sources = null;

        public void PreInit()
        {
            if(_sources.Length != 0)
            {
                List<NGravitySource> sources = new List<NGravitySource>();
                foreach (var i in _sources)
                {
                    EcsEntity entity = _world.NewEntity();
                    ref NGravitySource source = ref entity.Get<NGravitySource>();
                    source.GravityFactor = i.GravityFactor;
                    source.Id = i.Id;
                    sources.Add(source);
                }
                EcsEntity all = _world.NewEntity();
                ref AllCustomGravitySources allSources = ref all.Get<AllCustomGravitySources>();
                allSources.Sources = sources.ToArray();
            }
        }
    }
}