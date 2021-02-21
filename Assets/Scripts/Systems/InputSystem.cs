using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Linkers;
using PetOne.Ui;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetOne.Systems
{
    internal sealed class InputSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string ATTACK_PROPERTY_NAME = "Attack";
        private const string EQUIP_PROPERTY_NAME = "Equip";
        private const string DISARM_PROPERTY_NAME = "Disarm";
        private const string ATTACK_EVENT_NAME = "Attack Ended";
        private const string EQUIP_EVENT_NAME = "Equipment Ended";
        private const string DISARM_EVENT_NAME = "Disarm Ended";

        // auto-injected fields.
        private readonly EcsFilter<ViewComponent, InputTag> _filter1 = null;
        private readonly Inputs _inputs = null;
        private readonly AnimationEventsProvider _provider = null;
        [EcsIgnoreInject] private readonly UiRepository repository = UiRepository.Instance;


        public void Init()
        {
            _inputs.Player.Move.performed += OnMovePerformed;
            _inputs.Player.Move.canceled += OnMoveCanceled;
            _inputs.Player.Run.performed += OnRunPerformed;
            _inputs.Player.Jump.performed += OnJumpPerformed;
            _inputs.Player.Jump.canceled += OnJumpCanceled;
            _inputs.Player.Attack.performed += OnAttackPerformed;
            _provider.AnimationEvent += OnAnimaitonEvent;
        }
        public void Destroy()
        {
            _inputs.Player.Move.performed -= OnMovePerformed;
            _inputs.Player.Move.canceled -= OnMoveCanceled;
            _inputs.Player.Run.performed -= OnRunPerformed;
            _inputs.Player.Jump.performed -= OnJumpPerformed;
            _inputs.Player.Jump.canceled -= OnJumpCanceled;
            _inputs.Player.Attack.performed -= OnAttackPerformed;
            _provider.AnimationEvent -= OnAnimaitonEvent;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            var inputDirection = context.ReadValue<Vector2>();

            foreach (var i in _filter1)
            {
                var entity = _filter1.GetEntity(i);
                entity.Get<TargetSpeedPercentChangedTag>();

                ref var direction = ref entity.Get<InputDirection>();
                direction.Value = inputDirection;
            }
        }
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
            {
                ref var entity = ref _filter1.GetEntity(i);

                ref var view = ref _filter1.Get1(i);
                view.TargetSpeedPercent = 0;

                entity.Del<InputDirection>();
                entity.Del<RunTag>();
                entity.Del<PhysicTranslation>();
            }
        }
        private void OnRunPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
            {
                var entity = _filter1.GetEntity(i);
                if (entity.Has<InputDirection>() && !entity.Has<TiredTag>() && !entity.Has<JumpData>())
                {
                    entity.Get<TargetSpeedPercentChangedTag>();
                    entity.Get<RunTag>();
                    repository.IsStaminaVisible = true;
                }
            }
        }
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
            {
                var entity = _filter1.GetEntity(i);
                if (entity.Has<RunTag>())
                {
                    entity.Del<RunTag>();
                    entity.Get<TargetSpeedPercentChangedTag>();
                }
                else
                {
                    if (entity.Has<ArmedTag>() && !entity.Has<AttackTag>())
                    {
                        entity.Del<ArmedTag>();
                        var view = _filter1.Get1(i);
                        view.Animator.SetTrigger(DISARM_PROPERTY_NAME);
                        entity.Get<BlockMoveTag>();
                    }
                    else if(!entity.Has<BlockMoveTag>() && !entity.Has<ArmedTag>())
                        entity.Get<JumpQueryTag>();
                }
            }
        }
        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
                _filter1.GetEntity(i).Get<FactorResetTag>();
        }
        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
            {
                var entity = _filter1.GetEntity(i);
                if (!entity.Has<ArmedTag>() && !entity.Has<BlockMoveTag>())
                {
                    var view = _filter1.Get1(i);
                    entity.Del<PhysicTranslation>();
                    entity.Get<BlockMoveTag>();
                    view.Animator.SetTrigger(EQUIP_PROPERTY_NAME);
                }
            }
            foreach (var i in _filter1)
            {
                var entity = _filter1.GetEntity(i);
                if(entity.Has<ArmedTag>() && !entity.Has<AttackTag>())
                {
                    var view = _filter1.Get1(i);
                    entity.Del<PhysicTranslation>();
                    entity.Get<AttackTag>();
                    entity.Get<BlockMoveTag>();
                    view.Animator.SetTrigger(ATTACK_PROPERTY_NAME);
                }
            }
        }
        private void OnAnimaitonEvent(AnimationEventsProvider.AnimationEventContext context)
        {
            switch (context.StringParameter)
            {
                case ATTACK_EVENT_NAME:
                    foreach (var i in _filter1)
                    {
                        var entity = _filter1.GetEntity(i);
                        entity.Del<BlockMoveTag>();
                        entity.Del<AttackTag>();
                    }
                    break;

                case EQUIP_EVENT_NAME:
                    foreach (var i in _filter1)
                    {
                        var entity = _filter1.GetEntity(i);
                        if (!entity.Has<AttackTag>())
                        {
                            entity.Del<BlockMoveTag>();
                            entity.Get<ArmedTag>();
                        }
                    }
                    break;

                case DISARM_EVENT_NAME:
                    foreach (var i in _filter1)
                    {
                        var entity = _filter1.GetEntity(i);
                        if (!entity.Has<AttackTag>())
                        {
                            entity.Del<BlockMoveTag>();
                            entity.Del<ArmedTag>();
                        }
                    }
                    break;
            }
        }
    }
}