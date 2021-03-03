using System;
using UnityEngine;

namespace PetOne.Linkers
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventsProvider : MonoBehaviour
    {
        public event Action<AnimationEventContext> AnimationEvent = _ => { };
        private AnimationEventContext _eventContext = new AnimationEventContext();


        private void OnAnimationEvent(AnimationEvent animationEvent)
        {
            InitContext(animationEvent);
            AnimationEvent.Invoke(_eventContext);
        }

        private void InitContext(AnimationEvent animationEvent)
        {
            _eventContext.FloatParemeter = animationEvent.floatParameter;
            _eventContext.IntParameter = animationEvent.intParameter;
            _eventContext.StringParameter = animationEvent.stringParameter;
            _eventContext.ObjectParameter = animationEvent.objectReferenceParameter;
        }

        public struct AnimationEventContext
        {
            public float FloatParemeter;
            public int IntParameter;
            public string StringParameter;
            public UnityEngine.Object ObjectParameter;
        }
    }
}