using System;
using ArenaBreak.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ArenaBreak.Core
{
    public class GameManager : MonoBehaviour
    {
        public enum State
        {
            Ready,
            Playing,
            GameOver,
            Cleared
        }

        // InputSystem_Actions 에셋에 없는 키라 코드에서 직접 만든다
        private const string RestartBindingPath = "<Keyboard>/r";

        // 빌드는 전체 화면이라 닫기 버튼이 없다. 종료 수단이 없으면 창을 못 빠져나온다
        private const string QuitBindingPath = "<Keyboard>/escape";

        [Header("References")]
        [SerializeField] private Health _playerHealth;
        [SerializeField] private WaveSpawner _waveSpawner;

        // 게임이 끝나면 꺼야 한다. timeScale = 0 은 Update를 멈추지 않는다
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private WeaponSystem _weaponSystem;

        [Header("Timing")]
        [SerializeField] private float _readyDuration = 1.5f;

        public event Action<State> StateChanged;

        public State Current { get; private set; } = State.Ready;

        private InputAction _restartAction;
        private InputAction _quitAction;
        private float _playingStartTime;

        private void Awake()
        {
            _restartAction = new InputAction("Restart", InputActionType.Button, RestartBindingPath);
            _quitAction = new InputAction("Quit", InputActionType.Button, QuitBindingPath);
        }

        private void OnEnable()
        {
            _playerHealth.Died += OnPlayerDied;
            _waveSpawner.AllWavesCleared += OnAllWavesCleared;
            _restartAction.Enable();
            _quitAction.Enable();

            SetState(State.Ready);
            _playingStartTime = Time.unscaledTime + _readyDuration;
        }

        private void OnDisable()
        {
            _playerHealth.Died -= OnPlayerDied;
            _waveSpawner.AllWavesCleared -= OnAllWavesCleared;
            _restartAction.Disable();
            _quitAction.Disable();
        }

        private void OnDestroy()
        {
            _restartAction.Dispose();
            _quitAction.Dispose();
        }

        private void Update()
        {
            // 어느 상태에서든 나갈 수 있어야 한다. 에디터에서는 아무 일도 하지 않는다
            if (_quitAction.WasPressedThisFrame())
            {
                Application.Quit();
            }

            switch (Current)
            {
                case State.Ready:
                    // timeScale과 무관한 시계를 쓴다. 코루틴을 쓰면 정지 상태에서 멈춘다
                    if (Time.unscaledTime >= _playingStartTime)
                    {
                        SetState(State.Playing);
                    }
                    break;

                case State.Playing:
                    break;

                case State.GameOver:
                case State.Cleared:
                    if (_restartAction.WasPressedThisFrame())
                    {
                        Restart();
                    }
                    break;
            }
        }

        private void OnPlayerDied()
        {
            SetState(State.GameOver);
            Freeze();
        }

        private void OnAllWavesCleared()
        {
            SetState(State.Cleared);
            Freeze();
        }

        private void Freeze()
        {
            Time.timeScale = 0f;

            // 끄면 각자의 OnDisable에서 입력 액션도 함께 해제된다
            _playerController.enabled = false;
            _weaponSystem.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Restart()
        {
            // 씬을 로드하기 전에 되돌려야 새 씬이 멈춘 채로 시작하지 않는다
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void SetState(State next)
        {
            Current = next;
            StateChanged?.Invoke(next);
            Debug.Log($"게임 상태 — {next}");
        }
    }
}
