using System.Collections;
using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Enemy
{
    [RequireComponent(typeof(Health), typeof(MeshRenderer))]
    public class HitFlash : MonoBehaviour
    {
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.05f;

        private Health _health;
        private MeshRenderer _renderer;
        private Color _baseColor;
        private int _lastHealth;
        private Coroutine _running;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _renderer = GetComponent<MeshRenderer>();
            _baseColor = _renderer.material.color;
        }

        private void OnEnable()
        {
            _lastHealth = _health.CurrentHealth;
            _health.HealthChanged += OnHealthChanged;

            // 플래시 도중 풀로 돌아갔다 나오면 흰색으로 굳어 있다
            _renderer.material.color = _baseColor;
        }

        private void OnDisable()
        {
            _health.HealthChanged -= OnHealthChanged;
            _running = null;
        }

        private void OnHealthChanged(int current, int max)
        {
            bool damaged = current < _lastHealth;
            _lastHealth = current;

            if (!damaged)
            {
                return;
            }

            if (_running != null)
            {
                StopCoroutine(_running);
            }

            _running = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            _renderer.material.color = _flashColor;

            // 죽는 순간 timeScale이 0이 되어도 원래 색으로 돌아와야 한다
            yield return new WaitForSecondsRealtime(_flashDuration);

            _renderer.material.color = _baseColor;
            _running = null;
        }
    }
}
