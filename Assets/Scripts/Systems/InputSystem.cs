using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Client
{
    sealed class InputSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string ATTACK_PROPERTY_NAME = "Attack";

        // auto-injected fields.
        private readonly EcsFilter<ViewComponent, InputTag> _filter1 = null;
        private readonly EcsFilter<AttackTag> _filter2 = null;
        private readonly EcsFilter<JumpData> _filter3 = null;
        private readonly Inputs _inputs = null;
        private readonly AnimationEventsProvider _provider = null;


        public void Init()
        {
            _inputs.Player.Move.performed += OnMovePerformed;
            _inputs.Player.Move.canceled += OnMoveCanceled;
            _inputs.Player.Run.performed += OnRunPerformed;
            _inputs.Player.Run.canceled += OnRunCanceled;
            _inputs.Player.Jump.performed += OnJumpPerformed;
            _inputs.Player.Jump.canceled += OnJumpCanceled;
            _inputs.Player.Attack.performed += OnAttackPerformed;
            _provider.AttackEnded += OnAttackEnded;
        }
        public void Destroy()
        {
            _inputs.Player.Move.performed -= OnMovePerformed;
            _inputs.Player.Move.canceled -= OnMoveCanceled;
            _inputs.Player.Run.performed -= OnRunPerformed;
            _inputs.Player.Run.canceled -= OnRunCanceled;
            _inputs.Player.Jump.performed -= OnJumpPerformed;
            _inputs.Player.Jump.canceled -= OnJumpCanceled;
            _inputs.Player.Attack.performed -= OnAttackPerformed;
            _provider.AttackEnded -= OnAttackEnded;
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
                entity.Del<PhysicTranslation>();
            }
        }
        private void OnRunPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
            {
                var entity = _filter1.GetEntity(i);
                if (!entity.Has<TiredTag>())
                {
                    entity.Get<TargetSpeedPercentChangedTag>();
                    entity.Get<RunTag>();
                }
            }
        }
        private void OnRunCanceled(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
            {
                var entity = _filter1.GetEntity(i);
                entity.Get<TargetSpeedPercentChangedTag>();
                entity.Del<RunTag>();
            }
        }
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
                _filter1.GetEntity(i).Get<JumpQueryTag>();
        }
        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            foreach (var i in _filter3)
                _filter3.GetEntity(i).Get<FactorReset>();
        }
        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            foreach (var i in _filter1)
            {
                EcsEntity entity = _filter1.GetEntity(i);
                if (!entity.Has<AttackTag>())
                {
                    ViewComponent view = _filter1.Get1(i);
                    entity.Del<PhysicTranslation>();
                    entity.Get<AttackTag>();
                    view.Animator.SetTrigger(ATTACK_PROPERTY_NAME);
                }
            }
        }
        private void OnAttackEnded()
        {
            foreach (var i in _filter2)
            {
                EcsEntity entity = _filter2.GetEntity(i);
                entity.Del<AttackTag>();
            }
        }
    }
}