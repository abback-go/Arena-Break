using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private Transform _cameraPivot;

        [Header("Move")]
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravity = -20f;

        // 접지 상태에서 0으로 두면 isGrounded 판정이 프레임마다 흔들린다. 살짝 눌러 붙인다
        [SerializeField] private float _groundedStickVelocity = -2f;

        [Header("Look")]
        [SerializeField] private float _mouseSensitivity = 0.1f;
        [SerializeField] private float _minPitch = -80f;
        [SerializeField] private float _maxPitch = 80f;

        // 커서가 잠기는 첫 프레임에 delta가 크게 튄다. 막지 않으면 시작하자마자 바닥을 본다
        [SerializeField] private float _maxLookDeltaPerFrame = 100f;

        [Header("Cursor")]
        [SerializeField] private bool _lockCursor = true;

        private CharacterController _controller;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;

        private float _pitch;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            InputActionMap playerMap = _inputActions.FindActionMap("Player", true);
            _moveAction = playerMap.FindAction("Move", true);
            _lookAction = playerMap.FindAction("Look", true);
            _jumpAction = playerMap.FindAction("Jump", true);
        }

        // 맵 전체가 아니라 액션 단위로 켠다. 같은 에셋을 쓰는 다른 컴포넌트를 같이 끄지 않기 위해
        private void OnEnable()
        {
            _moveAction.Enable();
            _lookAction.Enable();
            _jumpAction.Enable();

            if (_lockCursor)
            {
                SetCursorLocked(true);
            }
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _lookAction.Disable();
            _jumpAction.Disable();

            if (_lockCursor)
            {
                SetCursorLocked(false);
            }
        }

        private void Update()
        {
            Look();
            Move();
        }

        private void Look()
        {
            Vector2 lookDelta = Vector2.ClampMagnitude(
                _lookAction.ReadValue<Vector2>(), _maxLookDeltaPerFrame);

            // 마우스 delta는 이미 프레임 간 이동량이다. Time.deltaTime을 곱하면 프레임레이트에 따라 감도가 달라진다
            transform.Rotate(Vector3.up, lookDelta.x * _mouseSensitivity);

            _pitch = Mathf.Clamp(_pitch - lookDelta.y * _mouseSensitivity, _minPitch, _maxPitch);
            _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void Move()
        {
            Vector2 input = _moveAction.ReadValue<Vector2>();
            Vector3 horizontal = (transform.right * input.x + transform.forward * input.y) * _moveSpeed;

            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0f)
                {
                    _verticalVelocity = _groundedStickVelocity;
                }

                if (_jumpAction.WasPressedThisFrame())
                {
                    // 원하는 점프 높이에서 초기 속도를 역산
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                }
            }
            else
            {
                _verticalVelocity += _gravity * Time.deltaTime;
            }

            Vector3 velocity = horizontal + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }

        private void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
