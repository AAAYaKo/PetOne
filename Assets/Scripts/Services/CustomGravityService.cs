using DataStructures.ViliWonka.KDTree;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Client
{
    public class CustomGravityService
    {
        private NGravitySource[] _sources;
        private readonly float _radiusOfScan;
        private readonly LayerMask _layer;

        internal CustomGravityService(float radius, int layer)
        {
            _radiusOfScan = radius;
            _layer = layer;
        }

        internal void SetSources(NGravitySource[] sources) => _sources = sources;

        internal NGravitySource GetNewGravitySourceId(float3 position)
        {
            Collider[] colliders = Physics.OverlapSphere(position, _radiusOfScan, _layer);
            int count = colliders.Length;
            Vector3[] points = new Vector3[count];
            for (int i = 0; i < count; i++)
                points[i] = colliders[i].ClosestPoint(position);

            KDTree tree = new KDTree(points, 4);
            KDQuery query = new KDQuery();
            List<int> index = new List<int>();
            query.ClosestPoint(tree, position, index);

            int id = colliders[index[0]].GetInstanceID();

            foreach (var i in _sources)
            {
                if (i.Id == id)
                    return i;
            }
            return _sources[0];
        }
    }
}