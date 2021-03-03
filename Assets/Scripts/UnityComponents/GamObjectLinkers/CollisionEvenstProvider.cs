using System;
using UnityEngine;

namespace PetOne.Linkers
{
    [RequireComponent(typeof(Collider))]
    internal sealed class CollisionEvenstProvider : MonoBehaviour
    {
        public event Action<CollisionContext<Collision>> CollisionEvent = _ => { };
        public event Action<CollisionContext<Collider>> TriggerEvent = _ => { };


        private void OnCollisionEnter(Collision collision)
        {
            CollisionEvent.Invoke(CollisionContext.OnCollisionEnter.SetCollistion(collision));
        }

        private void OnCollisionStay(Collision collision)
        {
            CollisionEvent.Invoke(CollisionContext.OnCollisionStay.SetCollistion(collision));
        }

        private void OnCollisionExit(Collision collision)
        {
            CollisionEvent.Invoke(CollisionContext.OnCollisionExit.SetCollistion(collision));
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEvent.Invoke(CollisionContext.OnTriggerEnter.SetCollistion(other));
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerEvent.Invoke(CollisionContext.OnTriggerStay.SetCollistion(other));
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerEvent.Invoke(CollisionContext.OnTriggerExit.SetCollistion(other));
        }

        public struct CollisionContext
        {
            public static readonly CollisionContext<Collision> OnCollisionEnter = new CollisionContext<Collision>();
            public static readonly CollisionContext<Collision> OnCollisionStay = new CollisionContext<Collision>();
            public static readonly CollisionContext<Collision> OnCollisionExit = new CollisionContext<Collision>();

            public static readonly CollisionContext<Collider> OnTriggerEnter = new CollisionContext<Collider>();
            public static readonly CollisionContext<Collider> OnTriggerStay = new CollisionContext<Collider>();
            public static readonly CollisionContext<Collider> OnTriggerExit = new CollisionContext<Collider>();
        }

        public struct CollisionContext<T> where T : class
        {
            public T Collision { get; private set; }


            public CollisionContext<T> SetCollistion(T collision)
            {
                if (collision == null)
                    throw new ArgumentNullException();
                else
                {
                    Collision = collision;
                    return this;
                }
            }
        }
    }
}
