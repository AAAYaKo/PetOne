using System;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventsProvider : MonoBehaviour
    {
        public event Action<AnimationEventContext> AnimationEvent;
        private AnimationEventContext eventContext = new AnimationEventContext();


        private void OnAnimationEvent(AnimationEvent animationEvent)
        {
            InitContext(animationEvent);
            AnimationEvent?.Invoke(eventContext);
        }

        private void InitContext(AnimationEvent animationEvent)
        {
            eventContext.FloatParemeter = animationEvent.floatParameter;
            eventContext.IntParameter = animationEvent.intParameter;
            eventContext.StringParameter = animationEvent.stringParameter;
            eventContext.ObjectParameter = animationEvent.objectReferenceParameter;
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