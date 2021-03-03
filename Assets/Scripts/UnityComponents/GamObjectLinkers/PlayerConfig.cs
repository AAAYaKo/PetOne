using UnityEngine;

namespace PetOne.Linkers
{
    [RequireComponent(typeof(Rigidbody), typeof(CollisionEvenstProvider))]
    internal sealed class PlayerConfig : MonoBehaviour
    {
        public Transform SelfTransform;
        public Rigidbody SelfRigidbody;
        public Transform ViewTransform;
        public Animator ViewAnimator;
        public AnimationEventsProvider AnimationEventsProvider;
        public CollisionEvenstProvider CollisionProvider;
        public Collider Collider;


#if UNITY_EDITOR
        private void OnValidate()
        {
            SelfTransform = transform;
            SelfRigidbody = GetComponent<Rigidbody>();
            CollisionProvider = GetComponent<CollisionEvenstProvider>();
            Collider = GetComponent<Collider>();
            ViewAnimator = GetComponentInChildren<Animator>();
            ViewTransform = ViewAnimator.transform;
            AnimationEventsProvider = ViewAnimator.GetComponent<AnimationEventsProvider>();
        }
#endif
    }
}