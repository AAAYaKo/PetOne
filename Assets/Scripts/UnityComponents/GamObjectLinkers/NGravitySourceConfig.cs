using Unity.Mathematics;
using UnityEngine;

namespace PetOne.Linkers
{
    [RequireComponent(typeof(Collider)), AddComponentMenu("CustomGravity/NGravitySourceTag")]
    internal sealed class NGravitySourceConfig : MonoBehaviour
    {
        public float GravityFactor = 9.81f;
        public int Id;
        public float3 Position;

#if UNITY_EDITOR
        [SerializeField] private int Layer = 7;

        private void OnValidate()
        {
            gameObject.layer = 7;
            Id = GetComponent<Collider>().GetInstanceID();
            Position = transform.position;
        }
#endif
    }
}