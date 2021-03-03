using Leopotam.Ecs;
using PetOne.Components;
using PetOne.Linkers;
using PetOne.Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetOne.Systems
{
    /// <summary>
    /// Subscribe and Unsubcribe to input events
    /// </summary>
    internal sealed class InputSystem : IEcsInitSystem, IEcsDestroySystem
    {
        // names related to player animator
        private const string ATTACK_PROPERTY_NAME = "Attack";
        private const string EQUIP_PROPERTY_NAME = "Equip";
        private const string DISARM_PROPERTY_NAME = "Disarm";
        private const string ATTACK_EVENT_NAME = "Attack Ended";
        private const string EQUIP_EVENT_NAME = "Equipment Ended";
        private const string DISARM_EVENT_NAME = "Disarm Ended";

        // auto-injected fields.
        private readonly EcsFilter<ViewComponent, PlayerTag> _filter = null;
        private readonly Inputs _inputs = null;
        private readonly PlayerStaminaModel _staminaModel = null;

        public void Init()
        {
            // Subscribe
            _inputs.Player.Move.performed += OnMovePerformed;
            _inputs.Player.Move.canceled += OnMoveCanceled;
            _inputs.Player.Run.performed += OnRunPerformed;
            _inputs.Player.Jump.performed += OnJumpPerformed;
            _inputs.Player.Jump.canceled += OnJumpCanceled;
            _inputs.Player.Attack.performed += OnAttackPerformed;
            foreach (var i in _filter)
            {
                var provider = _filter.Get1(i).EventsProvider;
                provider.AnimationEvent += OnAnimaitonEvent;
            }
        }

        public void Destroy()
        {
            // Unsubscribe
            _inputs.Player.Move.performed -= OnMovePerformed;
            _inputs.Player.Move.canceled -= OnMoveCanceled;
            _inputs.Player.Run.performed -= OnRunPerformed;
            _inputs.Player.Jump.performed -= OnJumpPerformed;
            _inputs.Player.Jump.canceled -= OnJumpCanceled;
            _inputs.Player.Attack.performed -= OnAttackPerformed;
            foreach (var i in _filter)
            {
                var provider = _filter.Get1(i).EventsProvider;
                provider.AnimationEvent -= OnAnimaitonEvent;
            }
        }


        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            var inputDirection = context.ReadValue<Vector2>();

            // Write input
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                entity.Get<TargetSpeedPercentChangedTag>();

                ref var direction = ref entity.Get<InputDirection>();
                direction.Value = inputDirection;
            }
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            // Reset input
            foreach (var i in _filter)
            {
                ref var entity = ref _filter.GetEntity(i);

                ref var view = ref _filter.Get1(i);
                view.TargetSpeedPercent = 0;

                entity.Del<InputDirection>();
                entity.Del<RunTag>();
                entity.Del<PhysicTranslation>();
            }
        }

        private void OnRunPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                // Run
                if (entity.Has<InputDirection>() && !entity.Has<TiredTag>() && !entity.Has<JumpData>())
                {
                    entity.Get<TargetSpeedPercentChangedTag>();
                    entity.Get<RunTag>();
                    _staminaModel.IsVisible = true;
                }
            }
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                // Cancel run
                if (entity.Has<RunTag>())
                {
                    entity.Del<RunTag>();
                    entity.Get<TargetSpeedPercentChangedTag>();
                }
                // Disarm
                else if (entity.Has<ArmedTag>() && !entity.Has<AttackTag>())
                {
                    entity.Del<ArmedTag>();
                    var view = _filter.Get1(i);
                    entity.Get<BlockMoveTag>();
                    view.Animator.SetTrigger(DISARM_PROPERTY_NAME);
                }
                // Jump
                else if (!entity.Has<BlockMoveTag>() && !entity.Has<ArmedTag>())
                    entity.Get<JumpQueryTag>();

            }
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            foreach (var i in _filter)
                _filter.GetEntity(i).Get<FactorResetTag>();
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter)
            {
                var entity = _filter.GetEntity(i);
                bool armered = entity.Has<ArmedTag>();
                // Eqip
                if (!armered && !entity.Has<BlockMoveTag>())
                {
                    var view = _filter.Get1(i);
                    entity.Get<BlockMoveTag>();
                    view.Animator.SetTrigger(EQUIP_PROPERTY_NAME);
                }
                // Attack
                else if (armered && !entity.Has<AttackTag>())
                {
                    var view = _filter.Get1(i);
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
                // On the End of Attack
                case ATTACK_EVENT_NAME:
                    foreach (var i in _filter)
                    {
                        var entity = _filter.GetEntity(i);
                        entity.Del<BlockMoveTag>();
                        entity.Del<AttackTag>();
                    }
                    break;

                // On the End of Equip
                case EQUIP_EVENT_NAME:
                    foreach (var i in _filter)
                    {
                        var entity = _filter.GetEntity(i);
                        if (!entity.Has<AttackTag>())
                        {
                            entity.Del<BlockMoveTag>();
                            entity.Get<ArmedTag>();
                        }
                    }
                    break;

                // On the End of Disarm
                case DISARM_EVENT_NAME:
                    foreach (var i in _filter)
                    {
                        var entity = _filter.GetEntity(i);
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