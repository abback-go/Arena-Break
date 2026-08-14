using ArenaBreak.Core;
using ArenaBreak.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaBreak.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private Health _playerHealth;
        [SerializeField] private WeaponSystem _weaponSystem;
        [SerializeField] private WaveSpawner _waveSpawner;
        [SerializeField] private GameManager _gameManager;

        [Header("Widgets")]
        // Image.fillAmount는 Source Image가 있어야 동작한다. 스프라이트 없이 폭으로 줄인다
        [SerializeField] private RectTransform _healthFill;
        [SerializeField] private Text _ammoText;
        [SerializeField] private Text _waveText;
        [SerializeField] private Text _killText;
        [SerializeField] private GameObject _messageRoot;
        [SerializeField] private Text _messageText;

        private void OnEnable()
        {
            _playerHealth.HealthChanged += OnHealthChanged;
            _weaponSystem.AmmoChanged += OnAmmoChanged;
            _waveSpawner.WaveStarted += OnWaveStarted;
            _waveSpawner.KillCountChanged += OnKillCountChanged;
            _gameManager.StateChanged += OnStateChanged;

            // 구독 전에 이미 지나간 값이 있으므로 첫 화면을 직접 맞춘다
            OnHealthChanged(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            OnKillCountChanged(_waveSpawner.KillCount);
            _messageRoot.SetActive(false);
        }

        private void OnDisable()
        {
            _playerHealth.HealthChanged -= OnHealthChanged;
            _weaponSystem.AmmoChanged -= OnAmmoChanged;
            _waveSpawner.WaveStarted -= OnWaveStarted;
            _waveSpawner.KillCountChanged -= OnKillCountChanged;
            _gameManager.StateChanged -= OnStateChanged;
        }

        private void OnHealthChanged(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            _healthFill.anchorMax = new Vector2(ratio, 1f);
        }

        private void OnAmmoChanged(int current, int max)
        {
            _ammoText.text = $"{current} / {max}";
        }

        private void OnWaveStarted(int waveNumber)
        {
            _waveText.text = $"WAVE {waveNumber}";
        }

        private void OnKillCountChanged(int killCount)
        {
            _killText.text = $"KILL {killCount}";
        }

        private void OnStateChanged(GameManager.State state)
        {
            switch (state)
            {
                case GameManager.State.GameOver:
                    ShowMessage("GAME OVER\nR 재시작 · Esc 종료");
                    break;

                case GameManager.State.Cleared:
                    ShowMessage("CLEARED\nR 재시작 · Esc 종료");
                    break;

                default:
                    _messageRoot.SetActive(false);
                    break;
            }
        }

        private void ShowMessage(string message)
        {
            _messageText.text = message;
            _messageRoot.SetActive(true);
        }
    }
}
